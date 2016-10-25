using UnityEngine;

public class BaseControlScheme {

    public int buttonIndex = 0;
	public bool firstUpdate = true;

	protected bool isActive = false;
    protected bool isInit = false;
    protected bool isPaused = false;

	protected StateManager stateManager;
	protected CustomUserActions userActions;

	public virtual void Initialize() {
        stateManager = GameAccesPoint.Instance.managerSystem.stateManager;
		userActions = GameAccesPoint.Instance.managerSystem.inputManager._userActions;
	}

    public virtual void UpdateControls() {
        if (!isActive) {
            return;
        }
    }

    public virtual void SetActiveState(bool state) {
        if (state) {
            isActive = true;
        } else {
            isActive = false;
        }
    }
}
