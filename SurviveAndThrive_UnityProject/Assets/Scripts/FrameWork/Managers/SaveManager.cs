using UnityEngine;
using System.Collections.Generic;

public delegate void OnSaveGame();

public class SaveManager : BaseManager {

	public event OnSaveGame onSaveGame;

	// --------------- Singleton Pattern ---------------
	private static SaveManager instance;

	public static SaveManager Instance {
		get {
			instance = FindObjectOfType<SaveManager>();

			if (instance == null) {
				instance = new GameObject("SaveManager").AddComponent<SaveManager>();
			}
			return instance;
		}
	}
	// --------------- Singleton Pattern ---------------

	private List<SavableData> saveData;

	private System.Type[] extraTypes = new System.Type[] {  
		typeof(PlayerState),
		typeof(SavableInventory)
	};

	public override void Initialize() {
		GameAccesPoint.Instance.managerSystem.saveManager = this;
	}

	public void SaveGameState() {
		saveData = new List<SavableData>();

		if (onSaveGame != null)
			onSaveGame();

		if (saveData.Count > 0) {
			try {
				XMLManager.XMLWrite(saveData, "Save" + GameAccesPoint.Instance.mainGameState._currentSessionIndex, extraTypes);
			} catch (System.Exception e) {
				Debug.LogError("Could not save the game! Error: " + e.Message);
			}
		} else {
			Debug.LogWarning("No data to save!");
		}
	}

	public void LoadGameState(bool newGame) {
		if (newGame) {
			saveData = new List<SavableData>();
			return;
		}

		List<SavableData> newSaveData = XMLManager.XMLRead<List<SavableData>>("Save" + GameAccesPoint.Instance.mainGameState._currentSessionIndex, extraTypes);
		if (newSaveData != null) {
			saveData = newSaveData;
		} else {
			saveData = new List<SavableData>();
			Debug.LogWarning("Nothing was loaded");
		}
	}

	public void AddSaveData(SavableData myData) {
		saveData.Add(myData);
	}

	public SavableData GetSaveData(SavableIdentifier source) {
		if (saveData != null) {
			foreach (SavableData d in saveData) {
				if (d.dataSource.Equals(source))
					return d;
			}
		}

		return null;
	}
}

[System.Serializable]
public class SavableData {

	public SavableIdentifier dataSource;
	public object[] saveData;

	public SavableData() { }
	public SavableData(SavableIdentifier dataSource, object[] saveData) {
		this.dataSource = dataSource;
		this.saveData = saveData;
	}

}

[System.Serializable]
public enum SavableIdentifier {
	Player,
	WorldController
}

public interface ISavable {

	bool LoadData();
	void SaveData();

}