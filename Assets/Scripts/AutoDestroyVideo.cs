using UnityEngine;
using UnityEngine.Video;

public class AutoDestroyVideo : MonoBehaviour
{
    VideoPlayer vp;

    void Start()
    {
        vp = GetComponentInChildren<VideoPlayer>();

        vp.loopPointReached += Finished;

        vp.Play();
    }

    void Finished(VideoPlayer source)
    {
        Destroy(gameObject);
    }
}