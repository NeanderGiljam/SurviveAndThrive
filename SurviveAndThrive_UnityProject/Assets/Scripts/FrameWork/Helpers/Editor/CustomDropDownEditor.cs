using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(SortedEnum))]
public class CustomDropDownEditor : PropertyDrawer {

	private int currentIndex = -1;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		if (property.enumNames != null) {
			string[] enumNames = property.enumNames;
            System.Array.Sort(enumNames);

			if (currentIndex == -1) {
				for (int i = 0; i < enumNames.Length; i++) {
					if (enumNames[i] == property.enumNames[property.enumValueIndex]) {
						currentIndex = i;
					}
				}
			}

			currentIndex = EditorGUI.Popup(position, label.text, currentIndex, enumNames);

			for (int i = 0; i < property.enumNames.Length; i++) {
				if (enumNames[currentIndex] == property.enumNames[i]) {
					property.enumValueIndex = i;
                }
			}
		}
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		return EditorGUI.GetPropertyHeight(property, label, true);
	}
}

[CustomPropertyDrawer(typeof(GameItemMenuEnum))]
public class CustomGameItemTypeEditor : PropertyDrawer {

	private int currentIndex = -1;
	private Dictionary<string, string> itemCategories = new Dictionary<string, string>();

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		if (itemCategories == null || itemCategories.Count <= 0) {
			Array enumValues = Enum.GetValues(typeof(GameItemType));

			for (int i = 0; i < enumValues.Length; i++) {
				string itemName = enumValues.GetValue(i).ToString();
				string itemCategory = GetCategory(itemName, "_");

				if (itemCategory != null) {
					itemCategories.Add(itemName, itemCategory);
				}
			}

			currentIndex = property.enumValueIndex;
		}

		Event evt = Event.current;

		if (evt.type == EventType.ContextClick) {
			if (evt.mousePosition.x > position.xMin && evt.mousePosition.x < position.xMax) {
				if (evt.mousePosition.y > position.yMin && evt.mousePosition.y < position.yMax) {
					GenericMenu menu = new GenericMenu();

					foreach (KeyValuePair<string, string> pair in itemCategories) {
						object[] context = new object[] { pair.Key, property };
						menu.AddItem(new GUIContent(pair.Value + "/" + pair.Key), false, Callback, context);
					}

					menu.AddSeparator("");
					menu.AddItem(new GUIContent("Add new item"), false, OpenItemsScript);

					menu.ShowAsContext();

					evt.Use();
				}
			}
		}

		currentIndex = EditorGUI.Popup(position, label.text, currentIndex, property.enumNames);
		property.enumValueIndex = currentIndex;
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		return EditorGUI.GetPropertyHeight(property, label, true);
	}

	public void Callback(object obj) {
		string key = (obj as object[])[0] as string;
		SerializedProperty property = (obj as object[])[1] as SerializedProperty;
		for (int i = 0; i < property.enumNames.Length; i++) {
			if (property.enumNames[i] == key) {
				currentIndex = i;
			}
		}
	}

	public void OpenItemsScript() {
		UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(Application.dataPath + "/Scripts/GameCore/Globals/GameItemEnum.cs", 3);
	}

	private string GetCategory(string fullName, string seperator) {
		int index = fullName.IndexOf("_");

		if (index > 0) {
			return fullName.Substring(0, index);
		}

		return null;
	}

}