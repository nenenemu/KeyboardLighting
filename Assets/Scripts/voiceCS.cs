using UnityEngine;
using TMPro;
using System.Text;
using System.Diagnostics;
using System.IO;

public class voiceCS : MonoBehaviour
{
    public TextMeshProUGUI outputText;

    private AudioClip clip;
    private string mic;

    private StringBuilder totalText = new StringBuilder();

    public bool isRecording = false;

    Process pythonProcess;
    StreamWriter pythonInput;
    StreamReader pythonOutput;

    void Start()
    {
        totalText.Clear();
        outputText.text = "";

        if (Microphone.devices.Length > 0)
            mic = Microphone.devices[0];//miicrophoneはマイク

        // ↓ここから追加
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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isRecording)
            {
                clip = Microphone.Start(mic, false, 5, 16000);
                isRecording = true;
                UnityEngine.Debug.Log("録音開始");
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (isRecording)
            {
                Microphone.End(mic);

                string path = Application.dataPath + "/../python/voice.wav";

                SaveWav(path, clip);

                UnityEngine.Debug.Log("保存先: " + path);

                string result = RunPython(path);

                totalText.Append(result);
                outputText.text = totalText.ToString();

                isRecording = false;
                UnityEngine.Debug.Log("録音終了");
            }
        }

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
        // 送信
        pythonInput.WriteLine(wav);
        pythonInput.Flush();

        // 受信（ここで1回だけ読む）
        string result = pythonOutput.ReadLine();

        // 空ならもう一回読む（ズレ対策）
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
    }

    void SaveWav(string path, AudioClip clip)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path));

        float[] samples = new float[clip.samples * clip.channels];//float[] samples = new float[clip.samples];
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
    }//SaveWavのところが原因？

    void OnApplicationQuit()
    {
        if (pythonProcess != null && !pythonProcess.HasExited)
        {
            pythonProcess.Kill();
        }
    }
}
