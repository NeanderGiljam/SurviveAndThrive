using UnityEngine;
using System.Collections;

public static class PlayerSettings {

	public static float health = 120;
	public static float hunger = 120;
	public static float thirst = 120;
	public static float energy = 120;

	public static float hungerDecreaseSpeed = 0.6f;
	public static float thirstDecreaseSpeed = 1.0f;
	public static float energyDecreaseSpeed = 0.1f;

	public static float hungerDamage = 2.0f;
	public static float thirstDamage = 1.0f;
	public static float energyDamage = 0.5f;

	public static float hpDamageInterval = 1.0f; // Every X seconds

}