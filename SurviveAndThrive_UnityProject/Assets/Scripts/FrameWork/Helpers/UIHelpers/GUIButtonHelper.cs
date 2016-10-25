using UnityEngine;
using UnityEngine.EventSystems;

public class GUIButtonHelper : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public int objectDepth = 0;
	private Transform collisionReceiver;

	private void Start() {
		collisionReceiver = transform;

		for (int i = 0; i < objectDepth; i++) {
			collisionReceiver = collisionReceiver.parent;
		}
	}

	public void OnPointerEnter(PointerEventData eventData) {
		collisionReceiver.SendMessage("OnPointerEnter", gameObject, SendMessageOptions.DontRequireReceiver);
    }

	public void OnPointerExit(PointerEventData eventData) {
		collisionReceiver.SendMessage("OnPointerExit", gameObject, SendMessageOptions.DontRequireReceiver);
	}
}
