using UnityEngine;
using UnityEngine.UI;

// TODO: Make all settings adjustable with controller
public class SettingsGUIState : GuiState {
	
	private GameObject SettingsMenu;

	private Toggle pixelateToggle;
	private Toggle showGUIToggle;

	private Slider masterVolumeSlider;
	private Text masterVolumeLabel;

	private GameSettings currentSettings;

	public override void Initialize() {
		base.Initialize();

		GameAccesPoint.Instance.settingsGUIState = this;
		GameAccesPoint.Instance.managerSystem.stateManager.guiStates.Add(this);

		// --------------- Root Setup ---------------
		SettingsMenu = gameObject;
		SettingsMenu.transform.SetParent(uiRoot);
		SettingsMenu.transform.position = uiRoot.position;
		// --------------- Root Setup ---------------

		transform.FindChild("r_Button_Back").GetComponent<Button>().onClick.AddListener(OnBackClicked);
		transform.FindChild("r_Button_Save").GetComponent<Button>().onClick.AddListener(OnSaveClicked);

		pixelateToggle = transform.FindChild("r_Toggle_Pixelate").GetComponent<Toggle>();
		showGUIToggle = transform.FindChild("r_Toggle_ShowGUI").GetComponent<Toggle>();

		Transform masterVolumeSliderObj = transform.FindChild("r_Slider_MasterVolume");
        masterVolumeSlider = masterVolumeSliderObj.GetComponent<Slider>();
		masterVolumeSlider.onValueChanged.AddListener(OnSliderChanged);
		masterVolumeLabel = masterVolumeSliderObj.FindChild("r_Label_Value").GetComponent<Text>();

		currentSettings = SettingsManager.Instance.currentSettings;
		if (currentSettings != null)
			ApplyDefaultSettings();

		isInit = true;
	}

	private void OnSaveClicked() {
		currentSettings.pixelate = pixelateToggle.isOn;
		currentSettings.alwaysShowGUI = showGUIToggle.isOn;
		currentSettings.masterVolume = masterVolumeSlider.value / 100f;

		SettingsManager.Instance.UpdateSettings(currentSettings);
	}

	private void OnBackClicked() {
		stateManager.SetGuiState(stateManager.previousGUIState);
	}

	private void OnSliderChanged(float newValue) {
		masterVolumeLabel.text = newValue.ToString();
	}

	private void ApplyDefaultSettings() {
		pixelateToggle.isOn = currentSettings.pixelate;
		showGUIToggle.isOn = currentSettings.alwaysShowGUI;
		masterVolumeSlider.value = currentSettings.masterVolume * 100;
		masterVolumeLabel.text = masterVolumeSlider.value.ToString();
    }

	public override void SetButtonsDictionary(GuiState currentGuiState) {
		base.SetButtonsDictionary(currentGuiState);

		ApplyDefaultSettings();

		//menuButtons.Clear();

		//for (int i = 0; i < buttons.Length; i++) {
		//	menuButtons.Add(i, new ButtonActionPair(buttons[i], clickedActions[i]));
		//}
	}

	public override void SetActiveState(bool state) {
		base.SetActiveState(state);

		SettingsMenu.SetActive(state);
	}

	public override void SetControlScheme() {
		base.SetControlScheme();

		inputManager.SetControlScheme<BaseMenuNavigationControlScheme>();
	}
}