using UnityEngine;
using System.Collections.Generic;

public enum InputStates {
    None,
    Up,
    Down
}

public class BaseMenuNavigationControlScheme : BaseControlScheme {

    // --------------- Tweakables ---------------

    private float inputDelay = 0.2f;

	// --------------- Tweakables ---------------

	private float timeStamp = 0;

    private InputStates current, previous;

	private List<int> keys;
	private Vector3 previousMousePos;

	public override void Initialize() {
        base.Initialize();

		userActions = GameAccesPoint.Instance.managerSystem.inputManager._userActions;
		previousMousePos = Input.mousePosition;

		isInit = true;
    }

	// TODO: Refactor this mess, make keyboard functions the same as mouse and controller
    public override void UpdateControls() {
 	    base.UpdateControls();

        if (isInit && stateManager.currentGuiState != null) {
            previous = current;

            if (userActions.moveDown) {
                current = InputStates.Up;
            } else if (userActions.moveUp) {
                current = InputStates.Down;
            } else {
                current = InputStates.None;
            }

            timeStamp += Time.deltaTime;

            if (current == InputStates.Up && previous != InputStates.Down) { //  && buttonIndex < stateManager.currentGuiState.menuButtons.Count - 1
				if (timeStamp > inputDelay) {
                    buttonIndex++;
					if (buttonIndex > stateManager.currentGuiState.menuButtons.Count - 1) {
						buttonIndex = 0;
					}
                    timeStamp = 0;
                }
            } else if (current == InputStates.Down && previous != InputStates.Up) { // && buttonIndex > 0
				if (timeStamp > inputDelay) {
                    buttonIndex--;
					if (buttonIndex < 0) {
						buttonIndex = stateManager.currentGuiState.menuButtons.Count - 1;
					}
					timeStamp = 0;
                }
            }

            if (stateManager.currentGuiState.menuButtons != null && stateManager.currentGuiState.menuButtons.Count > 0) {
				if (firstUpdate) {
					keys = new List<int>(stateManager.currentGuiState.menuButtons.Keys);
				}

                for (int i = 0; i < keys.Count; i++) {
                    if (keys[i] == buttonIndex) {
						if (firstUpdate) {
							stateManager.currentGuiState._eventSystem.SetSelectedGameObject(null); // Do this because does not detect as deselect on set active false
                            stateManager.currentGuiState.menuButtons[i].button.Select();
							firstUpdate = false;
						}

						if (Input.mousePosition != previousMousePos) {
							if (userActions.LastInputType != InControl.BindingSourceType.DeviceBindingSource && userActions.LastInputType != InControl.BindingSourceType.KeyBindingSource) {
								stateManager.currentGuiState._eventSystem.SetSelectedGameObject(null);
							} else {
								userActions.LastInputType = InControl.BindingSourceType.None;
								previousMousePos = Input.mousePosition;
							}
						} else {
							stateManager.currentGuiState.menuButtons[i].button.Select();
						}

						if (stateManager.currentGuiState._Cursor != null) {
                            if (stateManager.currentGuiState._eventSystem.currentSelectedGameObject != null) {
								stateManager.currentGuiState._Cursor.position = stateManager.currentGuiState._eventSystem.currentSelectedGameObject.transform.position;
							} else {
								if (stateManager.currentGuiState._hoveredObject != null) {
									stateManager.currentGuiState._Cursor.position = stateManager.currentGuiState._hoveredObject.transform.position;
								} else {
									stateManager.currentGuiState._Cursor.position = new Vector3(0, 5000, 0); // TODO: Change to on/off if causing issues
                                }
                            }
						}

						PlayMenuAudio();

						if (userActions.harvest.WasReleased && stateManager.currentGuiState._eventSystem.currentSelectedGameObject != null) {
							AudioManager.Instance.PlayByName("select_09");
                            stateManager.currentGuiState.menuButtons[i].action.Invoke();
                        }
                    }
                }
            }
        }
    }

	private int prevButtonIndex;
	private GameObject prevHoveredObj;

	private void PlayMenuAudio() {
		//Debug.Log("Prev Button idx: " + prevButtonIndex + " Button idx: " + buttonIndex);

		if (stateManager.currentGuiState._hoveredObject != prevHoveredObj && stateManager.currentGuiState._hoveredObject != null) {
			AudioManager.Instance.PlayByName("down_03");
		} else {
			if (prevButtonIndex > buttonIndex)
				AudioManager.Instance.PlayByName("down_03");
			else if (prevButtonIndex < buttonIndex)
				AudioManager.Instance.PlayByName("up_06");
		}

		prevButtonIndex = buttonIndex;
		prevHoveredObj = stateManager.currentGuiState._hoveredObject;
    }	
}