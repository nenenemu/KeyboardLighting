using UnityEngine;
using TMPro;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Collections;

public class voiceCS2 : MonoBehaviour
{
    public TextMeshProUGUI outputText;

    private AudioClip clip;
    private string mic;

    private StringBuilder totalText = new StringBuilder();

    public bool isRecording = false;

    Process pythonProcess;
    StreamWriter pythonInput;
    StreamReader pythonOutput;

    public UnityEngine.UI.Image logo;
    public UnityEngine.UI.Image Space1;
    public UnityEngine.UI.Image Space2;
    public UnityEngine.UI.Image Yomikomi;

    void Start()
    {
        totalText.Clear();
        outputText.text = "";

        if (Microphone.devices.Length > 0)
            mic = Microphone.devices[0];

        ProcessStartInfo psi = new ProcessStartInfo();

        psi.FileName = "py";
        psi.Arguments = "-3.10 \"" + Application.dataPath + "/../Python/recognize.py\"";

        psi.UseShellExecute = false;
        psi.RedirectStandardInput = true;
        psi.RedirectStandardOutput = true;
        psi.RedirectStandardError = true;
        psi.CreateNoWindow = true;

        pythonProcess = Process.Start(psi);

        pythonInput = pythonProcess.StandardInput;
        pythonOutput = pythonProcess.StandardOutput;

        UnityEngine.Debug.Log("Python起動完了");

        // 初期UI
        Space1.enabled = true;
        Space2.enabled = false;
        logo.enabled = false;
        Yomikomi.enabled = false;
    }

    void Update()
    {
        // ▼ Enter が押されたら文字を赤にする
        if (Input.GetKeyDown(KeyCode.Return))
        {
            outputText.color = Color.red;
        }

        // ▼ スペース押した瞬間（録音開始）
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isRecording)
            {
                clip = Microphone.Start(mic, false, 5, 16000);
                isRecording = true;

                // UI切り替え（録音中）
                Space1.enabled = false;
                Space2.enabled = true;
                logo.enabled = true;
                Yomikomi.enabled = false;

                UnityEngine.Debug.Log("録音開始");
            }
        }

        // ▼ スペース離した瞬間（録音終了 → Python処理中）
        if (Input.GetKeyUp(KeyCode.Space))
        {

            if (isRecording)
            {
                StartCoroutine(ProcessVoice());
            }

            /*if (isRecording)
            {
                Microphone.End(mic);

                // UI切り替え（Python処理中）
                Space1.enabled = false;
                Space2.enabled = false;
                logo.enabled = false;
                //Yomikomi.enabled = true;

                string path = Application.dataPath + "/../python/voice.wav";

                SaveWav(path, clip);

                UnityEngine.Debug.Log("保存先: " + path);

                // Python処理
                string result = RunPython(path);

                // ▼ Python返答後のUI
                Space1.enabled = true;
                Space2.enabled = false;
                logo.enabled = false;
                Yomikomi.enabled = false;

                totalText.Append(result);
                outputText.text = totalText.ToString();

                isRecording = false;
                UnityEngine.Debug.Log("録音終了");
            }*/
        }

        // ▼ バックスペースで削除
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (totalText.Length > 0)
            {
                totalText.Remove(totalText.Length - 1, 1);
                outputText.text = totalText.ToString();
            }
        }
    }

    string RunPython(string wav)
    {
        pythonInput.WriteLine(wav);
        pythonInput.Flush();

        string result = pythonOutput.ReadLine();

        while (string.IsNullOrEmpty(result))
        {
            result = pythonOutput.ReadLine();
        }

        UnityEngine.Debug.Log("Python OUTPUT: " + result);

        return result;
    }

    public string GetText()
    {
        return totalText.ToString();
    }

    public void ClearText()
    {
        totalText.Clear();
        outputText.text = "";

        // ★ 全部打ち終わったので白に戻す
        outputText.color = Color.white;
    }

    void SaveWav(string path, AudioClip clip)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path));

        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        using (FileStream fs = new FileStream(path, FileMode.Create))
        using (BinaryWriter bw = new BinaryWriter(fs))
        {
            int sampleRate = clip.frequency;
            int channels = clip.channels;

            bw.Write(System.Text.Encoding.UTF8.GetBytes("RIFF"));
            bw.Write(36 + samples.Length * 2);
            bw.Write(System.Text.Encoding.UTF8.GetBytes("WAVE"));

            bw.Write(System.Text.Encoding.UTF8.GetBytes("fmt "));
            bw.Write(16);
            bw.Write((short)1);
            bw.Write((short)channels);
            bw.Write(sampleRate);
            bw.Write(sampleRate * channels * 2);
            bw.Write((short)(channels * 2));
            bw.Write((short)16);

            bw.Write(System.Text.Encoding.UTF8.GetBytes("data"));
            bw.Write(samples.Length * 2);

            for (int i = 0; i < samples.Length; i++)
            {
                short v = (short)(samples[i] * short.MaxValue);
                bw.Write(v);
            }
        }
    }

    void OnApplicationQuit()
    {
        if (pythonProcess != null && !pythonProcess.HasExited)
        {
            pythonProcess.Kill();
        }
    }

    public IEnumerator ProcessVoice()
    {
        Microphone.End(mic);

        Space1.enabled = false;
        Space2.enabled = false;
        logo.enabled = false;
        Yomikomi.enabled = true;

        // ここが重要
        yield return null;

        string path = Application.dataPath + "/../python/voice.wav";

        SaveWav(path, clip);

        string result = RunPython(path);

        Space1.enabled = true;
        Space2.enabled = false;
        logo.enabled = false;
        Yomikomi.enabled = false;

        totalText.Append(result);
        outputText.text = totalText.ToString();

        isRecording = false;
    }
}
