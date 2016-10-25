using UnityEngine;
using System.Collections.Generic;

public class GameAccesPoint : MonoBehaviour {

    // ---------- Singelton Pattern ----------
    private static GameAccesPoint _instance;

    public static GameAccesPoint Instance {
        get {
            if (_instance == null) {
                if (!Application.isPlaying)
                    return null;

                _instance = GameObject.FindObjectOfType<GameAccesPoint>();
                if (_instance == null) {
                    GameObject go = new GameObject("GameAccesPoint");
                    _instance = go.AddComponent<GameAccesPoint>();
                }
                //_instance.Initialize();
            }
            return _instance;
        }
    }
    // ---------- Singelton Pattern ----------

    // ---------- System Classes ----------
    public ManagerSystem managerSystem;
    public MainMenuState menuState;
    public MainGameState mainGameState;

    public MainMenuGUIState mainMenuGUIState;
    public HUDGUIState hudGUIState;
    public PauseMenuGUIState pauseMenuGUIState;
	public CreditsGUIState creditsGUIState;
	public DebugGUIState debugGUIState;
	public SettingsGUIState settingsGUIState;
	public GameOverGUIState gameOverGUIState;
	// ---------- System Classes ----------

	public AudioController audioController;
    public CameraController cameraController;

    private void Awake() {
        _instance = this;
        DontDestroyOnLoad(_instance);
    }

    private void OnDestroy() {
        _instance = null;
    }
}
