using UnityEngine;
using UnityEngine.EventSystems;

public class UIEventsHelper : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler {

	public int objectDepth = 0;

	public bool dragEvents = true;
	public bool dropEvents = true;
	public bool clickEvents = true;
	//public bool pointerEvents = true;

	[Header("If null -> Depth is used")]
	public Transform eventReceiver;

	private void Start() {
		if (eventReceiver == null) {
			eventReceiver = transform;

			for (int i = 0; i < objectDepth; i++) {
				eventReceiver = eventReceiver.parent;
			}
		}
	}

	public void OnBeginDrag(PointerEventData eventData) {
		if (!dragEvents)
			return;

		object[] args = new object[] { eventData, gameObject };
		eventReceiver.SendMessage("OnBeginDrag", args, SendMessageOptions.DontRequireReceiver);
	}

	public void OnDrag(PointerEventData eventData) {
		if (!dragEvents)
			return;

		object[] args = new object[] { eventData, gameObject };
		eventReceiver.SendMessage("OnDrag", args, SendMessageOptions.DontRequireReceiver);
	}

	public void OnEndDrag(PointerEventData eventData) {
		if (!dragEvents)
			return;

		object[] args = new object[] { eventData, gameObject };
		eventReceiver.SendMessage("OnEndDrag", args, SendMessageOptions.DontRequireReceiver);
	}

	public void OnDrop(PointerEventData eventData) {
		if (!dropEvents)
			return;
		
		object[] args = new object[] { eventData, gameObject };
		eventReceiver.SendMessage("OnDrop", args, SendMessageOptions.DontRequireReceiver);
	}

	public void OnPointerClick(PointerEventData eventData) {
		if (!clickEvents)
			return;

		object[] args = new object[] { eventData, gameObject };
		eventReceiver.SendMessage("OnPointerClick", args, SendMessageOptions.DontRequireReceiver);
	}
}