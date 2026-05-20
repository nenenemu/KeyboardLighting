using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    public static int currentLevel = 1;

    public static bool Lv1 = false;
    public static bool Lv2 = false;
    public static bool Lv3 = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Level1()
    {
        SceneManager.LoadScene("Level1");
    }


    public void Level2()
    {

        SceneManager.LoadScene("Level2");
    }

    public void Level3()
    {
    
        SceneManager.LoadScene("Level3");
    }
    


}
