using UnityEngine;
using UnityEngine.Windows.Speech;
using TMPro;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class VoiceKeyboardTrainer : MonoBehaviour
{
    public TextMeshProUGUI outputText;

    DictationRecognizer dictationRecognizer;

    string totalText = "";
    string totalRomaji = "";

    string word = "";
    int index = 0;
    bool playing = false;

    Dictionary<char, Vector2Int> keyMap = new Dictionary<char, Vector2Int>()
    {
        {'q', new Vector2Int(2,2)},
        {'w', new Vector2Int(2,3)},
        {'e', new Vector2Int(2,4)},
        {'r', new Vector2Int(2,5)},
        {'t', new Vector2Int(2,6)},
        {'y', new Vector2Int(2,7)},
        {'u', new Vector2Int(2,8)},
        {'i', new Vector2Int(2,9)},
        {'o', new Vector2Int(2,10)},
        {'p', new Vector2Int(2,11)},

        {'a', new Vector2Int(3,2)},
        {'s', new Vector2Int(3,3)},
        {'d', new Vector2Int(3,4)},
        {'f', new Vector2Int(3,5)},
        {'g', new Vector2Int(3,6)},
        {'h', new Vector2Int(3,7)},
        {'j', new Vector2Int(3,8)},
        {'k', new Vector2Int(3,9)},
        {'l', new Vector2Int(3,10)},

        {'z', new Vector2Int(4,3)},
        {'x', new Vector2Int(4,4)},
        {'c', new Vector2Int(4,5)},
        {'v', new Vector2Int(4,6)},
        {'b', new Vector2Int(4,7)},
        {'n', new Vector2Int(4,8)},
        {'m', new Vector2Int(4,9)},

        {' ', new Vector2Int(5,6)}
    };

    [DllImport("RzChromaSDK")]
    public static extern int Init();

    [DllImport("RzChromaSDK")]
    public static extern int UnInit();

    [DllImport("RzChromaSDK")]
    public static extern int CreateKeyboardEffect(
        int effectType,
        IntPtr param,
        IntPtr effectId);

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

        // ⭐ここ追加（起動時空欄）
        totalText = "";
        totalRomaji = "";
        outputText.text = "";

        dictationRecognizer = new DictationRecognizer();

        dictationRecognizer.DictationResult += (text, confidence) =>
        {
            Debug.Log("認識: " + text);

            string romaji = ConvertToRomaji(text);

            totalText += text;
            totalRomaji += romaji;

            outputText.text = totalText;
        };

        dictationRecognizer.DictationComplete += (cause) =>
        {
            Debug.Log("Complete: " + cause);
        };

        dictationRecognizer.DictationError += (error, hresult) =>
        {
            Debug.LogError("Error: " + error);
        };
    }

    void Update()
    {
        KeyboardEffect effect = new KeyboardEffect();
        effect.Color = new int[132];

        if (!playing)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (dictationRecognizer.Status != SpeechSystemStatus.Running)
                {
                    dictationRecognizer.Start();
                }
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                if (dictationRecognizer.Status == SpeechSystemStatus.Running)
                {
                    dictationRecognizer.Stop();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (totalText.Length > 0)
            {
                totalText = totalText.Substring(0, totalText.Length - 1);
            }

            if (totalRomaji.Length > 0)
            {
                totalRomaji = totalRomaji.Substring(0, totalRomaji.Length - 1);
            }

            outputText.text = totalText;
        }

        if (!playing && Input.GetKeyDown(KeyCode.Return))
        {
            word = totalRomaji.ToLower();
            index = 0;

            if (word.Length > 0)
            {
                playing = true;
            }
        }

        if (playing)
        {
            CheckInput();

            if (index < word.Length)
            {
                char currentKey = word[index];

                if (keyMap.ContainsKey(currentKey))
                {
                    Vector2Int pos = keyMap[currentKey];

                    int arrayIndex = pos.x * 22 + pos.y;

                    if (arrayIndex >= 0 && arrayIndex < 132)
                    {
                        effect.Color[arrayIndex] = 0x0000FF;
                    }
                }
            }
        }

        SendEffect(effect);
    }

    void CheckInput()
    {
        if (index >= word.Length) return;

        char target = word[index];

        if (Input.GetKeyDown(target.ToString()))
        {
            index++;

            if (index >= word.Length)
            {
                Finish();
            }
        }
    }

    void Finish()
    {
        playing = false;

        totalText = "";
        totalRomaji = "";

        outputText.text = "";
    }

    void SendEffect(KeyboardEffect effect)
    {
        IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(effect));

        Marshal.StructureToPtr(effect, ptr, false);

        CreateKeyboardEffect(CHROMA_CUSTOM, ptr, IntPtr.Zero);

        Marshal.FreeHGlobal(ptr);
    }

    void OnApplicationQuit()
    {
        UnInit();
    }

    string ConvertToRomaji(string text)
    {
        text = text.ToLower();

        text = text.Replace("あ", "a");
        text = text.Replace("い", "i");
        text = text.Replace("う", "u");
        text = text.Replace("え", "e");
        text = text.Replace("お", "o");

        text = text.Replace("か", "ka");
        text = text.Replace("き", "ki");
        text = text.Replace("く", "ku");
        text = text.Replace("け", "ke");
        text = text.Replace("こ", "ko");

        text = text.Replace("さ", "sa");
        text = text.Replace("し", "shi");
        text = text.Replace("す", "su");
        text = text.Replace("せ", "se");
        text = text.Replace("そ", "so");

        text = text.Replace("た", "ta");
        text = text.Replace("ち", "chi");
        text = text.Replace("つ", "tsu");
        text = text.Replace("て", "te");
        text = text.Replace("と", "to");

        text = text.Replace("な", "na");
        text = text.Replace("に", "ni");
        text = text.Replace("ぬ", "nu");
        text = text.Replace("ね", "ne");
        text = text.Replace("の", "no");

        text = text.Replace("ん", "n");

        return text;
    }
}