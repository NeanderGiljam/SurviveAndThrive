using InControl;
using UnityEngine;
using System.Collections.Generic;

public class InputManager : BaseManager {

	public bool _isGamepad { get; private set; }

	// --------------- Singleton Pattern ---------------
	private static InputManager instance;

	public static InputManager Instance {
		get {
			instance = FindObjectOfType<InputManager>();

			if (instance == null) {
				instance = new GameObject("InputManager").AddComponent<InputManager>();
			}
			return instance;
		}
	}
	// --------------- Singleton Pattern ---------------

	public CustomUserActions _userActions { get; private set; }

	public InputType inputType = InputType.Keyboard;

    public BaseControlScheme activeControlScheme;
    public List<BaseControlScheme> allControlSchemes = new List<BaseControlScheme>();

    private bool isInited = false;

    public override void Initialize() {
        GameAccesPoint.Instance.managerSystem.inputManager = this;

		SetCustomUserActions();

		allControlSchemes.Add(new InGameControlScheme());
        allControlSchemes.Add(new BaseMenuNavigationControlScheme());

        foreach (BaseControlScheme scheme in allControlSchemes) {
            scheme.SetActiveState(false);
            scheme.Initialize();
        }

        activeControlScheme = SetControlScheme<BaseMenuNavigationControlScheme>();
		_isGamepad = false;

		isInited = true;
    }

    private void Update() {
        if (isInited) {
            if (activeControlScheme != null) {
                activeControlScheme.UpdateControls();
            } else {
                Debug.LogError("The active controlscheme does not exist");
            }

			CheckIfGamepad();
        }
    }

	Vector3 oldMousPos = Vector3.zero;

	private void CheckIfGamepad() {
		InputDevice inputDevice = InControl.InputManager.ActiveDevice;
		Vector3 currentMousePos = Input.mousePosition;

		if (Input.anyKey || currentMousePos != oldMousPos) {
			_isGamepad = false;
		}

		if (inputDevice.LeftStickX != 0 || inputDevice.LeftStickY != 0 || inputDevice.RightStickX != 0 || inputDevice.RightStickY != 0 || inputDevice.Action1 || inputDevice.Action2 || inputDevice.Action3 || inputDevice.Action4) {
			_isGamepad = true;
		}

		oldMousPos = currentMousePos;
	}

    public T SetControlScheme<T>() where T : BaseControlScheme {
        activeControlScheme = allControlSchemes.Find(item => item.GetType() == typeof(T));
        activeControlScheme.SetActiveState(true);
        activeControlScheme.buttonIndex = 0;
		activeControlScheme.firstUpdate = true;
        return (T)System.Convert.ChangeType(activeControlScheme, typeof(T));
    }

	private void SetCustomUserActions() {
		_userActions = new CustomUserActions();

		_userActions.RotateCameraLeft.AddDefaultBinding(Key.Q);
		_userActions.RotateCameraLeft.AddDefaultBinding(InputControlType.LeftBumper);

		_userActions.RotateCameraRight.AddDefaultBinding(Key.E);
		_userActions.RotateCameraRight.AddDefaultBinding(InputControlType.RightBumper);

		_userActions.moveUp.AddDefaultBinding(Key.W);
		_userActions.moveUp.AddDefaultBinding(Key.UpArrow);
		_userActions.moveUp.AddDefaultBinding(InputControlType.LeftStickUp);

		_userActions.moveDown.AddDefaultBinding(Key.S);
		_userActions.moveDown.AddDefaultBinding(Key.DownArrow);
		_userActions.moveDown.AddDefaultBinding(InputControlType.LeftStickDown);

		_userActions.moveLeft.AddDefaultBinding(Key.A);
		_userActions.moveLeft.AddDefaultBinding(Key.LeftArrow);
		_userActions.moveLeft.AddDefaultBinding(InputControlType.LeftStickLeft);

		_userActions.moveRight.AddDefaultBinding(Key.D);
		_userActions.moveRight.AddDefaultBinding(Key.RightArrow);
		_userActions.moveRight.AddDefaultBinding(InputControlType.LeftStickRight);

		_userActions.walk.AddDefaultBinding(Key.LeftShift);
		_userActions.walk.AddDefaultBinding(Key.RightShift);
		_userActions.walk.AddDefaultBinding(InputControlType.LeftTrigger);

		_userActions.harvest.AddDefaultBinding(Key.Space);
		_userActions.harvest.AddDefaultBinding(Key.Return);
		//_userActions.harvest.AddDefaultBinding(Mouse.LeftButton);
		_userActions.harvest.AddDefaultBinding(InputControlType.Action1);

		_userActions.use.AddDefaultBinding(Mouse.RightButton);
		_userActions.use.AddDefaultBinding(InputControlType.RightTrigger);

		_userActions.pause.AddDefaultBinding(Key.P);
		_userActions.pause.AddDefaultBinding(Key.Escape);
		_userActions.pause.AddDefaultBinding(InputControlType.Start);

		_userActions.rotateUp.AddDefaultBinding(Key.W);
		_userActions.rotateUp.AddDefaultBinding(Key.UpArrow);
		_userActions.rotateUp.AddDefaultBinding(InputControlType.RightStickUp);

		_userActions.rotateDown.AddDefaultBinding(Key.S);
		_userActions.rotateDown.AddDefaultBinding(Key.DownArrow);
		_userActions.rotateDown.AddDefaultBinding(InputControlType.RightStickDown);

		_userActions.rotateLeft.AddDefaultBinding(Key.A);
		_userActions.rotateLeft.AddDefaultBinding(Key.LeftArrow);
		_userActions.rotateLeft.AddDefaultBinding(InputControlType.RightStickLeft);

		_userActions.rotateRight.AddDefaultBinding(Key.D);
		_userActions.rotateRight.AddDefaultBinding(Key.RightArrow);
		_userActions.rotateRight.AddDefaultBinding(InputControlType.RightStickRight);
	}
}
