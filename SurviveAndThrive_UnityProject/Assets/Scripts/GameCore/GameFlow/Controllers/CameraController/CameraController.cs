using UnityEngine;

public class CameraController : BaseController, IGameSettingsListener {

	public GameObject _cameraContainer { get; private set; }
	public Camera _mainCamera { get; private set; }

	// --------------- Tweakables ---------------

	private float cameraNearClip = 0.1f; // 0.01f
	private float cameraFarClip = 30; // TODO: Make background plane dynamic based on far clip -> size and distance

	private float cameraContainerFollowHeight = 4.5f; // 3.5
	private float cameraFollowDistance = 5.0f; // 4.0

	private float cameraFollowSpeed = 3.5f; // Smoothing
	private float cameraContainerRotationSpeed = 4.0f;

	private float lowerZoomLimit = -1.0f;
	private float uppperZoomLimit = 2.0f;

	private Vector3 cameraContainerRotation = new Vector3(0, 0, 0); // y = 45
	private Vector3 cameraRotation = new Vector3(33, 0, 0); // x = 33

	// --------------- Tweakables ---------------

	private float rotationValue = 0;
	private float currentZoomValue = 0;

	private Transform cameraTarget;
	private Vector3 oldTargetPos;
	private Quaternion newRotation;
	private CustomUserActions userActions;

	public override void Initialize() {
        base.Initialize();

        GameAccesPoint.Instance.cameraController = this;

        _cameraContainer = GameObject.Find("CameraContainer");
        if (_cameraContainer == null) {
            Debug.LogWarning("CameraContainer could not be found");
            return;
        }

        _mainCamera = _cameraContainer.GetComponentInChildren<Camera>();

		SetCameraToSpawn();

		userActions = GameAccesPoint.Instance.managerSystem.inputManager._userActions;

		// --------------- Game Settings Controlled Options ---------------
		if (SettingsManager.Instance.currentSettings.pixelate) {
			_mainCamera.gameObject.AddComponent<PixelateEffect>();
		}
		SettingsManager.Instance.OnSettingsChanged += OnGameSettingsChanged;
		// --------------- Game Settings Controlled Options ---------------

		isInit = true;
    }

	private void SetCameraToSpawn() {
		WorldController worldController = GameAccesPoint.Instance.mainGameState._worldController;
		Vector2 spawnPos = worldController._spawnPos;

		_cameraContainer.transform.position = new Vector3(spawnPos.x * 60, cameraContainerFollowHeight, spawnPos.y * 60);
		_cameraContainer.transform.rotation = Quaternion.Euler(cameraContainerRotation);

		_mainCamera.transform.localPosition = new Vector3(0, 0, -cameraFollowDistance);
		_mainCamera.transform.localRotation = Quaternion.Euler(cameraRotation);

		_mainCamera.orthographic = false;

		_mainCamera.nearClipPlane = cameraNearClip;
		_mainCamera.farClipPlane = cameraFarClip;
	}

	public void SetCameraTarget(Transform cameraTarget) {
		this.cameraTarget = cameraTarget;
	}

	public override void UpdateGameTime(float globalGameTime, float deltaGameTime) {
		if (_cameraContainer == null || cameraTarget == null) {
			return;
		}

		if (Input.GetKey(KeyCode.I)) { // TODO: Change too playeraction -> + next 2
			ZoomCamera(Direction.In);
		} else if (Input.GetKey(KeyCode.O)) {
			ZoomCamera(Direction.Out);
		}

		if (Input.GetKeyDown(KeyCode.U)) {
			ResetZoom();
		}

		_cameraContainer.transform.position = Vector3.Lerp(_cameraContainer.transform.position, new Vector3(cameraTarget.position.x, cameraTarget.position.y + cameraContainerFollowHeight + currentZoomValue, cameraTarget.position.z), deltaGameTime * cameraFollowSpeed);
		_mainCamera.transform.localPosition = Vector3.Lerp(_mainCamera.transform.localPosition, new Vector3(_mainCamera.transform.localPosition.x, _mainCamera.transform.localPosition.y, -cameraFollowDistance - currentZoomValue), deltaGameTime * cameraFollowSpeed);

		RotateCamera();
	}

	public void ZoomCamera(Direction zoom, float zoomValue = 0.1f) {
		switch (zoom) {
			case Direction.In:
				currentZoomValue -= zoomValue;
				break;
			case Direction.Out:
				currentZoomValue += zoomValue;
				break;
		}

		currentZoomValue = Mathf.Clamp(currentZoomValue, lowerZoomLimit, uppperZoomLimit);
	}

	public void ResetZoom() { // TODO: Zoom to before edit value -> external zooming
		currentZoomValue = 0;
	}

	private void RotateCamera() {
		if (userActions.RotateCameraLeft && userActions.RotateCameraRight) {
			rotationValue = 0;
		} else if (userActions.RotateCameraLeft) {
			rotationValue -= cameraContainerRotationSpeed;
		} else if (userActions.RotateCameraRight) {
			rotationValue += cameraContainerRotationSpeed;
		}

		if(cameraTarget.position != oldTargetPos) {
			rotationValue = 0;
		}

		if (rotationValue != 0) {
			newRotation = Quaternion.Euler(new Vector3(0, cameraTarget.eulerAngles.y + rotationValue, 0));
		} else {
			newRotation = Quaternion.Euler(new Vector3(0, cameraTarget.eulerAngles.y, 0));
		}

		if (transform.rotation == newRotation) {
			return;
		}

		_cameraContainer.transform.rotation = Quaternion.Slerp(_cameraContainer.transform.rotation, newRotation, Time.deltaTime * cameraContainerRotationSpeed);

		oldTargetPos = cameraTarget.position;
	}

	public void OnGameSettingsChanged(GameSettings newSettings) {
		if (_mainCamera == null)
			return;

		if (newSettings.pixelate) {
			_mainCamera.gameObject.AddComponent<PixelateEffect>();
		} else {
			PixelateEffect effect = _mainCamera.gameObject.GetComponent<PixelateEffect>();
			if (effect) {
				Destroy(effect);
			}
		}
	}
}