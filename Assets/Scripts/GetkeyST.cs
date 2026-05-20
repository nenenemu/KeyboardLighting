using UnityEngine;

public class GetkeyST : MonoBehaviour
{
    public Animator animator1;
    public Animator animator2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator1.SetBool("End", false);
        animator2.SetBool("End", false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //animator1.SetBool("End", true);
            animator2.SetBool("End", true);
        }
    }
}
