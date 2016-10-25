using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chunk {

	// TODO: Remove debugging when releasing
	private bool debug = false; // TODO: Turn of when not debugging
	private bool disableFoliage = false;

	private SpawnSettingType debugNoisePreset = SpawnSettingType.Tree;

	// --------------- Tweakables ---------------

	private float noiseStrength = 7;
	private float heightValue = 2.5f;

	private Vector2 textureRepeat = new Vector2(1, 1);

	// --------------- Tweakables ---------------

	public bool _isVisible { get; private set; }
	public Vector2 _worldPosition { get; private set; }
	public BiomeType _biomeType { get; private set; }

	public string name = "";

	private long key;

	private GameObject ground;
	private ChunkData chunkData; // For testing
	private WorldController parent;

	private BiomeItemsDatabase myBiomeItems;

	private List<GameObject> nonChangeableObject = new List<GameObject>();
	private List<GameObject> changeableEntities = new List<GameObject>();

	private int[] noSpawnZone;
	private BiomeType[] neighbourBiomeTypes; // Left Bottom Top Right

	public Chunk() { }
	public Chunk(long key, Vector2 worldPosition, BiomeType biomeType, BiomeType[] neighbourBiomeTypes, GameObject groundPrefab = null, WorldController parent = null) {
		this.key = key;
		_worldPosition = worldPosition;
		_biomeType = biomeType;
		this.neighbourBiomeTypes = neighbourBiomeTypes;
		this.parent = parent;

		name = worldPosition.ToString();

		SetNoSpawnZone();
		myBiomeItems = parent._biomeDatabase.GetBiomeItems(_biomeType);

		CreateChunk(groundPrefab);

		if (debug) {
			chunkData.SetChunkData(key, _worldPosition, biomeType, neighbourBiomeTypes, _isVisible);
		}
	}

	public void SetVisibleState(bool isVisible) {
		_isVisible = isVisible;

		if (isVisible) {
			foreach (GameObject obj in nonChangeableObject) {
				obj.SetActive(true);
			}

			foreach (GameObject entity in changeableEntities) {
				entity.gameObject.SetActive(true);
			}
		} else {
			foreach (GameObject obj in nonChangeableObject) {
				if (obj != null) {
					obj.SetActive(false);
				}
			}

			foreach (GameObject entity in changeableEntities) {
				entity.gameObject.SetActive(false);
			}
		}

		if (debug) {
			chunkData.SetChunkData(key, _worldPosition, _biomeType, neighbourBiomeTypes, _isVisible);
		}
	}

	public void AddChangeableItem(GameObject item) {
		if (!changeableEntities.Contains(item)) {
			changeableEntities.Add(item);
			item.transform.SetParent(nonChangeableObject[0].transform);
		}
	}

	public void RemoveChangeableItem(GameObject item) {
		if (changeableEntities.Contains(item)) {
			changeableEntities.Remove(item);
			item.transform.SetParent(null);
		}
	}

	// --------------- CHUNK CREATION ---------------
	// --------------- CHUNK CREATION ---------------

	#region CHUNK CREATION

	private void CreateChunk(GameObject groundPrefab) {
		ground = (GameObject)GameObject.Instantiate(groundPrefab, new Vector3(_worldPosition.x * parent._chunkSize.x, 0, _worldPosition.y * parent._chunkSize.y), Quaternion.identity);
		ground.transform.localScale = new Vector3((parent._chunkSize.x / 10), 1, (parent._chunkSize.y / 10));
		ground.name = _biomeType.ToString();
		ground.transform.SetParent(parent.transform);

		AddElevation();

		if (debug) {
			chunkData = ground.AddComponent<ChunkData>();
		}
		nonChangeableObject.Add(ground);
		_isVisible = true;

		foreach (GameItemDatabase.BiomeData data in parent._biomeDatabase.biomeData) {
			if (data.biomeType == _biomeType) {
				Material groundMaterial = ground.GetComponent<MeshRenderer>().material;

				if (!debug) {
					if (data.biomeTexture == null) {
						groundMaterial.color = data.biomeColor;
					} else {
						groundMaterial.SetTexture("_Texture", data.biomeTexture); // TODO: Change back to groundMaterial.mainTexture by editing the shader property name
						groundMaterial.mainTextureScale = textureRepeat;
						groundMaterial.color = data.biomeColor;
					}
				} else {
					//groundMaterial.mainTexture = MakeNoiseTexture(_worldPosition, debugNoisePreset);
					groundMaterial.SetTexture("_Texture", MakeNoiseTexture(_worldPosition, debugNoisePreset));
				}
			}
		}

		if (_biomeType != BiomeType.Ocean && _biomeType != BiomeType.River) {
			if (!disableFoliage) {
				ChunkSpawnHelper spawnHelper = ground.AddComponent<ChunkSpawnHelper>();
				spawnHelper.Initialize(this);
			}

			//AddLife();
		}
	}

	private void AddElevation() {
		Mesh mesh = ground.GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = mesh.vertices;
		float sizeFactor = parent._chunkSize.x / 10;
		noiseStrength = noiseStrength / sizeFactor;

		int i = 0;
		while (i < vertices.Length) {
			vertices[i].y = Mathf.PerlinNoise((ground.transform.position.x / sizeFactor + vertices[i].x) / noiseStrength, (ground.transform.position.z / sizeFactor + vertices[i].z) / noiseStrength) * heightValue;
			i++;
		}

		mesh.vertices = vertices;
		//mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		ground.AddComponent<MeshCollider>();
	}

	public void AddLife() {
		AnimalController animalController = GameAccesPoint.Instance.mainGameState._animalController;

		float xStart = (_worldPosition.x * parent._chunkSize.x) - (parent._chunkSize.x / 2);
		float zStart = (_worldPosition.y * parent._chunkSize.y) - (parent._chunkSize.y / 2);
		Vector3 spawnPos = new Vector3(xStart + (parent._chunkSize.x / 2), 5, zStart + (parent._chunkSize.y / 2));

		//animalController.SpawnPreyAnimal("1:101", spawnPos);
		animalController.SpawnHerd("1:100", spawnPos, 1);

		parent.ChunkReady(this);
	}

	public void SpawnObject(SpawnSettingType spawnType, Vector2 spawnPosition, int resolution) {
		float xStart = (_worldPosition.x * parent._chunkSize.x) - (parent._chunkSize.x / 2);
		float zStart = (_worldPosition.y * parent._chunkSize.y) - (parent._chunkSize.y / 2);
		Vector3 newSpawnPos = new Vector3(xStart + spawnPosition.x * (parent._chunkSize.x / resolution) + Random.Range(-.5f, .5f), 0, zStart + spawnPosition.y * (parent._chunkSize.y / resolution) + Random.Range(-.5f, .5f));

		if (newSpawnPos.x < xStart + noSpawnZone[0] || newSpawnPos.x > xStart + parent._chunkSize.x - noSpawnZone[3] ||
			newSpawnPos.z < zStart + noSpawnZone[1] || newSpawnPos.z > zStart + parent._chunkSize.y - noSpawnZone[2]) {
			return;
		}

		BaseItem[] possibleItems = myBiomeItems.GetItemArrayByType(spawnType);

		if (possibleItems == null || possibleItems.Length <= 0) {
			return;
		}

		BaseItem item = possibleItems[Random.Range(0, possibleItems.Length)];

		if (item == null || item.gameObject == null) {
			return;
		}

		Ray ray = new Ray(new Vector3(newSpawnPos.x, 10, newSpawnPos.z), -Vector3.up);
		RaycastHit hitInfo;

		if (Physics.Raycast(ray, out hitInfo, 10, (1 << 8))) {
			newSpawnPos = hitInfo.point;
			float radius = item.transform.localScale.x + 0.2f;
			if (Physics.CheckSphere(newSpawnPos, radius, ~(1 << 8))) {
				return;
			}
		} else {
			return;
		}

		GameObject tmpObj = (GameObject)GameObject.Instantiate(item.gameObject, newSpawnPos, Quaternion.identity);
		tmpObj.GetComponent<BaseItem>().OnCreate(newSpawnPos, Quaternion.identity, ground.transform);

		BasePickableItem pickable = tmpObj.GetComponent<BasePickableItem>();
		if (pickable) {
			changeableEntities.Add(tmpObj);
		} else {
			nonChangeableObject.Add(tmpObj);
		}
	}

	public void DisableUnusedColliders() {
		List<GameObject>[] objectLists = new List<GameObject>[] {
			changeableEntities,
			nonChangeableObject
		};

		for (int i = 0; i < objectLists.Length; i++) {
			foreach (GameObject obj in objectLists[i]) {
				BaseItem item = obj.GetComponent<BaseItem>();
                if (item !=null && !item.needsCollider) {
					Collider[] objsColliders = obj.GetComponents<Collider>();
					Collider[] objsChildColliders = obj.GetComponentsInChildren<Collider>();

					for (int j = 0; j < objsColliders.Length; j++) {
						objsColliders[j].enabled = false;
					}

					for (int k = 0; k < objsChildColliders.Length; k++) {
						objsChildColliders[k].enabled = false;
					}
				}
			}
		}
	}

	public float[] GetNoiseSamples(Vector2 worldPosition, SpawnSettingType spawnType) {
		BiomeSetting biomeSettings = WorldSettings.GetSettingsByType(_biomeType);
		SpawnSetting spawnSettings = biomeSettings.GetSpawnSettingsByType(spawnType);

		float[] noiseSamples = new float[spawnSettings.resolution * spawnSettings.resolution];
		float frequency = spawnSettings.frequency;
		float resolution = spawnSettings.resolution;

		float x = 0.0F;
		while (x < spawnSettings.resolution) {
			float y = 0.0F;
			while (y < spawnSettings.resolution) {
				float xCoord = (worldPosition.x) * frequency + x / resolution * frequency;
				float yCoord = (worldPosition.y) * frequency + y / resolution * frequency;
				noiseSamples[(int)(y * spawnSettings.resolution + x)] = Mathf.PerlinNoise(xCoord, yCoord); // TODO: Significantly faster -> Look at simplex for more performance and Voronoi for other effects
				y++;
			}
			x++;
		}

		return noiseSamples;
	}

	private void SetNoSpawnZone() {
		noSpawnZone = new int[4]; // Left Bottom Top Right

		for (int i = 0; i < neighbourBiomeTypes.Length; i++) {
			if (neighbourBiomeTypes[i] != _biomeType) {
				noSpawnZone[i] = 1;
			} else {
				noSpawnZone[i] = 0;
			}
		}
	}

	#endregion

	// --------------- CHUNK CREATION ---------------
	// --------------- CHUNK CREATION ---------------

	// --------------- NOISE DEBUGGING ---------------
	// --------------- NOISE DEBUGGING ---------------

	#region NOISE DEBUGGING

	private Texture2D MakeNoiseTexture(Vector2 worldPos, SpawnSettingType spawnType) {
		BiomeSetting biomeSettings = WorldSettings.GetSettingsByType(_biomeType);
		SpawnSetting noiseSettings = biomeSettings.GetSpawnSettingsByType(spawnType);

		Texture2D noiseTex = new Texture2D(noiseSettings.resolution, noiseSettings.resolution, TextureFormat.RGB24, true);
		noiseTex.wrapMode = TextureWrapMode.Clamp;
		noiseTex.filterMode = FilterMode.Trilinear;
		noiseTex.anisoLevel = 9;
		Color[] pix = new Color[noiseTex.width * noiseTex.height];
		float[] noiseSamples = GetNoiseSamples(worldPos, spawnType);

		for (int x = 0; x < noiseSettings.resolution; x++) {
			for (int y = 0; y < noiseSettings.resolution; y++) {
				float sample = noiseSamples[y * noiseSettings.resolution + x];
				pix[y * noiseSettings.resolution + x] = new Color(sample, sample, sample);
			}
		}

		noiseTex.SetPixels(pix);
		noiseTex.Apply();

		return FlipTexture(noiseTex);
	}

	private Texture2D FlipTexture(Texture2D orgTexture) {
		Texture2D texture = new Texture2D(orgTexture.width, orgTexture.height);

		for (int i = 0; i < texture.width; i++) {
			for (int j = 0; j < texture.height; j++) {
				texture.SetPixel(texture.width - i - 1, texture.height - j - 1, orgTexture.GetPixel(i, j));
			}
		}
		texture.Apply();

		return texture;
	}

	#endregion

	// --------------- NOISE DEBUGGING ---------------
	// --------------- NOISE DEBUGGING ---------------
}

