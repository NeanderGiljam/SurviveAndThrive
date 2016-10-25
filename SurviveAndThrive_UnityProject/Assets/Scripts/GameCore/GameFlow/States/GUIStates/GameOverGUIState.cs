using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class GameOverGUIState : GuiState {

	private GameObject gameOverScreen;

	public override void Initialize() {
		base.Initialize();

		GameAccesPoint.Instance.gameOverGUIState = this;
		GameAccesPoint.Instance.managerSystem.stateManager.guiStates.Add(this);

		// --------------- Root Setup ---------------
		gameOverScreen = gameObject;
		gameOverScreen.transform.SetParent(uiRoot);
		gameOverScreen.transform.position = uiRoot.position;
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
		stateManager.SetGameState<MainMenuState>();
		RemoveState();
	}

	public override void SetActiveState(bool state) {
		base.SetActiveState(state);

		gameOverScreen.SetActive(state);
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

	public override void RemoveState() {
		GameAccesPoint.Instance.gameOverGUIState = null;
		GameAccesPoint.Instance.managerSystem.stateManager.guiStates.Remove(this);

		Destroy(gameOverScreen);
	}

}