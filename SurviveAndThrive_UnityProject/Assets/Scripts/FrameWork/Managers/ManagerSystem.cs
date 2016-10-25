using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void ManagersInitialized();

public class ManagerSystem : MonoBehaviour {
    public event ManagersInitialized OnManagersInit;

    public StateManager stateManager;
	public SettingsManager settingsManager;
	public AudioManager audioManager;
    public InputManager inputManager;
	public ImageEffectManager imageEffectManager;
	public SaveManager saveManager;

	private MonoBehaviour[] pausedMonoBehaviours;
    private List<BaseManager> managers = new List<BaseManager>();

	//private GUIStyle skin = new GUIStyle();

	public void InitManagers() {
        GameAccesPoint.Instance.managerSystem = this;

        managers.Clear();

        managers.Add(FindObjectOfType<StateManager>());
		managers.Add(FindObjectOfType<SettingsManager>());
        managers.Add(FindObjectOfType<AudioManager>());
        managers.Add(FindObjectOfType<InputManager>());
		managers.Add(FindObjectOfType<ImageEffectManager>());

        if (managers == null || managers.Count <= 0) {
            Debug.LogError("No managers found abort!");
            return;
        }

        foreach (BaseManager manager in managers) {
            manager.Initialize();
        }

        if (OnManagersInit != null) {
            OnManagersInit();
        }
    }

	private void Update() {
		if (Input.GetKeyDown(KeyCode.F3)) { // TODO: Move this code
			DebugHelper.ToggleDebugMode();
			if (DebugHelper.debugMode == true) {
				GameAccesPoint.Instance.managerSystem.stateManager.SetGuiState<DebugGUIState>();
				GameAccesPoint.Instance.managerSystem.Pause();
			} else {
				GameAccesPoint.Instance.managerSystem.stateManager.SetGuiState<HUDGUIState>();
				GameAccesPoint.Instance.managerSystem.Resume();
			}
		}
	}

    private void OnGUI() {
		GUI.skin.label.alignment = TextAnchor.MiddleLeft;
		GUI.Label(new Rect(5, Screen.height - 25, Screen.width, 30), "Pre-Alpha build, v0.0.0.9 \u00A9 2015, Four Nomads."); // Pre-Alpha, Alpha, (Open-)Beta, Release
		GUI.skin.label.alignment = TextAnchor.MiddleRight;
		GUI.Label(new Rect(-5, Screen.height - 25, Screen.width, 30), "All assets are subject to change."); // This build represents a work in progress.
	}

    public void Pause() {
        TogglePause(true);
    }

    public void Resume() {
        TogglePause(false);
    }

    public void ShutDown() {
        throw new System.NotImplementedException();
    }

    private void TogglePause(bool pause) {
        if (pause || pausedMonoBehaviours == null) {
            pausedMonoBehaviours = FindObjectsOfType<MonoBehaviour>();
        }

        for (int i = 0; i < pausedMonoBehaviours.Length; i++) {
            if (pausedMonoBehaviours[i] == null) {
                continue;
            }

            if (pausedMonoBehaviours[i] is IPausable) {
                try {
                    if (pause) {
                        ((IPausable)pausedMonoBehaviours[i]).Pause();
                    } else {
                        ((IPausable)pausedMonoBehaviours[i]).Resume();
                    }
                } catch (System.Exception ex) {
                    Debug.Log("Exception caught when pausing/resuming, exception: " + ex.ToString(), pausedMonoBehaviours[i]);
                }
            }
        }
        //isPaused = pause;
    }
}
