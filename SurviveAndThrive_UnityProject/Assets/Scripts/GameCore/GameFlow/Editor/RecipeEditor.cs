using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using KV = System.Collections.Generic.KeyValuePair<GameItemType, int>;

public class RecipeEditor : EditorWindow {

	static EditorWindow window;

	[MenuItem("FourNomads/RecipeEditor")]
	static void Init() {
		window = GetWindow(typeof(RecipeEditor));
		window.minSize = new Vector2(668, 200);
		window.Show();
	}

	string fileName = "recipes";
	GUIStyle style;

	string[] itemTypeNames;

	Texture2D[] icons = new Texture2D[7];
	List<Recipe> recipes = new List<Recipe>();
	List<bool> lockedRecipes = new List<bool>();

	void OnGUI() {
		DrawBackground();
		DrawContent();
		DrawOptions();
	}

	void DrawContent() {
		GUI.Label(new Rect(26, 5, 150, 16), "Output:", style);
		GUI.Label(new Rect(160, 5, 150, 16), "1st Item:", style);
		GUI.Label(new Rect(315, 5, 150, 16), "2nd Item:", style);
		GUI.Label(new Rect(470, 5, 150, 16), "3rd Item:", style);

		if (recipes != null && recipes.Count > 0) {
			for (int i = 0; i < recipes.Count; i++) {
				if (lockedRecipes[i]) {
					DrawLocked(i);
				} else {
					DrawUnlocked(i);
				}

				if (icons[5] != null && icons[6] != null) {
					if (lockedRecipes.Count - 1 < i)
						return;

					Texture2D currentLockIcon = lockedRecipes[i] ? icons[5] : icons[6];

					if (GUI.Button(new Rect(647, 26 + (21 * i), 16, 16), currentLockIcon)) {
						ToggleLocked(i);
					}
				}
			}
		}
    }

	void AddRecipe() {
		recipes.Add(new Recipe(GameItemType.EmptyItem_0, new KV(GameItemType.EmptyItem_0, 0), new KV(GameItemType.EmptyItem_0, 0), new KV(GameItemType.EmptyItem_0, 0)));
		lockedRecipes.Add(false);
    }

	void RemoveRecipe(int index) {
		recipes.RemoveAt(index);
		lockedRecipes.RemoveAt(index);
	}

	void ToggleLocked(int index) {
		lockedRecipes[index] = !lockedRecipes[index];
	}

	void DrawLocked(int i) {
		GUI.enabled = false;
		DrawUnlocked(i);
		GUI.enabled = true;
	}

	void DrawUnlocked(int i) {
		int currentIndex;
		GameItemType newKey;
		int newValue;
		KV newPair;

		GUI.Label(new Rect(5, 26 + (21 * i), 16, 16), i + ".", style);
		currentIndex = EditorGUI.Popup(new Rect(26, 26 + (21 * i), 128, 16), System.Array.IndexOf(itemTypeNames, recipes[i].outputItem.ToString()), itemTypeNames);
		recipes[i].outputItem = (GameItemType)System.Enum.Parse(typeof(GameItemType), itemTypeNames[currentIndex]);

        currentIndex = EditorGUI.Popup(new Rect(160, 26 + (21 * i), 128, 16), System.Array.IndexOf(itemTypeNames, recipes[i].firstItem.Key.ToString()), itemTypeNames);
		newKey = (GameItemType)System.Enum.Parse(typeof(GameItemType), itemTypeNames[currentIndex]); 
		newValue = EditorGUI.IntField(new Rect(289, 26 + (21 * i), 21, 15), recipes[i].firstItem.Value);
		newPair = new KV(newKey, newValue);
		recipes[i].firstItem = newPair;

		currentIndex = EditorGUI.Popup(new Rect(315, 26 + (21 * i), 128, 16), System.Array.IndexOf(itemTypeNames, recipes[i].secondItem.Key.ToString()), itemTypeNames);
		newKey = (GameItemType)System.Enum.Parse(typeof(GameItemType), itemTypeNames[currentIndex]);
		newValue = EditorGUI.IntField(new Rect(444, 26 + (21 * i), 21, 15), recipes[i].secondItem.Value);
		newPair = new KV(newKey, newValue);
		recipes[i].secondItem = newPair;

		currentIndex = EditorGUI.Popup(new Rect(470, 26 + (21 * i), 128, 16), System.Array.IndexOf(itemTypeNames, recipes[i].thirdItem.Key.ToString()), itemTypeNames);
		newKey = (GameItemType)System.Enum.Parse(typeof(GameItemType), itemTypeNames[currentIndex]);
		newValue = EditorGUI.IntField(new Rect(599, 26 + (21 * i), 21, 15), recipes[i].thirdItem.Value);
		newPair = new KV(newKey, newValue);
		recipes[i].thirdItem = newPair;

		if (icons[4] != null) {
			if (GUI.Button(new Rect(626, 26 + (21 * i), 16, 16), icons[4])) {
				RemoveRecipe(i);
			}
		}
	}

