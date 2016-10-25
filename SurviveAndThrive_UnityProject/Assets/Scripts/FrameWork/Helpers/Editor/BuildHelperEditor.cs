using UnityEngine;
using UnityEditor;
using System.Collections;

public class BuildHelperEditor : Editor {

	static GameItemDatabase gameItemDatabase;

	[MenuItem("File/BuildForWindows")]
	static void BuildWin() {

		gameItemDatabase = UnityEngine.Resources.Load("Prefabs/SystemPrefabs/GameItemDatabase", typeof(GameItemDatabase)) as GameItemDatabase;

		gameItemDatabase.LoadItems();
		gameItemDatabase.SaveItemList();

		string[] levels = new string[] {
			"Assets/Scenes/EntryScene.unity",
			"Assets/Scenes/MainMenuScene.unity",
			"Assets/Scenes/MainGameScene.unity"
		};

		string fileName = EditorUtility.SaveFilePanel("Destination", "", "", ".exe");
		BuildPipeline.BuildPlayer(levels, fileName, BuildTarget.StandaloneWindows, BuildOptions.None);
	}

	[MenuItem("File/BuildForMac")]
	static void BuildMac() {

		gameItemDatabase = UnityEngine.Resources.Load("Prefabs/SystemPrefabs/GameItemDatabase", typeof(GameItemDatabase)) as GameItemDatabase;

		gameItemDatabase.LoadItems();
		gameItemDatabase.SaveItemList();

		string[] levels = new string[] {
			"Assets/Scenes/EntryScene.unity",
			"Assets/Scenes/MainMenuScene.unity",
			"Assets/Scenes/MainGameScene.unity"
		};

		string fileName = EditorUtility.SaveFilePanel("Destination", "", "", ".app");
		BuildPipeline.BuildPlayer(levels, fileName, BuildTarget.StandaloneOSXUniversal, BuildOptions.None);
	}

}