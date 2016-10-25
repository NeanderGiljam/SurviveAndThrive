using UnityEngine;
using System.Collections;

public static class WorldSettings {
	/*
	- biomeType must be unique
	- density = spawn chance > equals less chance
	- density 0 = no spawning of that type
	- density 1 = always spawn when possible
	*/
	public static BiomeSetting[] biomeSettings = {
		new BiomeSetting() {
			biomeType = BiomeType.Ocean,
			spawnSettings = new SpawnSetting[] {
				new SpawnSetting() { noisePreset = SpawnSettingType.Grass,  spawnDensity = 0,       resolution = 64, frequency = 1.2f },
				new SpawnSetting() { noisePreset = SpawnSettingType.Plant,  spawnDensity = 0,       resolution = 64, frequency = 3.0f },
				new SpawnSetting() { noisePreset = SpawnSettingType.Stone,  spawnDensity = 0,       resolution = 64, frequency = 8.0f },
				new SpawnSetting() { noisePreset = SpawnSettingType.Tree,   spawnDensity = 0,       resolution = 64, frequency = 1.0f }
			}
		},
		new BiomeSetting() {
			biomeType = BiomeType.Desert,
			spawnSettings = new SpawnSetting[] {
				new SpawnSetting() { noisePreset = SpawnSettingType.Grass,  spawnDensity = 4,       resolution = 64, frequency = 1.2f },
				new SpawnSetting() { noisePreset = SpawnSettingType.Plant,  spawnDensity = 10,      resolution = 64, frequency = 3.0f },
				new SpawnSetting() { noisePreset = SpawnSettingType.Stone,  spawnDensity = 256,     resolution = 64, frequency = 8.0f },
				new SpawnSetting() { noisePreset = SpawnSettingType.Tree,   spawnDensity = 1024,    resolution = 64, frequency = 10.0f}
			}
		},
		new BiomeSetting() {
			biomeType = BiomeType.Plains,
			spawnSettings = new SpawnSetting[] {
				new SpawnSetting() { noisePreset = SpawnSettingType.Grass,  spawnDensity = 1,       resolution = 64, frequency = 1.2f },
				new SpawnSetting() { noisePreset = SpawnSettingType.Plant,  spawnDensity = 1,       resolution = 64, frequency = 3.0f },
				new SpawnSetting() { noisePreset = SpawnSettingType.Stone,  spawnDensity = 1024,    resolution = 64, frequency = 8.0f },
				new SpawnSetting() { noisePreset = SpawnSettingType.Tree,   spawnDensity = 0,       resolution = 64, frequency = 1.0f }
			}
		},
        new BiomeSetting() {
			biomeType = BiomeType.Jungle,
			spawnSettings = new SpawnSetting[] {
				new SpawnSetting() { noisePreset = SpawnSettingType.Grass,		spawnDensity = 1,       resolution = 64,	frequency = 1.2f },
				new SpawnSetting() { noisePreset = SpawnSettingType.Plant,		spawnDensity = 2,       resolution = 64,	frequency = 3.0f },
				new SpawnSetting() { noisePreset = SpawnSettingType.Stone,		spawnDensity = 256,     resolution = 64,	frequency = 8.0f },
				new SpawnSetting() { noisePreset = SpawnSettingType.Tree,		spawnDensity = 35,      resolution = 52,	frequency = 3.0f },
				new SpawnSetting() { noisePreset = SpawnSettingType.Food,		spawnDensity = 100,     resolution = 64,	frequency = 2.0f },
				new SpawnSetting() { noisePreset = SpawnSettingType.Resources,	spawnDensity = 100,		resolution = 64,	frequency = 2.0f }
			}
		},
		new BiomeSetting() {
			biomeType = BiomeType.Snow,
			spawnSettings = new SpawnSetting[] {
				new SpawnSetting() { noisePreset = SpawnSettingType.Grass,  spawnDensity = 0,       resolution = 64, frequency = 1.2f },
				new SpawnSetting() { noisePreset = SpawnSettingType.Plant,  spawnDensity = 0,       resolution = 64, frequency = 3.0f },
				new SpawnSetting() { noisePreset = SpawnSettingType.Stone,  spawnDensity = 0,		resolution = 64, frequency = 8.0f },
				new SpawnSetting() { noisePreset = SpawnSettingType.Tree,   spawnDensity = 0,		resolution = 64, frequency = 2.0f }
			}
		},
	};

	public static BiomeSetting GetSettingsByType(BiomeType biomeType) {
		foreach (BiomeSetting s in biomeSettings) {
			if (s.biomeType == biomeType) {
				return s;
			}
		}

		return new BiomeSetting();
	}
}

[System.Serializable]
public class BiomeSetting {
	internal BiomeType biomeType = BiomeType.PineForest;
	internal SpawnSetting[] spawnSettings;

	public SpawnSetting GetSpawnSettingsByType(SpawnSettingType spawnType) {
		if (spawnSettings == null || spawnSettings.Length <= 0) {
			Debug.LogWarning("The selected settings don't exist, using defaults");
			return new SpawnSetting();
		}

		foreach (SpawnSetting n in spawnSettings) {
			if (n.noisePreset == spawnType) {
				return n;
			}
		}

		return new SpawnSetting();
	}
}

public class SpawnSetting {
	internal SpawnSettingType noisePreset = SpawnSettingType.Tree;

	internal int spawnDensity = 0;

	[Range(2, 512)]
	internal int resolution = 64;

	internal float frequency = 2.0f;
}