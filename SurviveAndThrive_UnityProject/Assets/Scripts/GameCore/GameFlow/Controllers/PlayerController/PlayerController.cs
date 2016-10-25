using UnityEngine;
using System.Collections;

public class PlayerController : BaseController {

	public Transform _playerTransform { get; private set; }
	public Player _player { get; private set; }

	public override void UpdateGameTime(float globalGameTime, float deltaGameTime) {
		if (_playerTransform == null) {
			return;
		}

		_player.UpdateGameTime(globalGameTime, deltaGameTime);
	}

	public Transform SpawnPlayer(Vector2 spawnPosition) {
		if (_playerTransform == null) {
			GameObject playerPrefab = (GameObject)Resources.Load("Prefabs/PlayerPrefabs/Player");
			GameObject playerObj = (GameObject)Instantiate(playerPrefab, new Vector3(spawnPosition.x, 0, spawnPosition.y), Quaternion.identity);

			_player = playerObj.GetComponent<Player>();
			_player.Initialize();

			playerObj.name = "Player_" + _player._playerID;

			_playerTransform = playerObj.transform;
			_playerTransform.position = new Vector3(spawnPosition.x, 3, spawnPosition.y);
			_playerTransform.rotation = Quaternion.Euler(new Vector3(0, 45, 0));

			GameAccesPoint.Instance.mainGameState._cameraController.SetCameraTarget(_playerTransform);
		} else {
			_playerTransform.position = new Vector3(spawnPosition.x, 0, spawnPosition.y);
		}

		return _playerTransform;
	}
}