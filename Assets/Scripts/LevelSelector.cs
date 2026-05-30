using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

public class LevelSelector : MonoBehaviour
{
    public static GameObject lastSelectedButton = null;
    public GameObject defaultButton; // ← Level1 のボタンを入れる

    void Start()
    {
        StartCoroutine(SelectAfterUIReady());
    }

    IEnumerator SelectAfterUIReady()
    {
        // UI が揃うまで 1 フレーム待つ（これが全て）
        yield return null;

        // 一旦クリア（null 選択を防ぐ）
        EventSystem.current.SetSelectedGameObject(null);

        // 最後に押したボタンがあればそれを選択
        if (lastSelectedButton != null)
        {
            EventSystem.current.SetSelectedGameObject(lastSelectedButton);
        }
        else
        {
            // 初回は defaultButton（Level1）
            EventSystem.current.SetSelectedGameObject(defaultButton);
        }
    }

    public void Level1(GameObject button)
    {
        lastSelectedButton = button;
        SceneManager.LoadScene("Level1");
    }

    public void Level2(GameObject button)
    {
        lastSelectedButton = button;
        SceneManager.LoadScene("Level2");
    }

    public void Level3(GameObject button)
    {
        lastSelectedButton = button;
        SceneManager.LoadScene("Level3");
    }
}
