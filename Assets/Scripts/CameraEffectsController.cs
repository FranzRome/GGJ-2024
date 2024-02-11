using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraEffectsController : MonoBehaviour
{
    private PostProcessVolume post;

    // Start is called before the first frame update
    void Start()
    {
        post = GetComponent<PostProcessVolume>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartFade()
    {

    }
    
    IEnumerator Fade(float duration, float bloomStart = 0)
    {
        while(duration > 0)
        {
          yield return null;
        }
    }
}
