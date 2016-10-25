using UnityEngine;
using System.Collections;

public class IconAnimator : MonoBehaviour {
    public float duration = 1f;
    float minValue = 0.4f;
    public bool playing = false;
    AudioSource myAudio;
    // Use this for initialization
    void Start()
    {
        transform.localScale = new Vector3(1,1,1);
        myAudio = GetComponent<AudioSource>();
    }
    public void AnimateIt () {
        if (playing == false) {
            StartCoroutine(NeoAnimation());
            playing = true;
        }
    }
    private IEnumerator NeoAnimation()
    {
        float time = 0;
        while (time < 1f)
        {
            float scale = 1 - Mathf.Sin(1 * Mathf.PI * duration * time);
            time += Time.deltaTime / duration;
            transform.localScale = new Vector3(scale, scale, scale);
            if (scale < minValue)
            {
                transform.localScale = new Vector3(minValue, minValue, minValue);
            }
            yield return new WaitForEndOfFrame();
        }
        if (duration > 0.1f) {
            myAudio.Play();
        }
        playing = false;
    }
}
