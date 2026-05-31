using UnityEngine;
using UnityEngine.EventSystems;

public class ForceSelect : MonoBehaviour
{
    public GameObject Button1;
    public GameObject Button2; // ← 選択させたいボタン
    public GameObject targetButton; // ← 選択させたいボタン


    void Start()
    {
        StartCoroutine(SelectNextFrame());
    }

    System.Collections.IEnumerator SelectNextFrame()
    {
        yield return null; // UI が準備されるまで1フレーム待つ
        EventSystem.current.SetSelectedGameObject(targetButton);
    }
}
