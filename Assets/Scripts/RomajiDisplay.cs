using TMPro;
using UnityEngine;

public class RomajiDisplay : MonoBehaviour
{
    public voiceCS2 voice;
    public TextMeshProUGUI romajiText;

    public TypingTrainer2 trainer2;
    public TypingTrainer3 trainer3;

    public int currentIndex = 0;

    void Update()
    {
        if (voice == null || romajiText == null)
            return;

        string romaji =
            RomajiConverter.KanaToRomaji(voice.GetText());

        if (romaji.Length == 0)
        {
            romajiText.text = "";
            return;
        }

        bool isPlaying = false;

        if (trainer2 != null)
        {
            isPlaying = trainer2.playing;
            currentIndex = trainer2.index;
        }

        if (trainer3 != null)
        {
            isPlaying = trainer3.playing;
            currentIndex = trainer3.index;
        }

        currentIndex = Mathf.Clamp(currentIndex, 0, romaji.Length);

        string display = "";

        for (int i = 0; i < romaji.Length; i++)
        {
            if (isPlaying && i == currentIndex)
                display += "<color=red>" + romaji[i] + "</color>";
            else
                display += romaji[i];
        }

        romajiText.text = display;
    }
}