	void DrawBackground() {
		GUI.color = new Color(0.49f, 0.60f, 0.34f, 1); // new Color(0.12f, 0.12f, 0.12f, 1) => dark grey
        GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), EditorGUIUtility.whiteTexture);

		GUI.color = Color.white;
	}

	void DrawOptions() {
		GUI.color = new Color(0.36f, 0.36f, 0.36f, 1);
		GUI.Box(new Rect(0, window.position.height - 42, maxSize.x, 42), "");

		GUI.color = Color.white;

		GUI.DrawTexture(new Rect(5, window.position.height - 37, 32, 32), icons[0]);

		if (icons[2] != null) {
			if (GUI.Button(new Rect(42, window.position.height - 37, 32, 32), icons[2])) {
				SaveRecipes();
			}
		}

		if (icons[1] != null) {
			if (GUI.Button(new Rect(82, window.position.height - 37, 32, 32), icons[1])) {
				LoadRecipes();
			}
		}

		GUI.Label(new Rect(122, window.position.height - 29, 72, 16), "File Name:");
		fileName = GUI.TextField(new Rect(194, window.position.height - 29, 120, 16), fileName, 32);

		if (icons[3] != null) {
			if (GUI.Button(new Rect(window.position.width - 37, window.position.height - 37, 32, 32), icons[3])) {
				AddRecipe();
			}
		}
	}

	void SaveRecipes() {
		bool save = EditorUtility.DisplayDialog("Overwrite file?", "Are you sure that you want to overwrite a possibly excisting file?", "Yes", "Cancel");
		if (save) {
			XMLManager.XMLWrite(recipes, fileName, "Resources/XML", null);
		}
	}

	void LoadRecipes() {
		AssetDatabase.Refresh();

		recipes = XMLManager.XMLReadFromResources<List<Recipe>>(fileName);
		if (recipes == null) {
			recipes = new List<Recipe>();
		}

		for (int i = 0; i < recipes.Count; i++) {
			lockedRecipes.Add(true);
		}
	}

	void OnEnable() {
		icons[0] = (Texture2D)Resources.Load("EditorIcons/fn-logo", typeof(Texture2D));
		icons[1] = (Texture2D)Resources.Load("EditorIcons/load-icon", typeof(Texture2D));
		icons[2] = (Texture2D)Resources.Load("EditorIcons/save-icon", typeof(Texture2D));
		icons[3] = (Texture2D)Resources.Load("EditorIcons/add-icon", typeof(Texture2D));
		icons[4] = (Texture2D)Resources.Load("EditorIcons/remove-icon", typeof(Texture2D));
		icons[5] = (Texture2D)Resources.Load("EditorIcons/locked-icon", typeof(Texture2D));
		icons[6] = (Texture2D)Resources.Load("EditorIcons/unlocked-icon", typeof(Texture2D));

		style = new GUIStyle();
		style.normal.textColor = Color.white;

		itemTypeNames = System.Enum.GetNames(typeof(GameItemType));
		System.Array.Sort(itemTypeNames);

		LoadRecipes();
	}
}