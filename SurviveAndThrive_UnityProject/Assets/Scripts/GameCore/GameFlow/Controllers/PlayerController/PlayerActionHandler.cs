using UnityEngine;
using System.Collections.Generic;

public class PlayerActionHandler : MonoBehaviour, IHandler {

	public bool _isShooting { get; private set; }
	public BaseTool _currentTool { get; private set; }

	// --------------- Tweakables ---------------
	private float harvestRange = 1f;
	private float harvestDetectRange = 3f;
	// --------------- Tweakables ---------------

	//private float animationTime = 0;
	//private float currentAnimationTime = 0;
	private float currentSmallestDistance = 500;
	private bool isHarvested;

	private BasePickableItem currentPItem;
	private Player parent;

	public void Initialize(Object obj) {
		parent = (obj as Player);

		Bow bow = GameObject.Find("Bow_GRP").GetComponent<Bow>();
		bow.Initialize();
		bow.Equip();
	}

	public void HandleAction() {
		if (_currentTool != null) {
			if (parent.userActions.use) {
				if (!_isShooting) {
					AudioManager.Instance.PlayByName("bow_tension", 0.6f); // TODO: Move to bow logic
					parent.animationHandler.HandleShootAnimation();
				}
				_currentTool.Use();
				_isShooting = true;
			} else if (_isShooting) {
				_currentTool.CancelUse();
				parent.animationHandler.HandleShootAnimation(false, true);
				_isShooting = false;
			}
		}

		if (parent.userActions.harvest.WasPressed) { // TODO: Make more efficient
			FindHarvestItem();
		} 
		
		if (parent.userActions.harvest && !isHarvested) {
			if (Harvest()) {
				isHarvested = true;
				ResetHarvest();
				FindHarvestItem();
			}
		}

		if (parent.userActions.harvest.WasReleased || parent.animationHandler._animator.GetFloat("Speed") > 0) {
			isHarvested = false;
			ResetHarvest();
		}
	}

	public void SetTool(BaseTool tool) {
		_currentTool = tool;
		if (_currentTool != null) {
			parent.animationHandler._animator.SetBool("HasWeapon", true);
		} else {
			parent.animationHandler._animator.SetBool("HasWeapon", false);
		}

		Debug.Log("Current tool: " + _currentTool);
	}

	// --------------- HARVESTING ---------------
	#region Harvesting

	private void FindHarvestItem() {
		Collider[] objectsInRange = Physics.OverlapSphere(parent.transform.position, harvestDetectRange);
		List<BasePickableItem> pickableItemsInRange = new List<BasePickableItem>();

		if (objectsInRange != null && objectsInRange.Length > 0) {
			int i = 0;
			while (i < objectsInRange.Length) {
				BasePickableItem pItem = objectsInRange[i].GetComponent<BasePickableItem>();
				if (pItem != null) {
					pickableItemsInRange.Add(pItem);
				}

				i++;
			}

			if (pickableItemsInRange != null && pickableItemsInRange.Count > 0) {
				for (int j = 0; j < pickableItemsInRange.Count; j++) {
					float distance = Vector3.Distance(parent.transform.position, pickableItemsInRange[j].transform.position);
					if (distance < currentSmallestDistance) {
						currentPItem = pickableItemsInRange[j];
						currentSmallestDistance = distance;
					}
				}
			}
		}
	}

	private bool Harvest() {
		if (currentPItem != null) {
			Vector3 lookDir = currentPItem.transform.position - parent.transform.position;
			lookDir.y = 0;
			Quaternion newRot = Quaternion.LookRotation(lookDir);
			parent.transform.rotation = Quaternion.Lerp(parent.transform.rotation, newRot, Time.deltaTime * 5);

            float distance = Vector3.Distance(parent.transform.position, currentPItem.transform.position);
			if (distance <= harvestRange) {
				PlayHarvestAnimation();
				return currentPItem.HarvestItem();
			} else if (distance <= harvestDetectRange) {
				// Walk to and harvest
			} else {
				ResetHarvest();
			}
		}
		return false;
	}

	private void ResetHarvest() {
		if (currentPItem != null) {
			currentPItem.Reset();
		}

		parent.animationHandler.SetGatherAnimation(false);
		currentSmallestDistance = 500;
		currentPItem = null;
	}

	private void PlayHarvestAnimation() {
		parent.animationHandler.SetGatherAnimation(true);
	}

	#endregion
	// --------------- HARVESTING ---------------
}