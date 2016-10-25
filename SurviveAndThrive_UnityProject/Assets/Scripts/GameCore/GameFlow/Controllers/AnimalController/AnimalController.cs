using UnityEngine;
using System.Collections.Generic;

public class AnimalController : BaseController {

	// --------------- Tweakables ---------------

	private float animalUpdateRange = 60f;

	// --------------- Tweakables ---------------

	public GameItemDatabase _gameItemDatabase { get; private set; }
	public HerdHandler _herdHandler { get; private set; }

	private Transform playerTransform;

	private List<BaseAnimal> animals = new List<BaseAnimal>();

	public override void Initialize() {
		base.Initialize();

		_gameItemDatabase = GameAccesPoint.Instance.mainGameState._gameItemDatabase;
		_herdHandler = new HerdHandler(this);
	}

	public override void UpdateGameTime(float globalGameTime, float deltaGameTime) {
		if (playerTransform == null) {
			playerTransform = GameAccesPoint.Instance.mainGameState._playerController._playerTransform;
			return;
		}

		if (_herdHandler != null) {
			_herdHandler.UpdateHerds();
		}

		if (animals != null && animals.Count > 0) {
			for (int i = 0; i < animals.Count; i++) {
				float distance = Vector3.Distance(playerTransform.position, animals[i].gameObject.transform.position);
				if (distance < animalUpdateRange) {
					animals[i].UpdateGameTime();
				}
			}
		}
	}

	public void SpawnPreyAnimal(string animalID, Vector3 worldLocation) {
		// Spawn one animal
		BaseAnimal tmpAnimal = (BaseAnimal)_gameItemDatabase.CreateItemInstance(animalID, worldLocation, Quaternion.identity);
		animals.Add(tmpAnimal);
	}

	private int i = 0;

	public void SpawnHerd(string herdAnimalID, Vector3 worldLocation, int herdSize) {
		// Spawn a herd of animals of the same type
		if (i % 5 == 0) { // TODO: Change
			_herdHandler.SpawnHerd(herdAnimalID, worldLocation, herdSize);
		}

		i++;
	}

}