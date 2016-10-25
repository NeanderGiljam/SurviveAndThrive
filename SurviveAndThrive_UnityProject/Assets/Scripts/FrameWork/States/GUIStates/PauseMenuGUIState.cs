using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PauseMenuGUIState : GuiState {
    
    private GameObject pauseMenuGUI;

	public override void Initialize() {
        base.Initialize();

        GameAccesPoint.Instance.pauseMenuGUIState = this;
        GameAccesPoint.Instance.managerSystem.stateManager.guiStates.Add(this);

        // --------------- Root Setup ---------------
        pauseMenuGUI = gameObject;
        pauseMenuGUI.transform.SetParent(uiRoot);
        pauseMenuGUI.transform.position = uiRoot.position;
		// --------------- Root Setup ---------------

		// Use this to add a function to a button
		buttons = new Button[3];
		clickedActions = new UnityAction[] {
			() => OnResumeClicked(),
			() => OnSettingsClicked(),
			() => OnExitClicked()
		};
		buttonText = new string[] {
			"resume game",
			"settings",
			"exit"
		};

		_Cursor = pauseMenuGUI.transform.FindChild("r_Cursor").transform;

		for (int i = 0; i < buttons.Length; i++) {
			buttons[i] = pauseMenuGUI.transform.FindChild("r_Button" + i).GetComponent<Button>();
			if (buttons[i] != null) {
				buttons[i].onClick.AddListener(clickedActions[i]);
				buttons[i].GetComponentInChildren<Text>().text = buttonText[i]; // TODO: Build in safety
			}
		}

        isInit = true;
    }

	private void OnResumeClicked() {
		//audioController.PlayGenericButtonAudio();
		GameAccesPoint.Instance.managerSystem.Resume();
		stateManager.SetGuiState<HUDGUIState>();
	}

	private void OnSettingsClicked() {
		//audioController.PlayGenericButtonAudio();
		stateManager.SetGuiState<SettingsGUIState>();
	}

	private void OnExitClicked() {
		GameAccesPoint.Instance.managerSystem.Resume(); // TODO: Handle better from the manager
		GameAccesPoint.Instance.mainGameState._gameSpeedController.Stop();
		//audioController.PlayGenericButtonAudio();
		stateManager.SetGameState<MainMenuState>();
	}

	public override void SetButtonsDictionary(GuiState currentGuiState) {
        base.SetButtonsDictionary(currentGuiState);

        menuButtons.Clear();

		for (int i = 0; i < buttons.Length; i++) {
			menuButtons.Add(i, new ButtonActionPair(buttons[i], clickedActions[i]));
		}
	}

    public override void SetControlScheme() {
        base.SetControlScheme();

        inputManager.SetControlScheme<BaseMenuNavigationControlScheme>();
		// TODO: Mute/Pause audio on pauze start -> Unmute/Unpause on resume
    }

	public override void SetActiveState(bool state) {
		base.SetActiveState(state);

		if (state) {
			GameAccesPoint.Instance.managerSystem.imageEffectManager.FadeScreen(Direction.Out, 2f);
		} else {
			GameAccesPoint.Instance.managerSystem.imageEffectManager.FadeScreen(Direction.In, 2f);
		}

        pauseMenuGUI.SetActive(state);
    }

	public void OnPointerEnter(object hoveredButton) {
		if (hoveredButton is GameObject) {
			_hoveredObject = (GameObject)hoveredButton;
		}
	}

	public void OnPointerExit(object eventData) {
		_hoveredObject = null;
	}
}
