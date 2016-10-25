using UnityEngine;
using System.Collections.Generic;

public class ChunkData : MonoBehaviour {

	public long key;
	public Vector2 worldPosition;
	public BiomeType biomeType;
	public BiomeType[] neighbourBiomeTypes;
	public bool isVisible;

	public void SetChunkData(long key, Vector2 worldPosition, BiomeType biomeType, BiomeType[] neighbourBiomeTypes, bool isVisible) {
		this.key = key;
		this.worldPosition = worldPosition;
		this.biomeType = biomeType;
		this.neighbourBiomeTypes = neighbourBiomeTypes;
		this.isVisible = isVisible;
	}
}