using UnityEngine;
using System.Collections;

public class MainMenuState : SceneState {

    protected override void Awake() {
        base.Awake();

        GameAccesPoint.Instance.menuState = this;

        if (selectedGUIState == null) {
            selectedGUIState = SystemHelper.FindOrCreateGuiState<MainMenuGUIState>();
        }
    }

    public override void SetGuiState() {
		base.SetGuiState();

		stateManager.SetGuiState(selectedGUIState);
        inputManager.SetControlScheme<BaseMenuNavigationControlScheme>();
    }

    protected override void HandleOnStateChange() {
 	    base.HandleOnStateChange();
    }
}