// TODO: Make better so that no spawning errors occur
public class ChunkSpawnHelper : MonoBehaviour {

	private Chunk parent;

	public void Initialize(Chunk parent) {
		// Set variables
		this.parent = parent;
		SpawnOverTime();
	}

	public void SpawnOverTime() {
		IEnumerator[] sequence = new IEnumerator[] {
			AddFoliage(SpawnSettingType.Tree),
			AddFoliage(SpawnSettingType.Stone),
			AddFoliage(SpawnSettingType.Plant),
			AddFoliage(SpawnSettingType.Grass),
			AddFoliage(SpawnSettingType.Food), // TODO: Make better
			AddFoliage(SpawnSettingType.Resources),
			AddLife()
		};
		StartCoroutine(Sequence(sequence));
	}

	public IEnumerator Sequence(IEnumerator[] sequence) {
		for (int i = 0; i < sequence.Length; i++) {
			while (sequence[i].MoveNext()) {
				yield return sequence[i].Current;
			}
		}

		parent.DisableUnusedColliders(); // TODO: Reimplement (base item -> need collider? bool)
		Remove();
	}

	private IEnumerator AddFoliage(SpawnSettingType spawnType) {
		BiomeSetting biomeSettings = WorldSettings.GetSettingsByType(parent._biomeType);
		SpawnSetting spawnSettings = biomeSettings.GetSpawnSettingsByType(spawnType);
		float[] noiseSamples = parent.GetNoiseSamples(parent._worldPosition, spawnType);

		for (int x = 0; x < spawnSettings.resolution; x++) {
			for (int y = 0; y < spawnSettings.resolution; y++) {
				float sample = noiseSamples[y * spawnSettings.resolution + x];
				if (sample > 0.3f) {
					int randomNumber = Random.Range(0, spawnSettings.spawnDensity + 1);
					if (randomNumber == 1) {
						parent.SpawnObject(spawnType, new Vector2(x, y), spawnSettings.resolution);
						if ((y * spawnSettings.resolution + x) % 20 == 0) {
							yield return null;
						}
					}
				}
			}
		}
	}

	private IEnumerator AddLife() {
		parent.AddLife();
		yield return null;
	}

	private void Remove() {
		Destroy(this);
	}
}