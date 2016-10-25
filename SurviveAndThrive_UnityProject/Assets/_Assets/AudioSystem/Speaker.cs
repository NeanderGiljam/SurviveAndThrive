using UnityEngine;
using System.Collections;

namespace FNAAS {
	public class Speaker : MonoBehaviour {

		public int minNonPlayLenght = 2;
		public int maxNonPlayLenght = 4;

		[SortedEnum]
		public AudioType audioType;
		public AudioClip[] audioClips;

		private int currentPlayIndex = -1;
		private int minPatternLenght = 9;
		private int maxPatternLenght = 18;

		private bool isInit;
		private AudioSource audioSource;

		private bool[] pattern;

		private void Start() {
			audioSource = GetComponent<AudioSource>();

			GameAccesPoint.Instance.mainGameState._ambientAudioController.AddSpeaker(this, transform.position);

			if (audioSource != null)
				isInit = true;
		}

		public void PlayNext() {
			if (!gameObject.activeSelf)
				return;

			if (pattern[currentPlayIndex] && audioClips.Length > 0) {
				SetRandomSound();
				StartAudio(Random.Range(0.35f, 0.76f));
			}
		}

		public bool WantsPlayNext() {
			currentPlayIndex++;

			if (currentPlayIndex >= pattern.Length)
				currentPlayIndex = 0;
			
			return pattern[currentPlayIndex];
		}

		public void SetPattern() {
			pattern = new bool[Random.Range(minPatternLenght, maxPatternLenght + 1)];
			int nons = minNonPlayLenght;

			for (int i = 0; i < pattern.Length; i++) {
				if (i == nons) {
					pattern[i] = true;
					nons = i + Random.Range(minNonPlayLenght, maxNonPlayLenght + 1) + 1;
				} else {
					pattern[i] = false;
				}
			}
		}

		private void SetRandomSound() {
			int clipIndex = Random.Range(0, audioClips.Length - 1);
			audioSource.clip = audioClips[clipIndex];
		}

		// --------------- Play Functions --------------- //
		#region Play Functions

		public void StartAudio(float delay = 0) {
			if (!isInit || !this.isActiveAndEnabled)
				return;

			//Debug.Log("Play " + audioType + " sound.");

			audioSource.PlayDelayed(delay);
		}

		public void StopAudio() {
			if (!isInit)
				return;

			audioSource.Stop();
		}

		public void PauseAudio() {
			if (!isInit)
				return;

			audioSource.Pause();
		}

		public void ResumeAudio() {
			if (!isInit)
				return;

			audioSource.UnPause();
		}

		#endregion
		// --------------- Play Functions --------------- //
	}
}