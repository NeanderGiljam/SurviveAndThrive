using UnityEngine;
using UnityEngine.Analytics;
using System.Collections.Generic;

public static class AnalyticsHelper {

	private static bool doAnalytics = false; // TODO: Release builds set to true
	private static bool debug = false;

	public static void AddChunkTime(Vector2 chunkPos, BiomeType biomeType, float time) {
		if (!doAnalytics) {
			return;
		}

		AnalyticsResult result = Analytics.CustomEvent("timePerChunk", new Dictionary<string, object> {
			{ "Chunk Pos", chunkPos.ToString() },
			{ "Biome type", biomeType.ToString() },
			{ "Time", time }
		});

		if (debug) {
			Debug.Log("Analytics Result: " + result);
			Debug.Log("ChunkPos: " + chunkPos + " BiomeType: " + biomeType + " Time: " + time);
		}
	}

}