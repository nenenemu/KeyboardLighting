using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class LevelSelector : MonoBehaviour
{
    [Header("Buttons")]
    public GameObject Button1;
    public GameObject Button2;
    public GameObject Button3;

    // ç≈å„Ç…ëIëÇµÇƒÇ¢ÇΩÉ{É^Éìî‘çÜ
    public static int lastSelected = 1;

    private void Start()
    {
        StartCoroutine(SelectNextFrame());
    }

    System.Collections.IEnumerator SelectNextFrame()
    {
        yield return null;

        switch (lastSelected)
        {
            case 1:
                EventSystem.current.SetSelectedGameObject(Button1);
                break;

            case 2:
                EventSystem.current.SetSelectedGameObject(Button2);
                break;

            case 3:
                EventSystem.current.SetSelectedGameObject(Button3);
                break;

            default:
                EventSystem.current.SetSelectedGameObject(Button1);
                break;
        }
    }

    public void Level1()
    {
        lastSelected = 1;
        SceneManager.LoadScene("Level1");
    }

    public void Level2()
    {
        lastSelected = 2;
        SceneManager.LoadScene("Level2");
    }

    public void Level3()
    {
        lastSelected = 3;
        SceneManager.LoadScene("Level3");
    }
}