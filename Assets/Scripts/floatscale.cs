using UnityEngine;
using UnityEngine.UI;

public class FloatScale : MonoBehaviour
{
    public float scaleAmount = 0.1f;   // ‚Ç‚ê‚­‚ç‚¢‘å‚«‚³‚ª•Ï‚í‚é‚©
    public float speed = 2f;           // ƒtƒƒtƒ‚·‚é‘¬‚³

    private Vector3 baseScale;

    void Start()
    {
        baseScale = transform.localScale;
    }

    void Update()
    {
        float s = 1 + Mathf.Sin(Time.time * speed) * scaleAmount;
        transform.localScale = baseScale * s;
    }
}
