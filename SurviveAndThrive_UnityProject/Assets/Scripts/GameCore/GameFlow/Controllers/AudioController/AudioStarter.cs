using UnityEngine;
using System.Collections;

public class AudioStarter : MonoBehaviour {

	// --------------- Tweakables ---------------

	private float randomMinTime = 0.0f;
	private float randomMaxTime = 60.0f;

	// --------------- Tweakables ---------------

	private AudioSource audioSource;

	private void Start() {
		audioSource = GetComponent<AudioSource>();

		StartCoroutine(RandomAudioStart(0));
	}

	private IEnumerator RandomAudioStart(float time) {
		yield return new WaitForSeconds(time);
		audioSource.Play();
		float randomTime = audioSource.clip.length + Random.Range(randomMinTime, randomMaxTime);
		StartCoroutine(RandomAudioStart(randomTime));
	}
}