using UnityEngine;

public class Setumei : MonoBehaviour
{
    public Animator TObject;

    public UnityEngine.UI.Image image1;

    void Start()
    {
        TObject = GetComponent<Animator>();

        // 最初は表示
        TObject.SetBool("ON", false);
    }

    void Update()
    {
        // Mキーで表示
        if (Input.GetKey(KeyCode.M))
        {
            //image1.enabled = false; // 画像を表示
            //Debug.Log("Mキーが押されました");
            TObject.SetBool("ON", true);
        }

        if (Input.GetKeyUp(KeyCode.M))
        {
            //image1.enabled = true;
            //Debug.Log("Mキーが離されました");
            TObject.SetBool("ON", false);
        }

        
    }

    public void MON()
    {
        image1.enabled = true;
        
    }

    public void MOFF()
    {
        image1.enabled = false;
    }
}