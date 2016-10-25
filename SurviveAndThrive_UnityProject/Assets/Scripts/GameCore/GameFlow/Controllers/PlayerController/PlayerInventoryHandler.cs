using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public delegate void OnInventoryChanged(BaseItem changedItem);

public class PlayerInventoryHandler : MonoBehaviour, IHandler {

	//public event OnInventoryChanged onInventoryChanged;

	public Dictionary<Transform, InventoryItem> _inventory { get; private set; } // Is created by the HudGUIState!

	// ----- [Crafting] ----- //
	private int recipeIndex = 1;
	private GameObject currentTmpVisual;
	private Transform craftSlot;
	// ----- [Crafting] ----- //

	private GameObject inventoryVisual;
	private Transform dropAnchor;

	private Player player;
	private WorldController worldController;
	private RecipeHandler recipeHandler;

	private List<GameItemType> creatableItems = new List<GameItemType>();
	private Dictionary<Recipe, bool> currentCreatableRecipes = new Dictionary<Recipe, bool>();

	public void Initialize(Object obj) {
		player = obj as Player;

		dropAnchor = GameObject.Find("r_DropAnchor").transform; // TODO: Make safer
		inventoryVisual = Resources.Load("Prefabs/GUIPrefabs/InventoryVisual") as GameObject;

		GameAccesPoint.Instance.hudGUIState.AddPlayerInventoryHandler(this);
		worldController = GameAccesPoint.Instance.mainGameState._worldController;

		recipeHandler = new RecipeHandler();
		recipeHandler.Initialize();

		_inventory = GameAccesPoint.Instance.hudGUIState.GetInitialInventory();
		GameAccesPoint.Instance.hudGUIState.ClearInventory();

		foreach (KeyValuePair<Transform, InventoryItem> pair in _inventory) {
			if (pair.Key.name == "CraftOutputSlot") {
				craftSlot = pair.Key;
            }
		}

		if (player._currentPlayerState != null) {
			RecreateInventory(player._currentPlayerState.savableInventory);
		}
	}

	public bool AddItem(BasePickableItem item, Sprite icon, int slotIndex = -1) {
		bool isTool = false;
		if (item.GetComponent<BaseTool>()) {
			isTool = true;
		}

		Transform slot = null;
        if (slotIndex == -1) {
			slot = isTool ? CheckToolSlot() : FindEmptySlot();
		} else {
			slot = FindSlotByIndex(slotIndex);
			//if (slot == null) {
			//	slot = isTool ? CheckToolSlot() : FindEmptySlot();
			//}
		}

		if (slot != null) {
			GameObject newVisual = Instantiate(inventoryVisual);
			InventoryItem newItem = new InventoryItem(item, newVisual);
			Image visualsIcon = newVisual.GetComponent<Image>();

			_inventory[slot] = newItem;
			visualsIcon.sprite = icon;

			newVisual.transform.SetParent(slot);
			newVisual.transform.position = slot.position;
			newVisual.transform.localScale = Vector3.one;

            if (worldController != null && worldController._currentChunk != null) {
				GameAccesPoint.Instance.mainGameState._worldController._currentChunk.RemoveChangeableItem(item.gameObject);
			}
			item.transform.parent = null;

			AudioManager.Instance.PlayByName("inventory_pling_placeholder", .2f);

			UpdateTool(item, slot);

			return true;
		}

		return false;
	}

	public bool MoveItem(Transform oldSlot, Transform newSlot) {
		if (oldSlot.name == "CraftOutputSlot") {
			CreateCraftedItem(oldSlot);
		}

		InventoryItem invItem = _inventory[oldSlot];

		if (invItem == null || invItem.item == null) {
			Debug.LogError("The moved item got lost!");
			RemoveItem(oldSlot);
			return false;
		}

		if (newSlot.name == "ToolSlot") {
			if (!invItem.item.GetComponent<BaseTool>()) {
				//Debug.Log("Not a tool!");
				return false;
			}
		}

		_inventory[newSlot] = invItem;
		_inventory[oldSlot] = null;

		CheckCraftable();
		UpdateTool(invItem.item, newSlot, oldSlot);

		return true;
	}

	public bool SwapItems(Transform currentItemSlot, Transform swapItemSlot) {
		InventoryItem currentItem = _inventory[currentItemSlot];
		InventoryItem swapItem = _inventory[swapItemSlot];

		if (currentItem == null || swapItem == null || swapItemSlot.name == "CraftOutputSlot") {
			return false;
		}

		if (swapItemSlot.name == "ToolSlot") {
			if (!currentItem.item.GetComponent<BaseTool>()) {
				Debug.Log("Not a tool!");
				return false;
			}
		}

		_inventory[currentItemSlot] = swapItem;
		_inventory[swapItemSlot] = currentItem;

		CheckCraftable();
		UpdateTool(swapItem.item, swapItemSlot, currentItemSlot);

		return true;
	}

