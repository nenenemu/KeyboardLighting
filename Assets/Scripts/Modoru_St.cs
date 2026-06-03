using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Modoru_St : MonoBehaviour
{
    public Animator animator1;
    public Animator animator2;

    //public UnityEngine.UI.Image Image1;
    //public TextMeshProUGUI TX1;
    //public TextMeshProUGUI TX2;
    //public TextMeshProUGUI TX3;

    public GameObject Panel;

    public GameObject Panel2;

    //public static bool isStart;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //isStart = false;

        Panel.SetActive(false);
        if (Panel2 != null)
        {
            Panel2.SetActive(false);
        }
        
        //animator1.Play("naninani");
        //animator2.Play("nani");





    }

    // Update is called once per frame
    void Update()
    {
        


    }

    public void AStart()
    {
        //Panel.SetActive(true);
        animator2.SetBool("Start", true);
        //TX1.enabled = false;

    }

    public void AStart2()
    {
        Panel.SetActive(true);
        if (Panel2 != null)
        {
            Panel2.SetActive(true);
        }
        
    }

    public void AEnd()
    {
        Debug.Log("AEnd111111111111");
        Panel.SetActive(false);
        if (Panel2 != null)
        {
            Panel2.SetActive(false);
        }
        


    }

    public void AEnd2()
    {
        SceneManager.LoadScene("FirstScene");

    }

    public void AEnd3()
    {
        Debug.Log("AEnd3");
        animator1.SetBool("End", true);
    }



}
