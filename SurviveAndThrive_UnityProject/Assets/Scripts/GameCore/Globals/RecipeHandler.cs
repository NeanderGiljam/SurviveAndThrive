using System.Collections.Generic;

using KV = System.Collections.Generic.KeyValuePair<GameItemType, int>;

public class RecipeHandler {

	private List<Recipe> recipes = new List<Recipe>() {
		// Recipe -> Makes:							From:										And:										And:	
		new Recipe(GameItemType.Tool_Axe_Basic,		new KV(GameItemType.Resource_Stick, 1), new KV(GameItemType.Resource_Stone, 1), new KV(GameItemType.EmptyItem_0, 0)),
		new Recipe(GameItemType.Tool_PickAxe_Basic, new KV(GameItemType.Resource_Stick, 2), new KV(GameItemType.Resource_Stone, 1), new KV(GameItemType.EmptyItem_0, 0)),
		new Recipe(GameItemType.Weapon_Spear_Basic, new KV(GameItemType.Resource_Stick, 1), new KV(GameItemType.Resource_Stone, 2), new KV(GameItemType.EmptyItem_0, 0))
	};

	private Dictionary<GameItemType, int> itemTypes = new Dictionary<GameItemType, int>();
	private Dictionary<Recipe, bool> creatableRecipes = new Dictionary<Recipe, bool>();

	public void Initialize() {
		recipes = XMLManager.XMLReadFromResources<List<Recipe>>("recipes");
		if (recipes == null) {
			recipes = new List<Recipe>();
		}
	}

	public Dictionary<Recipe, bool> CheckInventoryForRecipes(BasePickableItem[] craftItems) { // Dictionary<Transform, InventoryItem> inventory
		itemTypes.Clear();
		itemTypes.Add(GameItemType.EmptyItem_0, 0);

		for (int i = 0; i < craftItems.Length; i++) {
			if (craftItems[i] != null) {
				GameItemType type = craftItems[i].itemType;

				if (itemTypes.ContainsKey(type)) {
					itemTypes[type] += 1;
				} else {
					itemTypes.Add(type, 1);
				}
			}
		}

		foreach (Recipe r in recipes) {
			bool hasPrereq = false;

			if (itemTypes.ContainsKey(r.firstItem.Key) && itemTypes[r.firstItem.Key] >= r.firstItem.Value) {
				if (itemTypes.ContainsKey(r.secondItem.Key) && itemTypes[r.secondItem.Key] >= r.secondItem.Value) {
					if (itemTypes.ContainsKey(r.thirdItem.Key) && itemTypes[r.thirdItem.Key] >= r.thirdItem.Value) {
						hasPrereq = true;
					}
				}
			}

			if (!creatableRecipes.ContainsKey(r)) {
				creatableRecipes.Add(r, hasPrereq);
			} else {
				creatableRecipes[r] = hasPrereq;
			}
        }

		return creatableRecipes;
	}

	public string GetGameItemId(GameItemType outputItem) {
		string id = ((int)outputItem).ToString();

		if (id.Length > 1) {
			string itNumber = id.Remove(3);
			string idNumer = id.Substring(3, id.Length - 3);
			id = idNumer + ":" + itNumber;
			return id;
		}

		return "No id for recipe item";
	}

}

public class Recipe {

	public GameItemType outputItem;
	public KeyValuePair<GameItemType, int> firstItem;
	public KeyValuePair<GameItemType, int> secondItem;
	public KeyValuePair<GameItemType, int> thirdItem;

	public Recipe() { }
	public Recipe(GameItemType outputItem, KeyValuePair<GameItemType, int> firstItem, KeyValuePair<GameItemType, int> secondItem, KeyValuePair<GameItemType, int> thirdItem) {
		this.outputItem = outputItem;
        this.firstItem = firstItem;
		this.secondItem = secondItem;
		this.thirdItem = thirdItem;
	}

}