	public bool UseInventoryItem(Transform parentSlot) {
		InventoryItem item = null;
		_inventory.TryGetValue(parentSlot, out item);

		if (item != null && item.item != null) {
			BaseConsumableItem consumeItem = item.item.GetComponent<BaseConsumableItem>();
			if (consumeItem != null) {
				RemoveItem(parentSlot);
				consumeItem.Consume(player.statsHandler);
				return true;
			}
		}

		return false;
	}

	public void RemoveItem(Transform parentSlot) {
		UpdateTool(_inventory[parentSlot].item, null);

		if (_inventory[parentSlot].inventoryVisual != null) {
			Destroy(_inventory[parentSlot].inventoryVisual);
		}

		_inventory[parentSlot] = null;
		CheckCraftable();
	}

	public bool DropItem(Transform startParent) {
		// TODO: Play drop animation
		if (startParent.name == "CraftOutputSlot") {
			CreateCraftedItem(startParent);
		}

		Ray ray = new Ray(dropAnchor.position, -Vector3.up);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 10f, (1 << 8))) {
			InventoryItem invItem = _inventory[startParent];

			if (invItem == null || invItem.item == null || invItem.item.gameObject == null)
				return false;

			GameAccesPoint.Instance.mainGameState._worldController._currentChunk.AddChangeableItem(invItem.item.gameObject);

			invItem.item.transform.position = hitInfo.point;
			invItem.item.gameObject.name = "Dropped obj: " + invItem.item.gameObject.name;
			invItem.item.SetVisualState(true);

			RemoveItem(startParent);
			return true;
		}

		return false;
	}

	private Transform CheckToolSlot() {
		Transform emptySlot = null;

		foreach (KeyValuePair<Transform, InventoryItem> pair in _inventory) {
			if (pair.Key.name == "ToolSlot") {
				if (pair.Key.childCount <= 0) {
					emptySlot = pair.Key;
				}
			}
		}

		if (emptySlot == null) {
			emptySlot = FindEmptySlot();
		}

		return emptySlot;
	}

	private Transform FindEmptySlot() {
		Transform emptySlot = null;

		foreach (KeyValuePair<Transform, InventoryItem> pair in _inventory) {
			if (pair.Key.childCount <= 0) {
				if (pair.Key.name == "ToolSlot" || pair.Key.name == "CraftSlot" || pair.Key.name == "CraftOutputSlot") {
					continue;
				}

				emptySlot = pair.Key;
				break;
			}
		}

		if (emptySlot == null) {
			Debug.LogWarning("No empty slot!");
		}

		return emptySlot;
	}

	private Transform FindSlotByIndex(int index) {
		//Transform slotsParent = GameAccesPoint.Instance.hudGUIState.transform.FindChild("Inventory/InventorySlots");
		//int childCount = slotsParent.childCount;

		//for (int i = 0; i < childCount; i++) {
		//	if (i == index) {
		//		return slotsParent.GetChild(i);
		//	}
		//}

		int i = 0;

		foreach (KeyValuePair<Transform, InventoryItem> pair in _inventory) {
			i++;
			if (i == index) {
				if (pair.Key.childCount <= 0) {
					return pair.Key;
				}
			}
		}

		return null;
	}

	private void UpdateTool(BaseItem item, Transform newSlot, Transform oldSlot = null) {
		if (item is BaseTool) {
			BaseTool tool = item as BaseTool;

			if (newSlot != null && newSlot.name == "ToolSlot") {
				tool.Equip();
			} else if (oldSlot != null && oldSlot.name == "ToolSlot") {
				tool.UnEquip();
			}
		}
	}

	private void CheckCraftable() {
		List<BasePickableItem> craftItems = new List<BasePickableItem>();

		foreach (KeyValuePair<Transform, InventoryItem> pair in _inventory) {
			if (pair.Key.name == "CraftSlot") {
				if (pair.Value != null && pair.Value.item != null) {
					craftItems.Add(pair.Value.item);
				}
			}
		}

		currentCreatableRecipes = recipeHandler.CheckInventoryForRecipes(craftItems.ToArray());

		creatableItems.Clear();

		foreach (KeyValuePair<Recipe, bool> pair in currentCreatableRecipes) {
			if (pair.Value && GameAccesPoint.Instance.mainGameState._gameItemDatabase.GetItem(pair.Key.outputItem) != null) {
				creatableItems.Add(pair.Key.outputItem);
			}
		}

		recipeIndex = 1;

		if (creatableItems.Count > 0) {
			GameAccesPoint.Instance.hudGUIState.SetRecipeCounter(recipeIndex + "/" + creatableItems.Count);
		} else {
			GameAccesPoint.Instance.hudGUIState.SetRecipeCounter("0/0");
		}

		SetCurrentRecipeVisual();
	}

	public void SwitchRecipeVisual(int direction) {
		recipeIndex += direction;
		if (recipeIndex > creatableItems.Count) {
			recipeIndex = 1;
		} else if (recipeIndex < 1) {
			recipeIndex = creatableItems.Count;
		}

		if (creatableItems.Count > 0) {
			GameAccesPoint.Instance.hudGUIState.SetRecipeCounter(recipeIndex + "/" + creatableItems.Count);
		} else {
			GameAccesPoint.Instance.hudGUIState.SetRecipeCounter("0/0");
		}
		
		SetCurrentRecipeVisual();
	}

	private void SetCurrentRecipeVisual() {
		if (craftSlot == null)
			return;

		// Clean old craftable visual
		if (currentTmpVisual != null) {
			Destroy(currentTmpVisual);
		}

		// Create new craft visual
		if (creatableItems.Count > 0) {
			BaseItem tmpItem = GameAccesPoint.Instance.mainGameState._gameItemDatabase.GetItem(creatableItems[recipeIndex - 1]);

			if (tmpItem == null && !(tmpItem is BasePickableItem))
				return;

			currentTmpVisual = Instantiate(inventoryVisual);
			Image visualsIcon = currentTmpVisual.GetComponent<Image>();

			visualsIcon.sprite = (tmpItem as BasePickableItem).myIcon;

			currentTmpVisual.transform.SetParent(craftSlot);
			currentTmpVisual.transform.position = craftSlot.position;
			currentTmpVisual.transform.localScale = Vector3.one;
		}
	}

	private void ConsumeCraftItems() { // TODO: When stacks are implemented fix amounts taken from ingredients
		List<KeyValuePair<GameItemType, int>> ingredients = new List<KeyValuePair<GameItemType, int>>();

		foreach (KeyValuePair<Recipe, bool> pair in currentCreatableRecipes) {
			if (pair.Key.outputItem == creatableItems[recipeIndex - 1]) {
				ingredients.Add(pair.Key.firstItem);
				ingredients.Add(pair.Key.secondItem);
				ingredients.Add(pair.Key.thirdItem);
			}
		}

		foreach (KeyValuePair<Transform, InventoryItem> pair in _inventory.Reverse()) {
			if (pair.Key.name == "CraftSlot") {
				if (pair.Value != null && pair.Value.item != null) {
					foreach (KeyValuePair<GameItemType, int> p in ingredients) {
						if (p.Key == pair.Value.item.itemType) {
							pair.Value.item.Destroy(0);
							RemoveItem(pair.Key);
						}
					}
				}
			}
		}
	}

	private void CreateCraftedItem(Transform newSlot) {
		BasePickableItem item = (BasePickableItem)GameAccesPoint.Instance.mainGameState._gameItemDatabase.CreateItem(creatableItems[recipeIndex - 1], dropAnchor.position, Quaternion.identity);
		item.SetVisualState(false);

		_inventory[newSlot] = new InventoryItem(item, currentTmpVisual);
		
		currentTmpVisual = null;
		ConsumeCraftItems();
	}

	private void DebugInventory() {
		foreach (KeyValuePair<Transform, InventoryItem> pair in _inventory) {
			string keyName = "";
			string valueName = "";

			keyName = pair.Key.name;
			if (pair.Value != null && pair.Value.item != null) {
				valueName = pair.Value.item.name;
			}

			Debug.Log("Slot: " + keyName + " | Item: " + valueName);
		}
	}

	// --------------- LOAD/SAVE ---------------
	#region LOAD/SAVE

	public SavableInventory GetSavableInventory() {
		int index = 0;
		List<int> inventoryKeys = new List<int>();
		List<string> inventoryValues = new List<string>();

		foreach (KeyValuePair<Transform, InventoryItem> pair in _inventory) {
			inventoryKeys.Add(index);

			string id = null;
			if (pair.Value != null) {
				if (pair.Value.item != null) {
					id = pair.Value.item._id;
				}
			}
			inventoryValues.Add(id);

			index++;
		}

		return new SavableInventory(inventoryKeys, inventoryValues);
	}

	private void RecreateInventory(SavableInventory savedInventory) {
		Debug.Log("Recreating inventory");

		List<int> keys = savedInventory.inventoryKeys;
		List<string> values = savedInventory.inventoryValues;

		for (int i = 0; i < keys.Count; i++) {
			if (!string.IsNullOrEmpty(values[i])) {
				BaseItem item = GameAccesPoint.Instance.mainGameState._gameItemDatabase.CreateItemInstance(values[i], Vector3.zero, Quaternion.identity);
				if (item is BasePickableItem) {
					BasePickableItem pickableItem = (BasePickableItem)item;
					pickableItem.SetVisualState(false);
					AddItem(pickableItem, pickableItem.myIcon, i);
				}
			}
		}
	}

	#endregion
	// --------------- LOAD/SAVE ---------------
}

public class InventoryItem { // TODO: Create stack class to hold more then one of each item

	public BasePickableItem item { get; private set; }
	public GameObject inventoryVisual { get; private set; }

	public InventoryItem(BasePickableItem item, GameObject inventoryVisual) {
		this.item = item;
		this.inventoryVisual = inventoryVisual;
	}

}

[System.Serializable]
public class SavableInventory {

	public List<int> inventoryKeys;
	public List<string> inventoryValues;

	public SavableInventory() { }
	public SavableInventory(List<int> inventoryKeys, List<string> inventoryValues) {
		this.inventoryKeys = inventoryKeys;
		this.inventoryValues = inventoryValues;
	}	

}