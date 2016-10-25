using UnityEngine;
using UnityEditor;
using System.Collections;

[InitializeOnLoad]
[CustomEditor(typeof(GameItemDatabase))]
public class GameItemDatabaseEditor : Editor {

	static GameItemDatabase gameItemDatabase;

	static GameItemDatabaseEditor() {
		EditorApplication.playmodeStateChanged += StateChanged;
		gameItemDatabase = Resources.Load("Prefabs/SystemPrefabs/GameItemDatabase", typeof(GameItemDatabase)) as GameItemDatabase;
	}

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (GUILayout.Button("Load Game Items")) {
			gameItemDatabase.LoadItems();
		}

		if (GUILayout.Button("Save Game Items")) {
			gameItemDatabase.SaveItemList();
		}
	}

	static void StateChanged() {
		if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode) {
			gameItemDatabase.LoadItems();
			gameItemDatabase.SaveItemList();
			AssetDatabase.Refresh();
		}
	}

}