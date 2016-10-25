using UnityEngine;
using System.Collections.Generic;

public class HerdHandler {

	// --------------- Tweakables ---------------

	private float herdUpdateRange = 60f;

	// --------------- Tweakables ---------------

	private AnimalController parent;
	private Transform playerTransform;

	private List<Herd> herds = new List<Herd>();

	public HerdHandler(AnimalController parent) {
		this.parent = parent;
	}

	public void SpawnHerd(string herdAnimalID, Vector3 worldLocation, int herdSize) {
		if (playerTransform == null) {
			playerTransform = GameAccesPoint.Instance.mainGameState._playerController._playerTransform;
		}

		BaseHerdAnimal animalType = (BaseHerdAnimal)parent._gameItemDatabase.GetItem(herdAnimalID);
		Herd newHerd = new Herd(this, animalType, worldLocation, herdSize);
		herds.Add(newHerd);
	}

	public void RemoveHerd(Herd herd) {
		herds.Remove(herd);
	}

	public void UpdateHerds() {
		if (herds == null || herds.Count <= 0) {
			return;
		}

		for (int i = 0; i < herds.Count; i++) {
			float distance = Vector3.Distance(playerTransform.position, herds[i]._currentHerdLocation.position);
			if (distance < herdUpdateRange) {
				herds[i].UpdateHerd();
			}
		}
	}

}

[System.Serializable]
public class Herd {

	public Transform _currentHerdLocation { get; private set; }
	public Vector3 _startLocation { get; private set; }

	public BaseHerdAnimal _herdLeader { get; private set; }
	public List<BaseHerdAnimal> herdAnimals = new List<BaseHerdAnimal>();

	private HerdHandler parent;

	public Herd() { }
	public Herd(HerdHandler parent, BaseHerdAnimal animalType, Vector3 worldLocation, int herdSize) {
		this.parent = parent;

		_startLocation = worldLocation;

		for (int i = 0; i < herdSize; i++) {
			Vector3 spawnPos = new Vector3(worldLocation.x + i + 1, worldLocation.y, worldLocation.z);
			BaseHerdAnimal newHerdAnimal = Object.Instantiate(animalType);
			newHerdAnimal.OnCreate(spawnPos, Quaternion.identity);
			newHerdAnimal.AssignHerd(this);
			herdAnimals.Add(newHerdAnimal);
		}

		SetLeader();
	}

	public void RemoveHerdAnimal(BaseHerdAnimal herdAnimal) {
		herdAnimals.Remove(herdAnimal);

		//Debug.Log("Herd animals count: " + herdAnimals.Count);
	}

	public void SetLeader() {
		if (herdAnimals.Count <= 0) {
			parent.RemoveHerd(this);
			return;
		}

		_herdLeader = herdAnimals[0];
		herdAnimals[0].SetLeaderStatus(true);

		_currentHerdLocation = _herdLeader.transform;
	}

	public void UpdateHerd() {
		if (herdAnimals.Count <= 0) {
			return;
		}

		int i = herdAnimals.Count;
		while (--i > -1) {
			herdAnimals[i].UpdateGameTime();
		}

		//foreach (BaseHerdAnimal animal in herdAnimals) {
		//	animal.UpdateGameTime();
		//}
	}
}