using UnityEngine;

[System.Serializable]
public class BaseItem : Entity {

	[HideInInspector] public string _id;
	[HideInInspector] public string _itemPath;
	public bool excludeFromBuild = false;
	public bool needsCollider = true;
	[GameItemMenuEnum]
	public GameItemType itemType;
	public BiomeType[] biomeTypes;
	public Vector3 minSize = Vector3.one;
	public Vector3 maxSize = Vector3.one;
	[Header("Uniform XZ based on X values")]
	public bool uniformXZ = true;

    public Mesh[] itemMeshes;

	public virtual void OnCreate(Vector3 position, Quaternion rotation, Transform parent = null, Vector3 scale = default(Vector3)) {
        if (itemMeshes.Length != 0)
        {
            int randomizer = Random.Range(0, itemMeshes.Length);
            if (GetComponent<MeshFilter>() != null)
            {
                GetComponent<MeshFilter>().mesh = itemMeshes[randomizer];
            }
        }
		transform.position = position;
		transform.rotation = rotation;

		if (scale == default(Vector3)) {
			if (!uniformXZ) {
				transform.localScale = new Vector3(Random.Range(minSize.x, maxSize.x), Random.Range(minSize.y, maxSize.y), Random.Range(minSize.z, maxSize.z));
			} else {
				float randomXZ = Random.Range(minSize.x, maxSize.x);
				transform.localScale = new Vector3(randomXZ, Random.Range(minSize.y, maxSize.y), randomXZ);
			}
		} else {
			transform.localScale = scale;
		}

		if (rotation == Quaternion.identity) {
			transform.rotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
		}

		if (parent != null) {
			transform.SetParent(parent);
		}

		Initialize();
	}

	public void SetID(string id) {
		_id = id;
	}

	public void SetPath(string path) {
		_itemPath = path;
	}

	public void SetItemState(string id, string itemPath, GameItemType itemType, BiomeType[] biomeTypes, Vector3 minSize, Vector3 maxSize, bool uniformXZ, bool needsCollider) {
		_id = id;
		_itemPath = itemPath;
		this.itemType = itemType;
		this.biomeTypes = biomeTypes;
		this.minSize = minSize; 
		this.maxSize = maxSize;
		this.uniformXZ = uniformXZ;
		this.needsCollider = needsCollider;
    }

}