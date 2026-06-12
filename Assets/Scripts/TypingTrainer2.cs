using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.Video;

public class TypingTrainer2 : MonoBehaviour
{
    public GameObject videoPrefab;
    public Transform videoSpawnPoint;

    public RomajiDisplay romajiDisplay;
    public voiceCS2 voice;

    string word = "";
    public int index = 0;
    public bool playing = false;

    Dictionary<char, Vector2Int> keyMap = new Dictionary<char, Vector2Int>()
    {
        {'q', new Vector2Int(2,2)}, {'w', new Vector2Int(2,3)}, {'e', new Vector2Int(2,4)},
        {'r', new Vector2Int(2,5)}, {'t', new Vector2Int(2,6)}, {'y', new Vector2Int(2,7)},
        {'u', new Vector2Int(2,8)}, {'i', new Vector2Int(2,9)}, {'o', new Vector2Int(2,10)},
        {'p', new Vector2Int(2,11)}, {'a', new Vector2Int(3,2)}, {'s', new Vector2Int(3,3)},
        {'d', new Vector2Int(3,4)}, {'f', new Vector2Int(3,5)}, {'g', new Vector2Int(3,6)},
        {'h', new Vector2Int(3,7)}, {'j', new Vector2Int(3,8)}, {'k', new Vector2Int(3,9)},
        {'l', new Vector2Int(3,10)}, {'z', new Vector2Int(4,3)}, {'x', new Vector2Int(4,4)},
        {'c', new Vector2Int(4,5)}, {'v', new Vector2Int(4,6)}, {'b', new Vector2Int(4,7)},
        {'n', new Vector2Int(4,8)}, {'m', new Vector2Int(4,9)}, {' ', new Vector2Int(5,6)}
    };

    [DllImport("RzChromaSDK")] public static extern int Init();
    [DllImport("RzChromaSDK")] public static extern int UnInit();
    [DllImport("RzChromaSDK")] public static extern int CreateKeyboardEffect(int effectType, IntPtr param, IntPtr effectId);

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
        KeyboardEffect effect = new KeyboardEffect();
        effect.Color = new int[132];

        // Enter で開始
        if (!playing && Input.GetKeyDown(KeyCode.Return))
        {
            string kana = voice.GetText();
            word = KanaToRomaji(kana);
            index = 0;

            if (word.Length > 0)
            {
                playing = true;
                voice.Space1.enabled = false;
            }
        }

        if (playing)
        {
            if (index < word.Length)
            {
                char current = word[index];

                // キーを光らせる
                if (keyMap.ContainsKey(current))
                {
                    Vector2Int pos = keyMap[current];
                    int arrayIndex = pos.x * 22 + pos.y;
                    effect.Color[arrayIndex] = 0x0000FF;
                }

                // 入力チェック
                if (Input.GetKeyDown(current.ToString()))
                {
                    index++;
                    romajiDisplay.currentIndex = index;

                    if (index >= word.Length)
                    {
                        playing = false;
                        voice.ClearText();
                        voice.Space1.enabled = true;

                        SpawnVideo();
                    }
                }
            }
        }

