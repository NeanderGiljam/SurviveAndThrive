using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MainMenuGUIState : GuiState {

	private GameObject uiMenu;

    public override void Initialize() {
        base.Initialize();

        GameAccesPoint.Instance.mainMenuGUIState = this;
        GameAccesPoint.Instance.managerSystem.stateManager.guiStates.Add(this);

        // --------------- Root Setup ---------------
        uiMenu = gameObject;
        uiMenu.transform.SetParent(uiRoot);
        uiMenu.transform.position = uiRoot.position;
		// --------------- Root Setup ---------------

		// Use this to add a function to a button
		buttons = new Button[5];
		clickedActions = new UnityAction[] {
			() => OnNewGameClicked(),
			() => OnResumeGameClicked(),
			() => OnCreditsClicked(),
			() => OnSettingsClicked(),
			() => OnExitClicked()
		};
		buttonText = new string[] {
			"new game",
			"load game",
			"credits",
			"settings",
			"quit"
		};

		_Cursor = uiMenu.transform.FindChild("r_Cursor").transform;

		Transform buttonGroup = uiMenu.transform.FindChild("r_ButtonGroup");
		if (buttonGroup == null) {
			Debug.LogError("Could not find the button group, stopping!");
			return;
		}

		for (int i = 0; i < buttons.Length; i++) {
			buttons[i] = buttonGroup.FindChild("r_Button" + i).GetComponent<Button>();
			if (buttons[i] != null) {
				buttons[i].onClick.AddListener(clickedActions[i]);
				buttons[i].GetComponentInChildren<Text>().text = buttonText[i]; // TODO: Build in safety
			}
		}

		isInit = true;
    }

	private void OnNewGameClicked() {
		//audioController.PlayGenericButtonAudio();
		GameAccesPoint.Instance.mainGameState.SetGameSettings(0, true);
		AudioManager.Instance.PlayByName("start_08", 1, false, 0, true);
		stateManager.SetGameState<MainGameState>();
	}

	private void OnResumeGameClicked() {
		//audioController.PlayGenericButtonAudio();
		GameAccesPoint.Instance.mainGameState.SetGameSettings(1, false); // TODO: Move to the selection screen when its there
		stateManager.SetGameState<MainGameState>();
	}

	private void OnCreditsClicked() {
		//audioController.PlayGenericButtonAudio();
		stateManager.SetGuiState<CreditsGUIState>();
	}

	private void OnSettingsClicked() {
		//audioController.PlayGenericButtonAudio();
		stateManager.SetGuiState<SettingsGUIState>();
	}

	private void OnExitClicked() {
		//audioController.PlayGenericButtonAudio();
		Application.Quit();
	}

	public override void SetButtonsDictionary(GuiState currentGuiState) {
        base.SetButtonsDictionary(currentGuiState);

        menuButtons.Clear();

		for (int i = 0; i < buttons.Length; i++) {
			menuButtons.Add(i, new ButtonActionPair(buttons[i], clickedActions[i]));
        }
    }

    public override void SetActiveState(bool state) {
        base.SetActiveState(state);

        uiMenu.SetActive(state);
    }

    public override void SetControlScheme() {
        base.SetControlScheme();

        inputManager.SetControlScheme<BaseMenuNavigationControlScheme>();
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