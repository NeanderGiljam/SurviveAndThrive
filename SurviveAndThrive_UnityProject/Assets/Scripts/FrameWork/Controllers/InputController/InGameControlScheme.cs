using UnityEngine;

public class InGameControlScheme : BaseControlScheme {

    public override void UpdateControls() {
        base.UpdateControls();

        ControllerNonGameplayRelatedInput();
    }

    public override void SetActiveState(bool state) {
        if (state) {
            isActive = true;
        } else {
            isActive = false;
        }
    }

	private void ControllerNonGameplayRelatedInput() {
		if (GameAccesPoint.Instance.managerSystem.inputManager.IsPaused) {
			return;
		}

        if (userActions.pause) {
            GameAccesPoint.Instance.managerSystem.Pause();
            GameAccesPoint.Instance.managerSystem.stateManager.SetGuiState<PauseMenuGUIState>();
        }
    }
}