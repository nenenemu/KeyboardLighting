using UnityEngine;

public class GetkeyST : MonoBehaviour
{
    public Animator animator1;
    public Animator animator2;

    public bool canEscape = true;

    void Start()
    {
        animator1.SetBool("End", false);
        animator2.SetBool("End", false);
    }

    void Update()
    {
        if (!canEscape)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            animator2.SetBool("End", true);
        }
    }
}