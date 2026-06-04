using TMPro;
using UnityEngine;

public class RomajiDisplay : MonoBehaviour
{
    public voiceCS2 voice;
    public TextMeshProUGUI romajiText;

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

        currentIndex = Mathf.Clamp(currentIndex, 0, romaji.Length);

        string display = "";

        for (int i = 0; i < romaji.Length; i++)
        {
            if (i == currentIndex)
                display += "<color=red>" + romaji[i] + "</color>";
            else
                display += romaji[i];
        }

        romajiText.text = display;
    }
}