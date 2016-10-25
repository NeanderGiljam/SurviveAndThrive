using UnityEngine;
using System.Collections.Generic;

public delegate void OnChunkEnter(Vector2 chunkLocation);
public delegate void OnChunkSpawned(Chunk completedChunk);

public class WorldController : BaseController, ISavable {

	public event OnChunkEnter onChunkEnter;
	public event OnChunkSpawned onChunkSpawned;

	public GameItemDatabase _biomeDatabase { get; private set; }

	public Vector2 _chunkSize { get { return new Vector2(30, 30); } }
	public Vector2 _spawnPos { get; private set; }

	public Chunk _currentChunk { get; private set; }

	private GameObject groundPrefab;

	// --------------- Tweakables ---------------

	private int visibleRange = 1;

	//private string seed = "Survive"; // TODO: Apply this to noisemap offset

	// --------------- Tweakables ---------------

	public Dictionary<long, Chunk> chunks = new Dictionary<long, Chunk>(); // TODO: If needed split into visible and non-visible chunks

	private int currentX;
	private int currentZ;

	private int oldX = -50000;
	private int oldZ = -50000;

	private BiomeType currentBiome;

	private Transform playerTransform;
	private Texture2D biomeMap;

	public override void Initialize() {
		SaveManager.Instance.onSaveGame += SaveData;
		if (!LoadData()) {
			// Create new game data
		}

		//Random.seed = 12345; // TODO: Reimplement seed

		_biomeDatabase = GameAccesPoint.Instance.mainGameState._gameItemDatabase;

		biomeMap = (Texture2D)Resources.Load("Prefabs/BiomeData/BiomeMap_03", typeof(Texture2D));
		groundPrefab = (GameObject)Resources.Load("Prefabs/GroundPlane");

		if (_biomeDatabase == null || biomeMap == null) {
			Debug.LogError("Can't spawn world because of an issue.");
		}

		_spawnPos = new Vector2(0, 0); // Chunkpos -> Worldpos / chunksize, y pos = height - coord

		playerTransform = GameAccesPoint.Instance.mainGameState._playerController.SpawnPlayer(new Vector2(_spawnPos.x * _chunkSize.x, _spawnPos.y * _chunkSize.y));

		isInit = true;
	}

	float chunkEnterTime = 0;

	public override void UpdateGameTime(float globalGameTime, float deltaGameTime) {
		if (!isInit || !playerTransform) {
			return;
		}

		if (playerTransform.position.x < 0) {
			currentX = ((int)playerTransform.position.x - ((int)_chunkSize.x / 2)) / (int)_chunkSize.x;
		} else {
			currentX = ((int)playerTransform.position.x + ((int)_chunkSize.x / 2)) / (int)_chunkSize.x;
		}

		if (playerTransform.position.z < 0) {
			currentZ = ((int)playerTransform.position.z - ((int)_chunkSize.y / 2)) / (int)_chunkSize.y;
		} else {
			currentZ = ((int)playerTransform.position.z + ((int)_chunkSize.y / 2)) / (int)_chunkSize.y;
		}

		if (currentX != oldX || currentZ != oldZ) {
			DoAnalytics(new Vector2(oldX, oldZ));	

			oldX = currentX;
			oldZ = currentZ;

			SetVisualState();
			SetCurrentBiomeType();

			for (int x = -visibleRange; x <= visibleRange; x++) {
				for (int z = -visibleRange; z <= visibleRange; z++) {
					long newKey = ((int)currentX + x) + (((long)currentZ + z) << 32);
					if (!chunks.ContainsKey(newKey)) {
						Vector2 newPos = new Vector2(currentX + x, currentZ + z);
						Chunk newChunk = new Chunk(newKey, newPos, GetBiomeTypeFromMap(newPos), GetNeigbourTypes(newPos), groundPrefab, this);
						chunks.Add(newKey, newChunk);
					}
				}
			}

			SetCurrentChunk();
		}
	}

	public void ChunkReady(Chunk spawnedChunk) {
		if (onChunkSpawned != null)
			onChunkSpawned(spawnedChunk);
	}