        SendEffect(effect);
    }

    void SendEffect(KeyboardEffect effect)
    {
        IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(effect));
        Marshal.StructureToPtr(effect, ptr, false);
        CreateKeyboardEffect(CHROMA_CUSTOM, ptr, IntPtr.Zero);
        Marshal.FreeHGlobal(ptr);
    }

    string KanaToRomaji(string input)
    {
        Dictionary<string, string> map = new Dictionary<string, string>()
        {
            {"きゃ","kya"},{"きゅ","kyu"},{"きょ","kyo"},
            {"しゃ","sha"},{"しゅ","shu"},{"しょ","sho"},
            {"ちゃ","cha"},{"ちゅ","chu"},{"ちょ","cho"},
            {"にゃ","nya"},{"にゅ","nyu"},{"にょ","nyo"},
            {"ひゃ","hya"},{"ひゅ","hyu"},{"ひょ","hyo"},
            {"みゃ","mya"},{"みゅ","myu"},{"みょ","myo"},
            {"りゃ","rya"},{"りゅ","ryu"},{"りょ","ryo"},
            {"ぎゃ","gya"},{"ぎゅ","gyu"},{"ぎょ","gyo"},
            {"じゃ","ja"},{"じゅ","ju"},{"じょ","jo"},
            {"びゃ","bya"},{"びゅ","byu"},{"びょ","byo"},
            {"ぴゃ","pya"},{"ぴゅ","pyu"},{"ぴょ","pyo"},
            {"あ","a"},{"い","i"},{"う","u"},{"え","e"},{"お","o"},
            {"か","ka"},{"き","ki"},{"く","ku"},{"け","ke"},{"こ","ko"},
            {"さ","sa"},{"し","shi"},{"す","su"},{"せ","se"},{"そ","so"},
            {"た","ta"},{"ち","chi"},{"つ","tsu"},{"て","te"},{"と","to"},
            {"な","na"},{"に","ni"},{"ぬ","nu"},{"ね","ne"},{"の","no"},
            {"は","ha"},{"ひ","hi"},{"ふ","fu"},{"へ","he"},{"ほ","ho"},
            {"ま","ma"},{"み","mi"},{"む","mu"},{"め","me"},{"も","mo"},
            {"や","ya"},{"ゆ","yu"},{"よ","yo"},
            {"ら","ra"},{"り","ri"},{"る","ru"},{"れ","re"},{"ろ","ro"},
            {"わ","wa"},{"を","wo"},{"ん","n"},
            {"が","ga"},{"ぎ","gi"},{"ぐ","gu"},{"げ","ge"},{"ご","go"},
            {"ざ","za"},{"じ","ji"},{"ず","zu"},{"ぜ","ze"},{"ぞ","zo"},
            {"だ","da"},{"ぢ","ji"},{"づ","zu"},{"で","de"},{"ど","do"},
            {"ば","ba"},{"び","bi"},{"ぶ","bu"},{"べ","be"},{"ぼ","bo"},
            {"ぱ","pa"},{"ぴ","pi"},{"ぷ","pu"},{"ぺ","pe"},{"ぽ","po"},
            {"ふぁ","fa"},{"ふぃ","fi"},{"ふぇ","fe"},{"ふぉ","fo"},
            {"てぃ","ti"},{"でぃ","di"},{"とぅ","tu"},{"どぅ","du"},
            {"うぃ","wi"},{"うぇ","we"},{"うぉ","wo"},
            {"しぇ","she"},{"じぇ","je"},{"ちぇ","che"},
            {"つぁ","tsa"},{"つぃ","tsi"},{"つぇ","tse"},{"つぉ","tso"},
            {"てゅ","tyu"},{"でゅ","dyu"},
            {"ゔぁ","va"},{"ゔぃ","vi"},{"ゔ","vu"},{"ゔぇ","ve"},{"ゔぉ","vo"},
            {"ぁ","xa"},{"ぃ","xi"},{"ぅ","xu"},{"ぇ","xe"},{"ぉ","xo"},
            {"ゃ","xya"},{"ゅ","xyu"},{"ょ","xyo"}
        };

        string result = "";

        for (int i = 0; i < input.Length; i++)
        {
            string current = input[i].ToString();

            // 促音（っ）
            if (current == "っ" && i + 1 < input.Length)
            {
                string next = input.Substring(i + 1, 1);

                if (i + 2 <= input.Length)
                {
                    string two = input.Substring(i + 1, Mathf.Min(2, input.Length - (i + 1)));
                    if (map.ContainsKey(two))
                    {
                        result += map[two][0];
                        continue;
                    }
                }

                if (map.ContainsKey(next))
                    result += map[next][0];

                continue;
            }

            // 長音（ー）
            if (current == "ー")
            {
                if (result.Length > 0)
                {
                    char last = result[result.Length - 1];
                    if ("aeiou".Contains(last))
                        result += last;
                }
                continue;
            }

            // 2文字（拗音）
            if (i < input.Length - 1)
            {
                string two = input.Substring(i, 2);
                if (map.ContainsKey(two))
                {
                    result += map[two];
                    i++;
                    continue;
                }
            }

            // 1文字
            if (map.ContainsKey(current))
                result += map[current];
        }

        return result;
    }

    void ClearKeyboard()
    {
        KeyboardEffect effect = new KeyboardEffect();
        effect.Color = new int[132];

        IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(effect));
        Marshal.StructureToPtr(effect, ptr, false);
        CreateKeyboardEffect(CHROMA_CUSTOM, ptr, IntPtr.Zero);
        Marshal.FreeHGlobal(ptr);
    }

    void OnDisable()
    {
        ClearKeyboard();
    }

    void OnApplicationQuit()
    {
        UnInit();
    }

    void SpawnVideo()
    {
        GameObject obj = Instantiate(videoPrefab, videoSpawnPoint);

        VideoPlayer vp = obj.GetComponent<VideoPlayer>();

        if (vp != null)
        {
            vp.isLooping = false;

            vp.loopPointReached += OnVideoFinished;

            vp.Play();
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        Destroy(vp.gameObject);
    }
}
