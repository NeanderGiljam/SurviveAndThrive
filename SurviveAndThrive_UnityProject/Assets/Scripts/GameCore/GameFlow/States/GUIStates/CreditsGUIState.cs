using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CreditsGUIState : GuiState {

	private GameObject creditsScreen;

	public override void Initialize() {
		base.Initialize();

		GameAccesPoint.Instance.creditsGUIState = this;
		GameAccesPoint.Instance.managerSystem.stateManager.guiStates.Add(this);

		// --------------- Root Setup ---------------
		creditsScreen = gameObject;
		creditsScreen.transform.SetParent(uiRoot);
		creditsScreen.transform.position = uiRoot.position;
		// --------------- Root Setup ---------------

		// Use this to add a function to a button
		buttons = new Button[1];
		clickedActions = new UnityAction[] {
			() => OnClicked()
		};

		for (int i = 0; i < buttons.Length; i++) {
			buttons[i] = transform.GetComponent<Button>();
			if (buttons[i] != null) {
				buttons[i].onClick.AddListener(clickedActions[i]);
			}
		}

		isInit = true;
	}

	private void OnClicked() {
		stateManager.SetGuiState(stateManager.previousGUIState);
	}

	public override void SetActiveState(bool state) {
		base.SetActiveState(state);

		creditsScreen.SetActive(state);
	}

	public override void SetControlScheme() {
		base.SetControlScheme();

		inputManager.SetControlScheme<BaseMenuNavigationControlScheme>();
	}

	public override void SetButtonsDictionary(GuiState currentGuiState) {
		base.SetButtonsDictionary(currentGuiState);

		menuButtons.Clear();

		for (int i = 0; i < buttons.Length; i++) {
			menuButtons.Add(i, new ButtonActionPair(buttons[i], clickedActions[i]));
		}
	}
}