	private void SetVisualState() { // TODO: Change this to direct neighbours -> this.key + 1 == neighbour etc.
		foreach (KeyValuePair<long, Chunk> pair in chunks) {
			int distance = (int)Vector2.Distance(new Vector2(currentX, currentZ), pair.Value._worldPosition);

			if (distance > visibleRange && pair.Value._isVisible) {
				pair.Value.SetVisibleState(false);
			} else if(distance <= visibleRange && !pair.Value._isVisible) {
				pair.Value.SetVisibleState(true);
			}
		}
	}

	private void SetCurrentBiomeType() {
		long currentChunkKey = (int)currentX + ((long)currentZ << 32);
		if (chunks.ContainsKey(currentChunkKey)) {
			currentBiome = chunks[currentChunkKey]._biomeType;
		} else {
			currentBiome = BiomeType.Undefined;
		}
	}

	private BiomeType GetBiomeTypeFromMap(Vector2 position) {
		int resolution = 256;
		Color positionColor = biomeMap.GetPixel((int)position.x + (resolution / 2), (int)position.y + (resolution / 2));
		float grayValue = positionColor.grayscale;

		if (grayValue <= 0.25f) {
			return BiomeType.Ocean;
		} else if (grayValue > 0.25f && grayValue <= 0.71f) {
			return BiomeType.Jungle;
		} else if (grayValue > 0.71f && grayValue <= 0.76f) {
			//return BiomeType.Plains;
			return BiomeType.Jungle;
		} else if (grayValue > 0.76f && grayValue <= 0.91f) {
			//return BiomeType.Desert;
			return BiomeType.Jungle;
		} else {
			//return BiomeType.Snow;
			return BiomeType.Jungle;
		}
	}

	private BiomeType[] GetNeigbourTypes(Vector2 chunkPos) {
		BiomeType[] biomeTypes = new BiomeType[4];
		int index = 0;

		for (int x = -1; x <= 1; x++) {
			for (int z = -1; z <= 1; z++) {
				if (Mathf.Abs(x) != Mathf.Abs(z)) {
					biomeTypes[index] = GetBiomeTypeFromMap(new Vector2(chunkPos.x + x, chunkPos.y + z));
					index++;
				}
			}
		}

		if (biomeTypes == null || biomeTypes.Length <= 0) {
			return null;
		}

		return biomeTypes;
	}

	private void SetCurrentChunk() {
		long currentChunkKey = (int)currentX + ((long)currentZ << 32);
		if (chunks.ContainsKey(currentChunkKey)) {
			_currentChunk = chunks[currentChunkKey];

			if (onChunkEnter != null)
				onChunkEnter(_currentChunk._worldPosition);
        }
	}

	private void OnGUI() {
		if (playerTransform == null) {
			return;
		}

		DebugHelper.playerCoords = "Player Coordinates -> X: " + (int)playerTransform.position.x + " Y: " + (int)playerTransform.position.y + " Z: " + (int)playerTransform.position.z;
		DebugHelper.chunkCoords = "Chunk Coordinates -> X: " + currentX + " Z: " + currentZ;
		DebugHelper.biomeType = "Current Biome: " + currentBiome;
		DebugHelper.SetDebugWindowData();
	}

	private void DoAnalytics(Vector2 oldPos) {
		BiomeType biomeType = GetBiomeTypeFromMap(oldPos);

		float chunkLeaveTime = Time.realtimeSinceStartup;

		if (chunkEnterTime != 0) {
			float timeInPreviousChunk = chunkLeaveTime - chunkEnterTime;
			AnalyticsHelper.AddChunkTime(oldPos, biomeType, timeInPreviousChunk);
		}

		chunkEnterTime = Time.realtimeSinceStartup;
	}

	public bool LoadData() {
		SavableData data = SaveManager.Instance.GetSaveData(SavableIdentifier.WorldController);
		if (data != null && data.saveData != null) {
			// Handle loaded data
			return true;
		}

		//Debug.LogWarning("No data loaded for " + name);
		return false;
	}

	public void SaveData() {
		// TODO: Create real data
		//SavableData saveData = new SavableData(SavableIdentifier.WorldController, new object[] { "aap" });
		//SaveManager.AddSaveData(saveData);
	}

}