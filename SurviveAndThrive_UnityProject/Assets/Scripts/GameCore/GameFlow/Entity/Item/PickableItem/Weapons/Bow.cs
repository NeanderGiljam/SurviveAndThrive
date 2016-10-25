using UnityEngine;
using System.Collections.Generic;
using System;

public class Bow : BaseTool {

	private float velocity;
	private float maxVelocity = 30;

	private Transform arrowAnchor;
	private Transform target;

	private Player player;
	private Transform currentTarget;
	private Vector3 forwardOnTarget;

	public override void Initialize() {
		player = GameAccesPoint.Instance.mainGameState._playerController._player;
		cameraController = GameAccesPoint.Instance.cameraController; // TODO: Is null due to init order -> fix

		baseToolDamage = 10.0f;
		baseBuildUpTime = 3.0f; // Needed for animation
		currentBuildUpTime = 0;

		arrowAnchor = GameObject.Find("r_ArrowAnchor").transform;

		target = transform.FindChild("r_Targetting");
		if (target != null) {
			target.gameObject.SetActive(false);
		}

		base.Initialize();
	}

	public override void Use() {
		currentBuildUpTime += 1 * Time.deltaTime;
		currentBuildUpTime = Mathf.Clamp(currentBuildUpTime, 0, baseBuildUpTime);

		HitCheck();

		if (currentBuildUpTime > (baseBuildUpTime / 2) && currentBuildUpTime != baseBuildUpTime) {
			if (cameraController == null) {
				cameraController = GameAccesPoint.Instance.cameraController;
			} else {
				cameraController.ZoomCamera(Direction.Out, 0.01f);
			}
		}

		velocity = MathHelper.MapValueToRange(currentBuildUpTime, 0, baseBuildUpTime, 0, 1) * maxVelocity;
	}

	public override void CancelUse() {
		if (currentBuildUpTime > (baseBuildUpTime / 2)) {
			player.animationHandler.HandleShootAnimation(true);
			Shoot();
		} else {
			player.animationHandler.HandleShootAnimation(false, true);
		}

		currentTarget = null;
		target.gameObject.SetActive(false);
		target.transform.position = transform.position;
		cameraController.ResetZoom();
		currentBuildUpTime = 0;
	}

	private void Shoot() {
		BaseItem tmpArrow = gameItemDatabase.CreateItemInstance("15:101", arrowAnchor.position, Quaternion.identity);
		Arrow newArrow = tmpArrow.GetComponent<Arrow>();
		AudioManager.Instance.PlayByName("bow_shot");

		if (currentTarget != null) {
			newArrow.Initialize(currentTarget.transform.position, velocity, player.transform.rotation);
		} else {
			newArrow.Initialize(Vector3.zero, velocity, player.transform.rotation);
		}
	}

	private void HitCheck() {
		if (target == null) {
			return;
		}

		target.gameObject.SetActive(true);

		if (currentTarget == null) {
			target.localScale = new Vector3(target.localScale.x, target.localScale.y, 17); // TODO: Make distance dynamic
		} else {
			float angle = Vector3.Angle(transform.forward, forwardOnTarget);
			if (angle > 10) {
				currentTarget = null;
			}
		}
	}

	private void OnTriggerEnter(Collider other) {
		if (currentTarget == null) {
			if (other.transform.root.GetComponent<BaseAnimal>()) {
				currentTarget = other.transform;
				forwardOnTarget = transform.forward;
			}
		}
	}

	protected override void Animate() {
		// When animations are seperate handle from here
	}
}