using UnityEngine;
using System.Collections.Generic;

public class GameItemDatabase : BaseController {

	[System.Serializable]
	public class BiomeData {
		public BiomeType biomeType;
		public Texture2D biomeTexture;
		public Color biomeColor;
	}

	public List<BiomeData> biomeData = new List<BiomeData>();

	public BaseItem defaultItem;
	public List<BaseItem> gameItems = new List<BaseItem>();

	private Dictionary<string, BaseItem> gameItemsDictionary = new Dictionary<string, BaseItem>();
	private Dictionary<BiomeType, BiomeItemsDatabase> biomeItemsDictionary = new Dictionary<BiomeType, BiomeItemsDatabase>();

	public override void Initialize() {
		LoadItemsFromXML();
		SetGameItemDictionary();
	}

	public BaseItem CreateItem(GameItemType itemType, Vector3 position, Quaternion rotation, Transform parent = null) {
		BaseItem item = GetItem(itemType);
		BaseItem newItem = null;

        if (item != null) {
			newItem = Instantiate(item, position, rotation) as BaseItem;
			if (parent != null) {
				newItem.transform.SetParent(parent);
			}
			newItem.Initialize();
		}

		return newItem;
	}

	public BaseItem CreateItemInstance(string itemId, Vector3 position, Quaternion rotation, Transform parent = null) {
		if (gameItems == null || gameItems.Count <= 0) {
			Debug.LogError("No items in database");
			return null;
		}

		if (gameItemsDictionary.ContainsKey(itemId)) {
			BaseItem item = null;
			gameItemsDictionary.TryGetValue(itemId, out item);
			if (item != null) {
				BaseItem instance = Instantiate(item);
				instance.OnCreate(position, rotation, parent);
				return instance;
			}
		}

		Debug.LogWarning("The object with id: " + itemId + " could not be found. Please check if the object has an item script with the correct settings and that it is a valid item in the items enum!");

		return defaultItem;
	}

	public BaseItem GetItem(string id) {
		if (gameItems == null || gameItems.Count <= 0) {
			Debug.LogError("No items in database");
			return null;
		}

		if (gameItemsDictionary.ContainsKey(id)) {
			BaseItem item = null;
			gameItemsDictionary.TryGetValue(id, out item);
			if (item != null) {
				return item;
			}
		}

		Debug.LogWarning("The object with id: " + id + " could not be found. Please check if the object has an item script with the correct settings and that it is a valid item in the items enum!");

		return null;
	}

	public BaseItem GetItem(GameItemType itemType) {
		if (gameItems == null || gameItems.Count <= 0) {
			Debug.LogError("No items in database");
			return null;
		}

		foreach (BaseItem i in gameItems) {
			if (i.itemType == itemType) {
				return i;
			}
		}

		Debug.LogWarning("The object: " + itemType.ToString() + " could not be found. Please check if the object has an item script with the correct settings and that it is a valid item in the items enum!");

		return null;
	}

	public BiomeItemsDatabase GetBiomeItems(BiomeType type) {
		foreach (KeyValuePair<BiomeType, BiomeItemsDatabase> pair in biomeItemsDictionary) {
			if (pair.Key == type) {
				return pair.Value;
			}
		}

		return null;
	}

	private void SortBiomeItems() {
		if (gameItems == null || gameItems.Count <= 0) {
			Debug.LogWarning("Could not sort biome items");
			return;
		}

		string[] biomeTypes = System.Enum.GetNames(typeof(BiomeType));

		for (int i = 0; i < biomeTypes.Length; i++) {
			BiomeItemsDatabase newBiomeItems = new BiomeItemsDatabase();
			foreach (BaseItem item in gameItems) {
				if (item.biomeTypes == null || item.biomeTypes.Length <= 0) {
					continue;
				}

				for (int j = 0; j < item.biomeTypes.Length; j++) {
					if (biomeTypes[i] == item.biomeTypes[j].ToString()) {
						string itemBaseID = item._id.Substring(0, 2);
						if (itemBaseID == "13") {
							newBiomeItems.trees.Add(item);
							//newBiomeItems.biomeObjectTypes[0].Add(item);
						} else if (itemBaseID == "12") {
							newBiomeItems.rocks.Add(item);
							//newBiomeItems.biomeObjectTypes[1].Add(item);
						} else if (itemBaseID == "11") {
							newBiomeItems.plants.Add(item);
							//newBiomeItems.biomeObjectTypes[2].Add(item);
						} else if (itemBaseID == "10") {
							newBiomeItems.grasses.Add(item);
							//newBiomeItems.biomeObjectTypes[3].Add(item);
						} else if (itemBaseID == "1") {
							newBiomeItems.animals.Add(item);
							//newBiomeItems.biomeObjectTypes[4].Add(item);
						} else if (itemBaseID == "14") {
							newBiomeItems.foods.Add(item);
							//newBiomeItems.biomeObjectTypes[5].Add(item);
						} else if (itemBaseID == "17") {
							newBiomeItems.resources.Add(item);
						}
					}
				}
			}
			biomeItemsDictionary.Add((BiomeType)System.Enum.Parse(typeof(BiomeType), biomeTypes[i]), newBiomeItems);
		}
	}

