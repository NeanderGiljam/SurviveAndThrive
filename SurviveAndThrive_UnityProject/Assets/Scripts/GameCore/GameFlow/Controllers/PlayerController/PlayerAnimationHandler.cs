using UnityEngine;
using System.Collections;

public class PlayerAnimationHandler : MonoBehaviour, IHandler {

	public Animator _animator { get; private set; }
	public RuntimeAnimatorController _rtAnimatorController { get; private set; }

	// --------------- Tweakables ---------------

	private float maxIdleInterval = 60;
	private float minIdleInterval = 10;

	// --------------- Tweakables ---------------

	private float idleIntervalTimer = 0;
	private float currentIdleInterval = 10;

	private Player parent;
	//private PlayerState[] idleAllowStates = new PlayerState[] {
	//	PlayerState.Still,
	//	PlayerState.Idle
	//};

	public void Initialize(Object obj) {
		parent = (obj as Player);

		_animator = parent.transform.GetComponent<Animator>();
		_rtAnimatorController = _animator.runtimeAnimatorController;
	}

	public void SetGatherAnimation(bool isStart) {
		if (isStart) {
			_animator.SetBool("Collecting", true);
		} else {
			_animator.SetBool("Collecting", false);
		}
	}

	public void HandleShootAnimation(bool shoot = false, bool cancel = false) {
		if (cancel) {
			_animator.SetBool("FirePrep", false);
			return;
		}

		if (!shoot) {
			_animator.SetBool("FirePrep", true);
		} else {
			float time = AnimationHelper.GetClipLenght(_rtAnimatorController, "Animation_Fire_Standing_Still");
			StartCoroutine(HandleBoolAnimation("Attack", time));
			_animator.SetBool("FirePrep", false);
		}
	}

	public void HandleAnimations(float speed) {
		_animator.SetFloat("Speed", speed);

		idleIntervalTimer += Time.deltaTime;

		if (speed == 0 && idleIntervalTimer >= currentIdleInterval) {
			//bool canIdle = false;
			//for (int i = 0; i < idleAllowStates.Length; i++) {
			//	if (parent._currentPlayerState == idleAllowStates[i]) {
			//		canIdle = true;
			//	}
			//}

			currentIdleInterval = Random.Range(minIdleInterval, maxIdleInterval);
			idleIntervalTimer = 0;

			//if (!canIdle) {
			//	return;
			//}

			StartCoroutine(HandleBoolAnimation("Idle", 0.1f));
		}
	}

	private IEnumerator HandleBoolAnimation(string parameterName, float time) {
		_animator.SetBool(parameterName, true);
		yield return new WaitForSeconds(time);
		_animator.SetBool(parameterName, false);
	}
}