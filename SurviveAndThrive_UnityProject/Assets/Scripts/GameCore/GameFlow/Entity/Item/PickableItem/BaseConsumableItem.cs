using UnityEngine;
using System.Collections.Generic;

public class BaseConsumableItem : BasePickableItem {

	public List<ConsumableStat> fills = new List<ConsumableStat>();

	public void Consume(PlayerStatsHandler statsHandler) {
		foreach (ConsumableStat c in fills) {
			//Debug.Log("My nutrisional values are: " + c.statToFill + ": " + c.value);
			statsHandler.UpdateValue(c.statToFill, c.value);
		}

		Destroy(0);
	}
}

[System.Serializable]
public class ConsumableStat {
	
	public PlayerStats statToFill;
	public float value;

}