using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BaseItem), true)]
public class ItemEditor : Editor {

	private BaseItem item;
	private GUIStyle style = new GUIStyle();

	private void OnEnable() {
		item = (target as BaseItem);

		string id = ((int)item.itemType).ToString();

		if (id.Length > 1) {
			string itNumber = id.Remove(3);
			string idNumer = id.Substring(3, id.Length - 3);
			id = idNumer + ":" + itNumber;
		}

		(target as BaseItem).SetID(id);
	}

	public override void OnInspectorGUI() {
		GUILayout.Label("BaseItem ID: " + (target as BaseItem)._id, style);

		base.OnInspectorGUI();
	}
}