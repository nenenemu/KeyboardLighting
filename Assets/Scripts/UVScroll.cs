using UnityEngine;
using UnityEngine.UI;

public class UVScroll : MonoBehaviour
{
    public float speedX = 0.02f;
    public float speedY = -0.02f;

    private Material mat;

    void Awake()
    {
        // ★ 背景をシーンをまたいでも消さない
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // ★ RawImage のマテリアルを取得
        mat = GetComponent<RawImage>().material;
    }

    void Update()
    {
        // ★ ずっとスクロールし続ける
        mat.mainTextureOffset += new Vector2(speedX, speedY) * Time.deltaTime;
    }
}
