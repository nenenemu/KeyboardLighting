using TMPro;
using UnityEngine;

public class RomajiDisplay : MonoBehaviour
{
    public voiceCS2 voice;
    public TextMeshProUGUI romajiText;

    void Update()
    {
        if (voice == null || romajiText == null)
            return;

        romajiText.text =
            RomajiConverter.KanaToRomaji(voice.GetText());
    }
}

