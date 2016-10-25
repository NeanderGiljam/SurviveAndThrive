using UnityEngine;
using System.Collections;

public class Arrow : BasePickableItem {

	private float velocity;
	private float startTime;
	private float startHeight;

	private bool isInit = false;
	private bool nearTarget = false;

	private Vector3 initialForward;
	private Vector3 targetPos;

	private Material myMaterial;

	// ----- Collision -----
	public float skinWidth = 0.1f;
	private float minimumExtent;
	private float partialExtent;
	private float sqrMinimumExtent;

	private Vector3 previousPosition;
	private Collider myCollider;
	// ----- Collision -----

	public void Initialize(Vector3 targetPos, float velocity, Quaternion bowRotation) {
		this.velocity = velocity;
		this.targetPos = targetPos;
		transform.rotation = bowRotation;
		initialForward = transform.forward;

		startHeight = transform.position.y;
		startTime = Time.realtimeSinceStartup;

		myMaterial = GetComponent<Renderer>().material;

		// ----- Collision -----

		myCollider = GetComponent<Collider>();
		previousPosition = transform.position;
		minimumExtent = Mathf.Min(Mathf.Min(myCollider.bounds.extents.x, myCollider.bounds.extents.y), myCollider.bounds.extents.z);
		partialExtent = minimumExtent * (1.0f - skinWidth);
		sqrMinimumExtent = minimumExtent * minimumExtent;

		// ----- Collision -----

		isInit = true;
	}

	private void FixedUpdate() {
		if (isInit) {
			if (targetPos != Vector3.zero && !nearTarget) {
				transform.LookAt(targetPos);
				transform.position += transform.forward * Time.deltaTime * velocity;

				float distanceToTarget = Vector3.Distance(transform.position, targetPos);
				if (distanceToTarget < 0.3f) {
					startTime = Time.realtimeSinceStartup;
					startHeight = transform.position.y;
					initialForward = transform.forward;
					nearTarget = true;
				}
			} else {
				float t = Time.realtimeSinceStartup - startTime;
				float rad = 1 * Mathf.Deg2Rad;
				float yPos = velocity * Mathf.Sin(rad) * t - 0.5f * 4.9f * Mathf.Pow(t, 2f);

				transform.position += initialForward * Time.deltaTime * velocity;
				transform.position = new Vector3(transform.position.x, startHeight + yPos, transform.position.z);
				transform.LookAt(new Vector3(transform.position.x, startHeight + yPos, transform.position.z));
			}

			DoCollisionCheck();
		}
	}

	private void DoCollisionCheck() { // TODO: Make better working (?raycast distance)
		Vector3 movementThisStep = transform.position - previousPosition;
		float movementSqrMagnitude = movementThisStep.sqrMagnitude;

		if (movementSqrMagnitude > sqrMinimumExtent) {
			float movementMagnitude = Mathf.Sqrt(movementSqrMagnitude);
			RaycastHit hitInfo;

			if (Physics.Raycast(previousPosition, movementThisStep, out hitInfo, movementMagnitude, (1 << 11))) {
				if (!hitInfo.collider)
					return;

				if (hitInfo.collider.isTrigger)
					hitInfo.collider.SendMessage("OnTriggerEnter", myCollider);

				if (!hitInfo.collider.isTrigger) {
					transform.position = hitInfo.point - (movementThisStep / movementMagnitude) * partialExtent;
				}
			}
		}

		previousPosition = transform.position;
	}

	public void RemoveArrow() {
		// TODO: Fade Away -> then destroy
		StartCoroutine("FadeAway");
	}

	private void OnCollisionEnter(Collision other) {
		if (!isInit) {
			return;
		}

		Rigidbody myBody = GetComponent<Rigidbody>();
		if (myBody != null) {
			myBody.constraints = RigidbodyConstraints.FreezeAll;
		}

		// TODO: Make more dynamic
		if (other.collider.tag != "Weapon" && other.collider.tag != "Player") {
			BaseAnimal animal = other.collider.transform.root.GetComponent<BaseAnimal>();
			if (animal != null) {
				if (other.collider.name.Contains("leg")) {
					animal.Hit(.8f);
				} else {
					animal.Hit(3f);
				}
				transform.SetParent(other.collider.transform);
				transform.GetComponent<Collider>().enabled = false;
			}
			isInit = false;
		}
	}

	private IEnumerator FadeAway() {
		for (float alpha = 1f; alpha > 0; alpha -= 0.05f) {
			myMaterial.color = new Color(myMaterial.color.r, myMaterial.color.g, myMaterial.color.b, alpha);
			yield return null;
		}

		StopAllCoroutines();
		Destroy(gameObject);
	}
}