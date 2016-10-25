using UnityEngine;
using System.Collections;

public class AudioClipData {

    // --------------- External Set ---------------
    public int audioId;
    public float initialVolume;
    public float birthTimeStamp;
    public float fadeSpeed = -1f;
    public string audioName;
    public bool killOnZeroVolume = true;
    public GameObject parent;
    public AudioSource audioSource;
    public AudioManager audioManager;
    // --------------- External Set ---------------

    private float currentVolume = 1f;
    private float targetVolume = 1f;
    private float timer = 0.1f;

    public void UpdateAudioClip() {
        if (audioSource == null) {
            Destroy();
            return;
        }

        if (!audioSource.loop && (audioSource.clip.length < timer)) {
            Destroy();
            return;
        }

        if (killOnZeroVolume && currentVolume <= 0) {
            Destroy();
            return;
        }

        if (audioSource.isPlaying) {
            timer += Time.deltaTime;
        }

        FadeAudio();
        
        audioSource.volume = currentVolume;
    }

    private void FadeAudio() {
        float deltaTime = Time.deltaTime;

        if (currentVolume != targetVolume) {
            if (fadeSpeed <= 0f) {
                currentVolume = targetVolume;
            } else {
                if (currentVolume < targetVolume) {
                    currentVolume = Mathf.Min(targetVolume, currentVolume + (deltaTime * fadeSpeed));
                } else {
                    currentVolume = Mathf.Max(targetVolume, currentVolume - (deltaTime * fadeSpeed));
                }
            }
        }
    }

    public void SetVolume(float volume) {
        this.currentVolume = volume;
        this.targetVolume = volume;
        this.fadeSpeed = -1f;
    }

    public void SetTargetVolume(float targetVolume, float fadeSpeed) {
        this.targetVolume = targetVolume;
        this.fadeSpeed = fadeSpeed;
    }

    private void Destroy() {
        audioManager.activeAudio.Remove(this);

        if (audioSource != null) {
            GameObject.Destroy(audioSource.gameObject);
        }
    }
}