using UnityEngine;
using System.Collections;

public class BaseHerdAnimal : BasePreyAnimal {

	public bool _isLeader { get; private set; }
	public Herd _myHerd { get; private set; }

	// --------------- Tweakables ---------------

	private float alertDistance = 8.0f;
	private float minimalDistanceOnRun = 30.0f;

	// --------------- Tweakables ---------------

	private float runSpeed = 6;

	private bool isAlerted = false;

	private Vector3 enterPoint;
	private Transform runTarget;
	private SphereCollider alertTrigger;

	public override void OnCreate(Vector3 position, Quaternion rotation, Transform parent = null, Vector3 scale = default(Vector3)) {
		base.OnCreate(position, rotation, parent, scale);

		animator = GetComponent<Animator>();
		myAnimalState = AnimalState.Idle;

		alertTrigger = GetComponent<SphereCollider>();
		if (alertTrigger != null) {
			alertTrigger.radius = alertDistance;
		}

		currentHealth = baseHealth;
		isInit = true;
	}

	public override void UpdateGameTime() {
		if (myAnimalState != AnimalState.Moving && myAnimalState != AnimalState.Running) {
			animator.SetFloat("Speed", 0);
		}

		if (transform.position.y < -10) {
			Die();
			Destroy(gameObject);
		}

		if (firstHit) {
			animator.SetFloat("Speed", 1f);
			myAnimalState = AnimalState.Running;
			//isAlerted = true;
			firstHit = false;
		}

		switch (myAnimalState) {
			default:
			case AnimalState.None:
				break;
			case AnimalState.Idle:
				myAnimalState = AnimalState.FindPath;
				break;
			case AnimalState.FindPath:
				if (_isLeader && path == null) {
					GetRandomPath(Vector3.zero);
				} else {
					myAnimalState = AnimalState.Moving;
				}
				break;
			case AnimalState.Moving:
				if (_isLeader) {
					if (path == null) {
						myAnimalState = AnimalState.FindPath;
					} else {
						FollowPath();
					}
				} else {
					FollowLeader();
				}

				animator.SetFloat("Speed", 0.4f);
				break;
			case AnimalState.Alerted:
				if (!isAlerted) {
					// ----- Stop Old State
					StopAllCoroutines();
					isGrazing = false;
					animator.SetBool("Eat", false);
					// ----- Stop Old State

					animator.SetBool("Alerted", true);
					float t = AnimationHelper.GetClipLenght(animator.runtimeAnimatorController, "Animation_Alert_Left"); // TODO: Make dynamic for different animations
					StartCoroutine(SetState(AnimalState.Running, t - 0.5f));
					isAlerted = true;
				}

				float alertedDistance = Vector3.Distance(runTarget.position, enterPoint);
				if (alertedDistance >= 1f) {
					StopAllCoroutines();
					myAnimalState = AnimalState.Running;
				}

				break;
			case AnimalState.Running:
				animator.SetFloat("Speed", 1.0f);

				if (isAlerted) {
					StopCoroutine("Graze"); // TODO: Find more solid fix (probably refactor animals!)
					path = null;
					isAlerted = false;
				}

				if (runTarget != null) {
					float distance = Vector3.Distance(transform.position, runTarget.transform.position);

					if (distance > minimalDistanceOnRun) {
						runTarget = null;
						animator.SetBool("Alerted", false);
						myAnimalState = AnimalState.FindPath;
					}
				}

				if (path != null) {
					RunAllongPath(runSpeed);
				} else {
					Vector3 startDirection = Vector3.zero;
                    if (runTarget != null) {
						startDirection = runTarget.position - transform.position;
					}

					GetRandomPath(-startDirection);
				}

				break;
			case AnimalState.Grazing:
				animator.SetBool("Eat", true);
				if (!isGrazing) {
					animator.SetBool("Eat", false);
					float time = AnimationHelper.GetClipLenght(animator.runtimeAnimatorController, "Animation_Grazing_End");
					StartCoroutine(SetState(AnimalState.Moving, time));
				}
				break;
			case AnimalState.Dying:
				myAnimalState = AnimalState.Dead;
				break;
			case AnimalState.Dead:
				// Set death animation
				break;
		}

		if (isGrazing) {
			myAnimalState = AnimalState.Grazing;
		}

		if (myAnimalState != AnimalState.Dead && myAnimalState != AnimalState.Dying && isHit == true) {
			DecreaseHealthOverTime(damageOnHit);
		}

		//Debug.Log("State: " + myAnimalState);

		//transform.position += Seperation();
	}

	// TODO: Follow the leader based on the Boids alghoritm
	private void FollowLeader() {
		BaseHerdAnimal myLeader = _myHerd._herdLeader;

		Quaternion rotation = Allign();
		Vector3 movement = Cohesion(myLeader.transform.position);

		transform.position += movement;
		transform.rotation = rotation;
	}

	private Vector3 Cohesion(Vector3 leaderPos) {
		Vector3 movement = leaderPos - transform.position;
		movement.Normalize();
		movement *= Time.deltaTime * baseMoveSpeed;

		return movement;
	}

	private Vector3 Seperation() {
		Vector3 seperation = Vector3.zero;

		/*
		Ray ray = new Ray(transform.position, transform.up);
		RaycastHit hitInfo;

        if (Physics.SphereCast(ray, 1.5f, out hitInfo, 1.5f)) {
			//Debug.Log("Distance to hit obj: " + hitInfo.distance + " I hit: " + hitInfo.collider.name, this.gameObject);
			Debug.DrawLine(transform.position, hitInfo.point, Color.magenta, 5f);
		}
		*/

		Collider[] hitObjects = Physics.OverlapSphere(new Vector3(transform.position.x, transform.position.y + 0.8f, transform.position.z), 1f, ~(1 << 8));

        if (hitObjects != null || hitObjects.Length > 0) {
			foreach (Collider col in hitObjects) {
				Debug.DrawLine(transform.position, col.transform.position, Color.magenta, 5f);
				seperation = transform.position - col.transform.position;
				seperation.Normalize();
			}
		}

		return seperation * Time.deltaTime;
	}

	private Quaternion Allign() {
		return Quaternion.identity;
	}

	protected override void Die() {
		if (_isLeader) {
			_myHerd.RemoveHerdAnimal(this);
			_myHerd.SetLeader();
		} else {
			_myHerd.RemoveHerdAnimal(this);
		}

		animator.SetBool("Bleed_Out", true);
		animator.SetBool("Death", true);

		//AudioManager.Instance.PlayByName("deer_dying"); // TODO: Implement with 3d sound

		base.Die();
	}

	public virtual void SetLeaderStatus(bool status) {
		_isLeader = status;
	}

	public virtual void AssignHerd(Herd herd) {
		_myHerd = herd;
	}

	private void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			if (!isHit) {
				runTarget = other.transform;
				enterPoint = other.transform.position;
				myAnimalState = AnimalState.Alerted;
			}
		}
	}

	private void OnTriggerExit(Collider other) {
		//if (other.tag == "Player") {
		//	isAlerted = false;
		//	animator.SetBool("Alerted", false);
		//	myAnimalState = AnimalState.FindPath;
		//}
	}

	private IEnumerator SetState(AnimalState state, float time) {
		yield return new WaitForSeconds(time);
		myAnimalState = state;
	}
}