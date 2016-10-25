using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class HUDGUIState : GuiState, IEventReceiver {

    private GameObject hudGUI;

	private Image healthBar;
	private Image hungerBar;
	private Image thirstBar;
	private Image energyBar;
    private Image hungerIcon;
    private Image thirstIcon;
    private Image energyIcon;

    private Button rButton;
	private Button lButton;
	private Text amountText;

	private PlayerInventoryHandler inventoryHandler;

    // --------------- Hearth Beat ---------------
    private IconAnimator healthIconAnim;
    // --------------- Hearth Beat ---------------

    // --------------- Drag & Drop ---------------
    private GameObject currentDraggedObject;
	private Transform startParent;
	private bool wasDropped = false;
	// --------------- Drag & Drop ---------------

	public override void Initialize() {
        base.Initialize();

        GameAccesPoint.Instance.hudGUIState = this;
        GameAccesPoint.Instance.managerSystem.stateManager.guiStates.Add(this);

        // --------------- Root Setup ---------------
        hudGUI = gameObject;
        hudGUI.transform.SetParent(uiRoot);
        hudGUI.transform.position = uiRoot.position;
		// --------------- Root Setup ---------------

		GameObject tmpBar = GameObject.Find("r_HealthBar");
        if (tmpBar != null) {
			healthBar = tmpBar.GetComponent<Image>();
		}

		tmpBar = GameObject.Find("r_HungerBar");
		if (tmpBar != null) {
			hungerBar = tmpBar.GetComponent<Image>();
		}

		tmpBar = GameObject.Find("r_ThirstBar");
		if (tmpBar != null) {
			thirstBar = tmpBar.GetComponent<Image>();
		}

		tmpBar = GameObject.Find("r_EnergyBar");
		if (tmpBar != null) {
			energyBar = tmpBar.GetComponent<Image>();
		}

        tmpBar = GameObject.Find("r_HungerIcon");
        if (tmpBar != null)
        {
            hungerIcon = tmpBar.GetComponent<Image>();
        }
        tmpBar = GameObject.Find("r_ThirstIcon");
        if (tmpBar != null)
        {
            thirstIcon = tmpBar.GetComponent<Image>();
        }
        tmpBar = GameObject.Find("r_EnergyIcon");
        if (tmpBar != null)
        {
            energyIcon = tmpBar.GetComponent<Image>();
        }

        tmpBar = GameObject.Find("r_HealthIcon");
        if (tmpBar != null){
            healthIconAnim = GameObject.Find("r_HealthIcon").GetComponent<IconAnimator>();
        }
        if (healthBar == null || hungerBar == null || thirstBar == null || energyBar == null || healthIconAnim == null) {
			Debug.LogError("One of the bars could not be found.");
		}

		amountText = GameObject.Find("r_Amount_Text").GetComponent<Text>();
		rButton = GameObject.Find("r_Right_Button").GetComponent<Button>();
        lButton = GameObject.Find("r_Left_Button").GetComponent<Button>();

		rButton.onClick.AddListener(OnRightButtonClicked);
		lButton.onClick.AddListener(OnLeftButtonClicked);

		isInit = true;
    }

	public override void SetControlScheme() {
        base.SetControlScheme();

        inputManager.SetControlScheme<InGameControlScheme>();
    }

    public override void SetActiveState(bool state) {
        base.SetActiveState(state);

		hudGUI.SetActive(state);
	}

	// --------------- STATS ---------------
	#region STATS

	public void UpdateStats(float health, float hunger, float thirst, float energy) {
        if (healthBar == null || hungerBar == null || thirstBar == null || energyBar == null)
            return;

		healthBar.fillAmount = health;
		hungerBar.fillAmount = hunger;
		thirstBar.fillAmount = thirst;
        energyBar.fillAmount = energy;
        float tempVal = Mathf.Lerp(0.6f,1, hunger);
        hungerIcon.transform.localScale = new Vector3(tempVal, tempVal, tempVal);
        tempVal = Mathf.Lerp(0.6f,1, thirst);
        thirstIcon.transform.localScale = new Vector3(tempVal, tempVal, tempVal);
        tempVal = Mathf.Lerp(0.6f,1, energy);
        energyIcon.transform.localScale = new Vector3(tempVal, tempVal, tempVal);
        if (health <= 0.25f)
        {
            healthIconAnim.duration = health * 4;
            Mathf.Clamp(healthIconAnim.duration, 0.25f, 1);
            healthIconAnim.AnimateIt();
        }
	}

	#endregion
	// --------------- STATS ---------------

	private void OnRightButtonClicked() {
		inventoryHandler.SwitchRecipeVisual(1);
	}

	private void OnLeftButtonClicked() {
		inventoryHandler.SwitchRecipeVisual(-1);
	}

	public void SetRecipeCounter(string counterText) {
		amountText.text = counterText;
	}

	// --------------- INVENTORY ---------------
	#region INVENTORY

	public void AddPlayerInventoryHandler(PlayerInventoryHandler inventoryHandler) { // TODO: Find and implement safer way or apply safety
		this.inventoryHandler = inventoryHandler;
	}

	public Dictionary<Transform, InventoryItem> GetInitialInventory() {
		Dictionary<Transform, InventoryItem> tmpInventory = new Dictionary<Transform, InventoryItem>();

		// Inventory Slots
		Transform slotsParent = transform.FindChild("Inventory/InventorySlots");
		int childCount = slotsParent.childCount;
		for (int i = 0; i < childCount; i++) {
			tmpInventory.Add(slotsParent.GetChild(i), null);
		}

		// Crafting Slots
		slotsParent = transform.FindChild("Crafting/CraftingSlots");
		childCount = slotsParent.childCount;
		for (int i = 0; i < childCount; i++) {
			tmpInventory.Add(slotsParent.GetChild(i), null);
		}

		return tmpInventory;
	}

	public void ClearInventory() { // TODO: This or destroy and reinstance
		Dictionary<Transform, InventoryItem> tmpInventory = inventoryHandler._inventory;
		foreach (KeyValuePair<Transform, InventoryItem> pair in tmpInventory) {
			if (pair.Key.childCount > 0) {
				for (int i = 0; i < pair.Key.childCount; i++) {
					Transform child = pair.Key.GetChild(i);
					if (child != null) {
						Destroy(child.gameObject);
					}
				}
			}
		}
    }

	public void OnBeginDrag(object[] args) {
		GameObject sourceObject = args[1] as GameObject;

		currentDraggedObject = sourceObject;
		startParent = sourceObject.transform.parent;

		currentDraggedObject.transform.SetParent(startParent.root);
		Image imageComp = currentDraggedObject.GetComponent<Image>();
		if (imageComp != null) {
			imageComp.raycastTarget = false;
		}
	}

	public void OnDrag(object[] args) {
		if (currentDraggedObject != null) {
			currentDraggedObject.transform.position = Input.mousePosition;
		}
	}

	public void OnEndDrag(object[] args) {
		if (!wasDropped) {
			if (inventoryHandler.DropItem(startParent)) {
				ResetVisual(currentDraggedObject);
				return;
			}

			currentDraggedObject.transform.position = startParent.position;
			currentDraggedObject.transform.SetParent(startParent);
		}

		Image imageComp = currentDraggedObject.GetComponent<Image>();
		if (imageComp != null) {
			imageComp.raycastTarget = true;
		}

		ResetVisual();
	}

	public void OnDrop(object[] args) {
		wasDropped = true;

		Transform newParent = (args[1] as GameObject).transform;

		if (currentDraggedObject != null) {
			if (newParent.name == "ToolSlot" || newParent.name == "InventorySlot" || newParent.name == "CraftSlot") { // TODO: Find a way to make this name independent
				if (inventoryHandler.MoveItem(startParent, newParent)) {
					currentDraggedObject.transform.position = newParent.position;
					currentDraggedObject.transform.SetParent(newParent);
					return;
				}
			} else {
				bool swapped = inventoryHandler.SwapItems(startParent, newParent.parent);
				if (swapped) {
					currentDraggedObject.transform.position = newParent.parent.position;
					currentDraggedObject.transform.SetParent(newParent.parent);
					//newParent.position = startParent.position;
					//newParent.SetParent(startParent);
					StartCoroutine(AnimateItem(newParent, startParent, .1f));

					return;
				}
			}

			//currentDraggedObject.transform.position = startParent.position;
			//currentDraggedObject.transform.SetParent(startParent);

			StartCoroutine(AnimateItem(currentDraggedObject.transform, startParent, .1f));
		}
	}

	public void OnPointerClick(object[] args) {
		PointerEventData eventData = args[0] as PointerEventData;
		GameObject sourceObject = args[1] as GameObject;

		if (eventData.button == PointerEventData.InputButton.Right) {
			bool consumed = inventoryHandler.UseInventoryItem(sourceObject.transform.parent);
			if (consumed) {
				ResetVisual(sourceObject);
			}
		}
	}

	private void ResetVisual(GameObject visual = null) {
		if (visual) {
			Destroy(visual);
		}

		currentDraggedObject = null;
		startParent = null;
		wasDropped = false;
	}

	private IEnumerator AnimateItem(Transform inventoryVisual, Transform newSlot, float time) {
		float elapsedTime = 0;
		Vector3 startingPos = inventoryVisual.position;
		inventoryVisual.SetParent(transform.root);
		while (elapsedTime < time) {
			inventoryVisual.position = Vector3.Lerp(startingPos, newSlot.position, (elapsedTime / time));
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		inventoryVisual.position = newSlot.position;
		inventoryVisual.SetParent(newSlot);
	}

	#endregion
	// --------------- INVENTORY ---------------
}