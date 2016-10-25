using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : BaseManager, IGameSettingsListener {

    // --------------- Singleton Pattern ---------------
	private static AudioManager instance;

	public static AudioManager Instance {
		get {
            instance = FindObjectOfType<AudioManager>();

			if (instance == null) {
				instance = new GameObject("AudioManager").AddComponent<AudioManager>();
			}
			return instance;
		}
	}
	// --------------- Singleton Pattern ---------------

	public List<AudioClipData> activeAudio = new List<AudioClipData>();
	public List<AudioClip> audioClips;

	private float masterVolume = 1f;
	private string audioPath = "Audio";

	// --------------- Initialization  ---------------
	#region Initialization

	public override void Initialize() {
        GameAccesPoint.Instance.managerSystem.audioManager = this;

        audioClips = GetAllAudioClips();

        GameAccesPoint.Instance.managerSystem.stateManager.OnStateChange += OnStateChange;

		// --------------- Game Settings Controlled Options ---------------
		masterVolume = SettingsManager.Instance.currentSettings.masterVolume;
		SettingsManager.Instance.OnSettingsChanged += OnGameSettingsChanged;
		// --------------- Game Settings Controlled Options ---------------
	}

	#endregion
	// --------------- Initialization ---------------

	private void Update() {
        if (activeAudio == null || activeAudio.Count <= 0) {
            return;
        }

        int i = activeAudio.Count;

        while (--i > -1) {
            activeAudio[i].UpdateAudioClip();
        }
    }

    // --------------- Play Functions ---------------
    #region Play Functions

    public AudioClipData PlayByName(string audioName, float volume = 1f, bool loop = false, float fadeSpeed = 0f, bool dontDestroyOnLoad = false) {
        if (volume <= 0f) {
            return null;
        }
        
        AudioClip clip = GetAudioClipByName(audioName);

        if (clip == null) {
            Debug.LogWarning("Clip is null!");
            return null;
        }

        GameObject newGO = new GameObject("audio");

		if(dontDestroyOnLoad)
			DontDestroyOnLoad(newGO);

        AudioClipData newData = CreateNewClipData(newGO, clip, volume * masterVolume, loop, fadeSpeed);
        activeAudio.Add(newData);

        if (fadeSpeed > 0) {
            newData.SetVolume(0.01f);
            newData.SetTargetVolume(volume * masterVolume, fadeSpeed);
        } else {
            newData.SetVolume(volume * masterVolume);
        }

        newData.audioSource.Play();

        return newData;
    }

    private AudioClipData CreateNewClipData(GameObject parent, AudioClip clip, float volume, bool loop, float fadeSpeed) {
        parent.transform.SetParent(transform);

        AudioSource audioSource = parent.AddComponent<AudioSource>();

        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.loop = loop;

        AudioClipData newClipData = new AudioClipData();
        newClipData.audioId = parent.GetInstanceID();
        newClipData.initialVolume = volume;
        newClipData.fadeSpeed = fadeSpeed;
        newClipData.audioName = clip.name;
        newClipData.parent = parent;
        newClipData.audioSource = audioSource;
        newClipData.audioManager = this;

        parent.name += " : " + newClipData.audioId;

        return newClipData;
    }

    public void PauseByName(string audioName) {
        if (activeAudio == null || activeAudio.Count <= 0) {
            Debug.LogWarning("The audio clip could not be paused because it is not playing");
            return;
        }

        foreach (AudioClipData clipData in activeAudio) {
            if (clipData.audioName == audioName) {
                clipData.audioSource.Pause();
            }
        }
    }

    public void ResumeByName(string audioName) {
        if (activeAudio == null || activeAudio.Count <= 0) {
            Debug.LogWarning("The audio clip could not be paused because it is not playing");
            return;
        }

        foreach (AudioClipData clipData in activeAudio) {
            if (clipData.audioName == audioName) {
                clipData.audioSource.UnPause();
            }
        }
    }

    public void StopByName(string audioName, float fadeSpeed = 0f) {
        int i = activeAudio.Count;

        while (--i > -1) {
            if (activeAudio[i].audioName == audioName) {
                activeAudio[i].SetTargetVolume(0, fadeSpeed);
            }
        }
    } 

    #endregion
    // --------------- Play Functions ---------------


    // --------------- Get Functions ---------------
    #region Get Functions

    private AudioClip GetAudioClipByName(string audioName) {
        AudioClip audioClip = null;

        foreach (AudioClip clip in audioClips) {
            if (clip.name == audioName) {
                audioClip = clip;
            }
        }

        if (audioClip == null) {
            Debug.LogWarning("Could not find audioclip with the specified name!");
        }

        return audioClip;
    }

    private List<AudioClip> GetAllAudioClips() {
        List<AudioClip> audioClips = new List<AudioClip>();

        AudioClip[] audioArray = (AudioClip[])Resources.LoadAll<AudioClip>(audioPath);

        if (audioArray != null) {
            for (int i = 0; i < audioArray.Length; i++) {
                audioClips.Add(audioArray[i]);
            }
        } else {
            return null;
        }

        return audioClips;
    }

    private void RemoveClipData(string audioName) {
        for (int i = 0; i < activeAudio.Count; i++) {
            if (activeAudio[i].audioName == audioName) {
                activeAudio.RemoveAt(i);
            }
        }
    }

    #endregion
    // --------------- Get Functions ---------------

    public override void Pause() {
        base.Pause();

        foreach (AudioClipData data in activeAudio) {
            PauseByName(data.audioName);
        }
    }

    public override void Resume() {
        base.Resume();
        foreach (AudioClipData data in activeAudio) {
            ResumeByName(data.audioName);
        }
    }

    private void OnStateChange() {
        int i = activeAudio.Count;

        while (--i > -1) {
            StopByName(activeAudio[i].audioName);
        }
    }

	public void OnGameSettingsChanged(GameSettings newSettings) {
		masterVolume = newSettings.masterVolume;
		// TODO: Change volume on all playing audio
	}
}