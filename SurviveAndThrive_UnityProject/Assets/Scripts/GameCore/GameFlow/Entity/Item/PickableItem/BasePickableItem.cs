using UnityEngine;
using System.Collections;

public class BasePickableItem : BaseItem {

	public bool _isOnGround { get; private set; }

	public Sprite myIcon;

	private float animationHarvestTime = 8.7f;
	private float harvestTime = 1.5f; // TODO: If needed make dynamic to the gather animation that is used
	private float currentHarvestTime;

	private PlayerInventoryHandler inventoryHandler;

	public override void OnCreate(Vector3 position, Quaternion rotation, Transform parent = null, Vector3 scale = default(Vector3)) {
		base.OnCreate(position, rotation, parent, scale);
	}

	public override void Initialize() {
		base.Initialize();

		inventoryHandler = GameAccesPoint.Instance.mainGameState._playerController._player.inventoryHandler;
		_isOnGround = true;
	}

	public bool HarvestItem() {
		currentHarvestTime += 1 * Time.deltaTime;

		if (currentHarvestTime >= harvestTime) {
			PickupItem();
		}

		if (currentHarvestTime >= animationHarvestTime) {
			return true;
		}

		return false;
	}

	public void Reset() {
		currentHarvestTime = 0;
	}

	private void PickupItem() {
		if (!_isOnGround)
			return;

		if (inventoryHandler.AddItem(this, myIcon)) {
			SetVisualState(false);
		}
	}

	public void SetVisualState(bool state) {
		gameObject.GetComponent<Collider>().enabled = state;
		gameObject.SetActive(state);
		_isOnGround = state;

		// TODO: Pool the object !? Create pool functionality in base class
	}

	public override void Destroy(float time = 0) {
		// TODO: Call when consumed -> remove from inventory etc

		base.Destroy(time);
	}
}