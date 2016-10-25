using UnityEngine;
using System.Collections;

public class PlayerMovementHandler : MonoBehaviour, IHandler {

	private float maxWalkSpeed = 2.0f;
	private float maxRunSpeed = 4.5f;
	private float maxTurnSpeed = 150.0f; // 6 || 150.0f
	private float shootTurnSpeed = 100.0f;
	private float deadZone = 0.19f;

	private Vector3 rotationDirection;
	private Rigidbody playerRigidbody;

	private Player parent;
	private CustomUserActions userActions;

	public void Initialize(Object obj) {
		parent = (obj as Player);
		playerRigidbody = parent.transform.GetComponent<Rigidbody>();
		userActions = parent.userActions;
	}

	public void MovePlayer(float deltaGameTime) {
		if (playerRigidbody.velocity != Vector3.zero) {
			playerRigidbody.velocity = new Vector3(0, playerRigidbody.velocity.y, 0);
		}

		Vector3 movement = new Vector3(0, 0, userActions.MoveVertical);
		Quaternion newRotation = Quaternion.Euler(new Vector3(0, userActions.MoveHorizontal * deltaGameTime * maxTurnSpeed, 0));

		parent.transform.rotation *= !parent.actionHandler._isShooting ? newRotation : GetShootRotation(deltaGameTime);

		if (movement != Vector3.zero && !parent.actionHandler._isShooting) {
			//float moveSpeed = MathHelper.Average(new float[] { userActions.MoveHorizontal.Value, userActions.MoveVertical.Value });
			float currentSpeed = userActions.walk.IsPressed ? maxWalkSpeed : maxRunSpeed;

			parent.animationHandler.HandleAnimations(currentSpeed == maxWalkSpeed ? 0.5f : 1.0f);
			parent.transform.Translate(movement * deltaGameTime * currentSpeed, Space.Self); // movement.normalized * deltaGameTime * (currentSpeed * moveSpeed)
		} else {
			parent.animationHandler.HandleAnimations(0);
		}
	}

	private Vector2 oldMousePos;

	private Quaternion GetShootRotation(float deltaGameTime) { // TODO: Make cleaner
		Quaternion newRotation = Quaternion.identity;

		if (userActions.use.LastInputType == InControl.BindingSourceType.MouseBindingSource) {
			Vector2 heading = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - oldMousePos;
			float dot = Vector2.Dot(heading, oldMousePos);
			if (dot != 0) {
				int val = dot > 0 ? 1 : -1;
				newRotation = Quaternion.Euler(new Vector3(0, val * deltaGameTime * shootTurnSpeed, 0));
			}
		}
		oldMousePos = Input.mousePosition;

		return newRotation;

		//float xRot = 0;
		//float yRot = 0;

		//if (userActions.RotateHorizontal > deadZone || userActions.RotateHorizontal < deadZone) {
		//	xRot = userActions.RotateHorizontal;
		//}

		//if (userActions.RotateVertical > deadZone || userActions.RotateVertical < deadZone) {
		//	yRot = userActions.RotateVertical;
		//}

		//rotationDirection = new Vector3(xRot, 0, yRot);

		//if (userActions.use.LastInputType == InControl.BindingSourceType.MouseBindingSource) {
		//	Plane plane = new Plane(Vector3.up, Vector3.zero);
		//	Ray ray = GameAccesPoint.Instance.cameraController._mainCamera.ScreenPointToRay(Input.mousePosition);
		//	Vector3 hitpoint = Vector3.zero;

		//	float ent = 100.0f;
		//	if (plane.Raycast(ray, out ent)) {
		//		hitpoint = ray.GetPoint(ent);
		//	}

		//	rotationDirection = hitpoint - parent.transform.position;
		//	RotatePlayer(deltaGameTime, rotationDirection, false);
		//} else {
		//	RotatePlayer(deltaGameTime, rotationDirection);
		//}
	}

	private void RotatePlayer(float deltaGameTime, Vector3 targetDirection, bool transformDirection = true) {
		if (transformDirection) {
			targetDirection = GameAccesPoint.Instance.mainGameState._cameraController._mainCamera.transform.TransformDirection(targetDirection);
			targetDirection.Normalize();
		}

		targetDirection.y = 0;

		if (targetDirection == Vector3.zero) {
			return;
		}

		Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
		Quaternion newRotation = Quaternion.Lerp(parent.transform.rotation, targetRotation, deltaGameTime * maxTurnSpeed);

		parent.transform.rotation = newRotation;
	}
}