
public interface IEventReceiver {

	void OnBeginDrag(object[] args);
	void OnDrag(object[] args);
	void OnEndDrag(object[] args);
	void OnDrop(object[] args);
	void OnPointerClick(object[] args);

}