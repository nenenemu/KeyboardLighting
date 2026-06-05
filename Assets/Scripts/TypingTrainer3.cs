using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class TypingTrainer3 : MonoBehaviour
{
    public RomajiDisplay romajiDisplay;
    public voiceCS2 voice;
    public float highlightDelay = 7f;

    string word = "";
    public int index = 0;
    public bool playing = false;

    float timer = 0f;

    int[] keyColors = new int[132];

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
    }

    void Update()
    {
        // ⭐ 毎フレームリセット（重要）
        ResetColors();

        // エンターで開始
        if (!playing && Input.GetKeyDown(KeyCode.Return))
        {
            string kana = voice.GetText();
            word = KanaToRomaji(kana);

            index = 0;
            timer = 0f;

            if (word.Length > 0)
                playing = true;
        }

        if (playing && index < word.Length)
        {
            char current = word[index];

            timer += Time.deltaTime;

            // ⭐ 正解キー押したら次へ
            if (Input.GetKeyDown(current.ToString()))
            {
                index++;
                romajiDisplay.currentIndex = index;
                timer = 0f;

                if (index >= word.Length)
                {
                    playing = false;
                    voice.ClearText();
                }

                return; // ←このフレームで余計な描画をしない（重要）
            }

            // ⭐ 最初のキーはずっと光る
            if (index == 0)
            {
                SetKeyColor(current, 0x0000FF);
            }
            else
            {
                // ⭐ 2文字目以降は遅延後に光る
                if (timer >= highlightDelay)
                {
                    SetKeyColor(current, 0x0000FF);
                }
            }

            // ⭐ 間違いキー（押した瞬間だけ赤）
            foreach (char k in Input.inputString)
            {
                if (k != current)
                {
                    SetKeyColor(k, 0xFF0000);
                }
            }
        }

        SendEffect();
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

    // そのまま使ってOK
    string KanaToRomaji(string input)
    {
        Dictionary<string, string> map = new Dictionary<string, string>()
        {
            {"あ","a"},{"い","i"},{"う","u"},{"え","e"},{"お","o"},
            {"か","ka"},{"き","ki"},{"く","ku"},{"け","ke"},{"こ","ko"},
            {"さ","sa"},{"し","shi"},{"す","su"},{"せ","se"},{"そ","so"},
            {"た","ta"},{"ち","chi"},{"つ","tsu"},{"て","te"},{"と","to"},
            {"な","na"},{"に","ni"},{"ぬ","nu"},{"ね","ne"},{"の","no"},
            {"は","ha"},{"ひ","hi"},{"ふ","fu"},{"へ","he"},{"ほ","ho"},
            {"ま","ma"},{"み","mi"},{"む","mu"},{"め","me"},{"も","mo"},
            {"や","ya"},{"ゆ","yu"},{"よ","yo"},
            {"ら","ra"},{"り","ri"},{"る","ru"},{"れ","re"},{"ろ","ro"},
            {"わ","wa"},{"を","wo"},{"ん","n"}
        };

        string result = "";
        foreach (char c in input)
        {
            string s = c.ToString();
            if (map.ContainsKey(s))
                result += map[s];
        }
        return result;
    }

    void OnApplicationQuit()
    {
        UnInit();
    }
}