	// --------------- LOADING AND SAVING DATABASE ---------------
	// --------------- LOADING AND SAVING DATABASE ---------------

	#region LOADING AND SAVING DATABASE

	public void LoadItems() {
		gameItems.Clear();

		ResourceLoader loader = new ResourceLoader();
		List<string> itemPaths = loader.GetAllResourcesPathsByType<BaseItem>();

		foreach (string path in itemPaths) {
			BaseItem item = Resources.Load(path, typeof(BaseItem)) as BaseItem;

			if (item.excludeFromBuild) { // TODO: Excludes items that are marked as exclude, on build remove these items from resources 
				continue;
			}

			item.SetPath(path);
			gameItems.Add(item);
		}

		SetItemIDs();
	}

	public void SaveItemList() {
		List<ItemState> itemStates = new List<ItemState>();

		foreach (BaseItem item in gameItems) {
			ItemState itemState = new ItemState();
			itemState.SetState(item._id, item.itemType, item.biomeTypes, item._itemPath, item.minSize, item.maxSize, item.uniformXZ, item.needsCollider); // TODO: Test if breaks -> changes prefab value
			itemStates.Add(itemState);
		}

		XMLManager.XMLWrite(itemStates, "GameItemStates", "Resources/XML", null);
	}

	private void LoadItemsFromXML() {
		gameItems.Clear();

		List<ItemState> itemStates = XMLManager.XMLReadFromResources<List<ItemState>>("GameItemStates");

		foreach (ItemState state in itemStates) {
			BaseItem item = Resources.Load(state.itemPath, typeof(BaseItem)) as BaseItem;
			item.SetItemState(state.id, state.itemPath, state.itemType, state.biomeTypes, state.minSize, state.maxSize, state.uniformXZ, state.needsCollider);
			gameItems.Add(item);
		}
	}

	private void SetItemIDs() {
		foreach (BaseItem i in gameItems) {
			string id = ((int)i.itemType).ToString();

			if (id.Length > 1) {
				string itNumber = id.Remove(3);
				string idNumer = id.Substring(3, id.Length - 3);
				id = idNumer + ":" + itNumber;
			}

			i.SetID(id);
		}
	}

	private void SetGameItemDictionary() {
		if (gameItems == null || gameItemsDictionary == null) {
			return;
		}

		foreach (BaseItem item in gameItems) {
			if (!gameItemsDictionary.ContainsKey(item._id)) {
				gameItemsDictionary.Add(item._id, item);
			} else {
				Debug.LogWarning("The item with id: " + item._id + " is a duplicate, pleas make sure only one item of this type excists!");
			}
		}

		SortBiomeItems();
	}

	#endregion

	// --------------- LOADING AND SAVING DATABASE ---------------
	// --------------- LOADING AND SAVING DATABASE ---------------
}

public class ItemState {

	public string id;
	public string itemPath;

	public bool uniformXZ;
	public bool needsCollider;

	public Vector3 minSize;
	public Vector3 maxSize;

	public GameItemType itemType;
	public BiomeType[] biomeTypes;

	public void SetState(string id, GameItemType itemType, BiomeType[] biomeTypes, string itemPath, Vector3 minSize, Vector3 maxSize, bool uniformXZ, bool needsCollider) {
		this.id = id;
		this.itemType = itemType;
		this.biomeTypes = biomeTypes;
		this.minSize = minSize;
		this.maxSize = maxSize;
		this.uniformXZ = uniformXZ;
		this.itemPath = itemPath;
		this.needsCollider = needsCollider;
	}

	public ItemState GetState() {
		return this;
	}

}

public class BiomeItemsDatabase {

	public List<BaseItem> trees = new List<BaseItem>();
	public List<BaseItem> rocks = new List<BaseItem>();
	public List<BaseItem> plants = new List<BaseItem>();
	public List<BaseItem> grasses = new List<BaseItem>();
	public List<BaseItem> animals = new List<BaseItem>();
	public List<BaseItem> foods = new List<BaseItem>();
	public List<BaseItem> resources = new List<BaseItem>();

	public BaseItem[] GetItemArrayByType(SpawnSettingType type) {
		switch (type) {
			case SpawnSettingType.Tree:
				return trees.ToArray();
			case SpawnSettingType.Stone:
				return rocks.ToArray();
			case SpawnSettingType.Plant:
				return plants.ToArray();
			case SpawnSettingType.Grass:
				return grasses.ToArray();
			case SpawnSettingType.Life:
				return animals.ToArray();
			case SpawnSettingType.Food:
				return foods.ToArray();
			case SpawnSettingType.Resources:
				return resources.ToArray();
			default:
				Debug.LogError("Database does not yet contain the selected spawntype");
				return null;
		}
	}

}