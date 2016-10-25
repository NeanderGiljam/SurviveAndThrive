using UnityEngine;

public delegate void OnSettingsChanged(GameSettings newGameSettings);

public class SettingsManager : BaseManager {

	public event OnSettingsChanged OnSettingsChanged;

	// --------------- Singleton Pattern ---------------
	private static SettingsManager instance;

	public static SettingsManager Instance {
		get {
			instance = FindObjectOfType<SettingsManager>();

			if (instance == null) {
				instance = new GameObject("OptionsManager").AddComponent<SettingsManager>();
			}
			return instance;
		}
	}
	// --------------- Singleton Pattern ---------------

	public GameSettings currentSettings { get; private set; }

	public override void Initialize() {
		GameAccesPoint.Instance.managerSystem.settingsManager = this;

		currentSettings = LoadSettings();
	}

	public void UpdateSettings(GameSettings newSettings) {
		currentSettings = newSettings;
		SaveSettings(currentSettings);

		if (OnSettingsChanged != null) {
			OnSettingsChanged(currentSettings);
		}
    }

	private GameSettings LoadSettings() {
		GameSettings gameSettings = XMLManager.XMLRead<GameSettings>("GameSettings", null);
		if (gameSettings == null) {
			Debug.LogWarning("No Settings found creating new file");
			gameSettings = new GameSettings();
			SaveSettings(gameSettings);
		}

		return gameSettings;
	}

	private void SaveSettings(GameSettings setting) {
		XMLManager.XMLWrite(setting, "GameSettings", null);
	}

}

[System.Serializable]
public class GameSettings {

	public bool pixelate = false;
	public bool alwaysShowGUI = false;
	public float masterVolume = 0.9f;

}