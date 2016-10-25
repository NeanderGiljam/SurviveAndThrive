using UnityEngine;
using System.Collections;

public class LODObject : MonoBehaviour {

	public float[] lodDistances;
	public GameObject[] lodObjects;
	
	private int currentLODLevel = -1;
	private bool isInit;

	private void Start () {
		if (lodDistances == null || lodDistances.Length <= 0 || lodObjects == null || lodObjects.Length <= 0) {
			Debug.LogWarning("LOD object does not meet the necessary requirements!");
			return;
		}

		ChangeLOD(lodObjects.Length - 1);

		isInit = true;
	}

	private void FixedUpdate() {
		if (!isInit)
			return;

		float distance = Vector3.Distance(transform.position, Camera.main.transform.position);
		int level = -1;

		for (int i = 0; i < lodDistances.Length; i++) {
			if (distance < lodDistances[i]) {
				level = i;
				break;
			}
		}

		if (level == -1) {
			level = lodObjects.Length - 1;
		}

		if (currentLODLevel != level) {
			ChangeLOD(level);
		}
	}

	private void ChangeLOD(int level) {
		lodObjects[level].SetActive(true);
		currentLODLevel = level;
		for (int i = 0; i < lodObjects.Length; i++) {
			if(i != level)
				lodObjects[i].SetActive(false);
		}
	}
}