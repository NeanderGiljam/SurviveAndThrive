using UnityEngine;
using System.Collections;

public class BasePreyAnimal : BaseAnimal {

	// --------------- Tweakables ---------------

	public float minGrazeTime = 2.0f;
	public float maxGrazeTime = 10.0f;

	private float minPathStepDistance = 1.0f;
	private float maxPathStepDistance = 15.0f;

	private float rotationSpeed = 6.0f;

	private int pathLenght = 5;

	// --------------- Tweakables ---------------

	private int pathIndex = 0;
	protected bool isGrazing = false;
	protected Vector3[] path;

	public override void UpdateGameTime() {
		if (path == null) {
			GetRandomPath(Vector3.zero);
		} else {
			FollowPath();
		}
	}

	protected void FollowPath() {
		transform.position = Vector3.MoveTowards(transform.position, new Vector3(path[pathIndex].x, transform.position.y, path[pathIndex].z), Time.deltaTime * baseMoveSpeed);
		Rotate();

		if (CheckPathBlocked()) {
			ResetPath();
			return;
		}

		float distance = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(path[pathIndex].x, path[pathIndex].z));

		if (distance <= 0.1f) { // if (transform.position == path[pathIndex]) {
			if (!isGrazing) {
				StartCoroutine(Graze());
			}
		}
	}

	protected void RunAllongPath(float speed) {
		if (pathIndex >= path.Length) {
			ResetPath();
			return;
		}

		StopAllCoroutines();

		transform.position = Vector3.MoveTowards(transform.position, new Vector3(path[pathIndex].x, transform.position.y, path[pathIndex].z), Time.deltaTime * speed);
		Rotate();

		if (CheckPathBlocked()) {
			ResetPath();
			return;
		}

		float distance = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(path[pathIndex].x, path[pathIndex].z));

		if (distance <= 0.1f) {
			pathIndex++;
		}
	}

	protected void GetRandomPath(Vector3 startDirection) {
		path = new Vector3[pathLenght];
		Vector3 startPos = transform.position;
		Vector3 currentPos = Vector3.zero;
		Vector3 nextPos = Vector3.zero;
		Vector3 direction = Vector3.zero;

		pathIndex = 0;

		for (int i = 0; i < pathLenght; i++) {
			if (i == 0 && startDirection != Vector3.zero) {
				currentPos = startPos;
				nextPos = transform.position + startDirection;
				direction = new Vector3(nextPos.x - currentPos.x, transform.position.y, nextPos.z - currentPos.z).normalized;
			} else if (i == 0) {
				currentPos = startPos;
				nextPos = transform.position + Random.onUnitSphere;
				direction = new Vector3(nextPos.x - currentPos.x, transform.position.y, nextPos.z - currentPos.z).normalized;
			} else {
				float nextXOffset = Random.Range(-1.0f, 1.0f);
				float nextZOffset = 1 - Mathf.Abs(nextXOffset);
				Vector3 newDirection = new Vector3(nextXOffset, nextPos.y, nextZOffset).normalized;

				float angle = (180 / Mathf.PI) * Mathf.Atan2(currentPos.x - startPos.x, currentPos.z - startPos.z);
				direction = Quaternion.AngleAxis(angle, Vector3.up) * newDirection;
			}

			float pathStepDistance = Random.Range(minPathStepDistance, maxPathStepDistance);
			nextPos = currentPos + (direction * pathStepDistance);

			RaycastHit hitInfo;
			Ray ray = new Ray(new Vector3(nextPos.x, transform.position.y + 5, nextPos.z), -Vector3.up);
			Physics.Raycast(ray, out hitInfo, 5f, (1 << 8));

			nextPos.y = hitInfo.point.y; 

			path[i] = nextPos;

			//Debug.DrawLine(currentPos, nextPos, new Color(0.2f * i, 0, 0, 1), 10f);

			currentPos = nextPos;
		}
	}

	private void Rotate() {
		Vector3 targetDir = path[pathIndex] - transform.position;
		targetDir.y = 0;
		Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, Time.deltaTime * rotationSpeed, 0.0f);
		transform.rotation = Quaternion.LookRotation(newDir);
	}

	private void ResetPath() {
		pathIndex = 0;
		path = null;
	}

	private bool CheckPathBlocked() {
		// TODO: If needed add checks for either side -> Less clipping through objects
		Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z), transform.forward + new Vector3(0, 0.3f, 0));
		if (Physics.Raycast(ray, 1f, ~(1 << 8))) {
			//Debug.DrawLine(ray.origin, transform.position + ray.direction * 2f, Color.red, 10f);
			return true;
		}

		return false;
	}

	protected IEnumerator Graze() {
		isGrazing = true;
		// Do graze animation

		float grazeTime = Random.Range(minGrazeTime, maxGrazeTime);
		yield return new WaitForSeconds(grazeTime);

		pathIndex++;
		if (pathIndex >= pathLenght) {
			ResetPath();
		}

		isGrazing = false;
	}
}