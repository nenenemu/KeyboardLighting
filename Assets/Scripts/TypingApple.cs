using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;

public class TypingApple : MonoBehaviour
{
    public string word = "apple";
    int index = 0;
    bool playing = false;

    int[] keyColors = new int[132];

    // 間違えたキー保持
    HashSet<char> wrongKeys = new HashSet<char>();

    // 正解キー表示フラグ（スペース押したときだけ）
    bool showHint = false;

    Process pythonProcess;
    StreamWriter pythonInput;

    Dictionary<char, Vector2Int> keyMap = new Dictionary<char, Vector2Int>()
    {
        {'q', new Vector2Int(2,2)},{'w', new Vector2Int(2,3)},
        {'e', new Vector2Int(2,4)},{'r', new Vector2Int(2,5)},
        {'t', new Vector2Int(2,6)},{'y', new Vector2Int(2,7)},
        {'u', new Vector2Int(2,8)},{'i', new Vector2Int(2,9)},
        {'o', new Vector2Int(2,10)},{'p', new Vector2Int(2,11)},
        {'a', new Vector2Int(3,2)},{'s', new Vector2Int(3,3)},
        {'d', new Vector2Int(3,4)},{'f', new Vector2Int(3,5)},
        {'g', new Vector2Int(3,6)},{'h', new Vector2Int(3,7)},
        {'j', new Vector2Int(3,8)},{'k', new Vector2Int(3,9)},
        {'l', new Vector2Int(3,10)},
        {'z', new Vector2Int(4,3)},{'x', new Vector2Int(4,4)},
        {'c', new Vector2Int(4,5)},{'v', new Vector2Int(4,6)},
        {'b', new Vector2Int(4,7)},{'n', new Vector2Int(4,8)},
        {'m', new Vector2Int(4,9)},
        {' ', new Vector2Int(5,6)}
    };

    [DllImport("RzChromaSDK")]
    public static extern int Init();
    [DllImport("RzChromaSDK")]
    public static extern int UnInit();
    [DllImport("RzChromaSDK")]
    public static extern int CreateKeyboardEffect(int effectType, IntPtr param, IntPtr effectId);

    const int CHROMA_CUSTOM = 2;

    [StructLayout(LayoutKind.Sequential)]
    struct KeyboardEffect
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 132)]
        public int[] Color;
    }

    void Start()
    {
        Init();
        StartPython();
    }

    void Update()
    {
        ResetColors();

        if (playing && index < word.Length)
        {
            char current = word[index];

            // スペース押したらヒント表示ON
            if (Input.GetKeyDown(KeyCode.Space))
            {
                showHint = true;
            }

            // 正解キー表示（スペース押したときだけ）
            if (showHint)
            {
                SetKeyColor(current, 0x0000FF);
            }

            // 間違いキーはずっと光る
            foreach (char c in wrongKeys)
            {
                SetKeyColor(c, 0x0000FF);
            }

            // 入力処理
            foreach (char k in Input.inputString)
            {
                if (k == current)
                {
                    index++;
                    wrongKeys.Clear();
                    showHint = false;

                    if (index >= word.Length)
                    {
                        playing = false;
                    }
                }
                else
                {
                    wrongKeys.Add(k);
                }
            }
        }

        SendEffect();
    }

    // ⭐ ボタンから呼ぶ
    public void StartTyping()
    {
        index = 0;
        wrongKeys.Clear();
        showHint = false;
        playing = true;

        Speak(word);
    }

    void StartPython()
    {
        ProcessStartInfo psi = new ProcessStartInfo();

        psi.FileName = "py";
        psi.Arguments = "-3.10 \"" + Application.dataPath + "/../Python/speak.py\"";

        psi.UseShellExecute = false;
        psi.RedirectStandardInput = true;
        psi.CreateNoWindow = true;

        pythonProcess = Process.Start(psi);
        pythonInput = pythonProcess.StandardInput;
    }

    void Speak(string text)
    {
        if (pythonProcess == null || pythonProcess.HasExited) return;

        pythonInput.WriteLine(text);
        pythonInput.Flush();
    }

    void ResetColors()
    {
        for (int i = 0; i < keyColors.Length; i++)
            keyColors[i] = 0;
    }

    void SetKeyColor(char c, int color)
    {
        if (!keyMap.ContainsKey(c)) return;

        Vector2Int pos = keyMap[c];
        int idx = pos.x * 22 + pos.y;
        keyColors[idx] = color;
    }

    void SendEffect()
    {
        KeyboardEffect effect = new KeyboardEffect();
        effect.Color = new int[132];

        Array.Copy(keyColors, effect.Color, 132);

        IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(effect));
        Marshal.StructureToPtr(effect, ptr, false);
        CreateKeyboardEffect(CHROMA_CUSTOM, ptr, IntPtr.Zero);
        Marshal.FreeHGlobal(ptr);
    }

    void OnApplicationQuit()
    {
        UnInit();

        if (pythonProcess != null && !pythonProcess.HasExited)
            pythonProcess.Kill();
    }
}