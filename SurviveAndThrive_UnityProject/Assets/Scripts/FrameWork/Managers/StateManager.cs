using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void OnStateChangeHandler();
public delegate void OnGuiStateChangeHandler(GuiState currentGuiState);

public class StateManager : BaseManager {
    public event OnStateChangeHandler OnStateChange;
    public event OnGuiStateChangeHandler OnGuiStateChange;

    // --------------- Singleton Pattern ---------------
    private static StateManager instance = null;

    public static StateManager Instance { 
        get {
            if (instance == null) {
                instance = Object.FindObjectOfType(typeof(StateManager)) as StateManager;

                if (instance == null) {
                    GameObject go = new GameObject("Managers");
                    DontDestroyOnLoad(go);
                    instance = go.AddComponent<StateManager>();
                }
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }
    // --------------- Singleton Pattern ---------------

    [HideInInspector, SerializeField]
    public string selectedScene;

    static public bool IsActive() { return instance != null; }
    private bool isEnding = false;

    public List<GuiState> guiStates = new List<GuiState>();

	public GuiState newGUIState;
    public GuiState currentGuiState;
	public GuiState previousGUIState;

    public SceneState newSceneState;
    private SceneState currentGameState;

    public override void Initialize() {
        GameAccesPoint.Instance.managerSystem.stateManager = this;
    }

    public void StopGameSession(float waitTime) {
        if (!isEnding) {
            isEnding = true;
            GameAccesPoint.Instance.mainGameState._gameSpeedController.Stop();
            StartCoroutine(WaitForSessionEnd(waitTime));
        }
    }

    public void RestartSession() {
        SetGameState<MainMenuState>();
        SetGameState<MainGameState>();
    }

    /// <summary>
    /// Use this: SetGameState(Object.FindObjectOfType&lt;GameState&gt;());
    /// </summary>
    /// <param name="newSceneState"></param>
    public void SetGameState<T>() where T : SceneState {
        newSceneState = FindObjectOfType<T>();
		
        if (newSceneState == null) {
            Debug.LogError("The specified gamestate equals null!");
            return;
        }

        if (currentGameState == newSceneState) {
            Debug.LogWarning("You are trying to start the current gamestate again, aborted!");
            return;
        }

        if (currentGameState != null) {
            currentGameState.Disable();
        }

        this.currentGameState = newSceneState;

        if (OnStateChange != null) {
            OnStateChange();
        }

		//Application.LoadLevel(currentGameState.selectedScene); // Deprecation fix 

		StartCoroutine(WaitForLevelToLoad());
    }

	/// <summary>
	/// Use this: SetGuiState(selectedGUIState); or SetGuiState(GuiHelper.FindOrCreateGuiState&lt;GuiState&gt;());
	/// </summary>
	/// <param name="newGuiState"></param>
	public void SetGuiState(GuiState newGuiState) {
		previousGUIState = currentGuiState;

		if (newGuiState == null) {
			Debug.LogError("The specified guistate equals null!");
			return;
		}

		if (currentGuiState == newGuiState) {
			Debug.LogWarning("You are trying to start the current guistate again, aborted!");
			return;
		}

		int i = guiStates.Count;
		while (i-- > 0) {
			guiStates[i].SetActiveState(false);
		}

		//foreach (GuiState guiState in guiStates) {
		//	guiState.SetActiveState(false);
		//}

		currentGuiState = newGuiState;

		currentGuiState.SetActiveState(true);

		if (OnGuiStateChange != null) {
			OnGuiStateChange(currentGuiState);
		}
	}

	public void SetGuiState<T>() where T : GuiState {
		newGUIState = SystemHelper.FindOrCreateGuiState<T>();
		previousGUIState = currentGuiState;

		if (newGUIState == null) {
			Debug.LogError("The specified guistate equals null!");
			return;
		}

		if (currentGuiState == newGUIState) {
			Debug.LogWarning("You are trying to start the current guistate again, aborted!");
			return;
		}

		int i = guiStates.Count;
		while (i-- > 0) {
			guiStates[i].SetActiveState(false);
		}

		//foreach (GuiState guiState in guiStates) {
		//	guiState.SetActiveState(false);
		//}

		currentGuiState = newGUIState;

		currentGuiState.SetActiveState(true);

		if (OnGuiStateChange != null) {
			OnGuiStateChange(currentGuiState);
		}
	}

	public T GetGUIState<T>() where T : GuiState {
		foreach (GuiState s in guiStates) {
			if (s.GetType() == typeof(T)) {
				return s as T;
			}
		}

		return null;
	}

	private IEnumerator WaitForLevelToLoad() {
		/* -- Deprecation fix --
        while (Application.isLoadingLevel)
            yield return 1;
		*/
		AsyncOperation loadLevel = Application.LoadLevelAsync(currentGameState.selectedScene);

		yield return loadLevel;

		currentGameState.Enable();
        currentGameState.SetGuiState();
    }

    private IEnumerator WaitForSessionEnd(float time) {
        yield return new WaitForSeconds(time);
        SetGameState<MainMenuState>();
        isEnding = false;
    }
}
