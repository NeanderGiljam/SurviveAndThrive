using UnityEngine;
using System.Collections;

public class CollisionEventHelper : MonoBehaviour {

	public int objectDepth = 0;
	private Transform collisionReceiver;

	private void Start() {
		collisionReceiver = transform;

		for (int i = 0; i < objectDepth; i++) {
			collisionReceiver = collisionReceiver.parent;
		}
	}

	private void OnCollisionEnter(Collision other) {
		collisionReceiver.SendMessage("OnCollisionEnter", other, SendMessageOptions.DontRequireReceiver);
	}

	private void OnTriggerEnter(Collider other) {
		collisionReceiver.SendMessage("OnTriggerEnter", other, SendMessageOptions.DontRequireReceiver);
    }
}