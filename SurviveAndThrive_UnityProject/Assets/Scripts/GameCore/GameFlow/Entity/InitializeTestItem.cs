using UnityEngine;
using System.Collections;

public class InitializeTestItem : MonoBehaviour {

	public bool init = false;

	private bool needsInit = true;

	private void Update() {
		if (needsInit && init) {
			BaseItem item = GetComponent<BaseItem>();
			item.OnCreate(transform.position, transform.rotation, null, transform.lossyScale);
			needsInit = false;
		}
	}
}
