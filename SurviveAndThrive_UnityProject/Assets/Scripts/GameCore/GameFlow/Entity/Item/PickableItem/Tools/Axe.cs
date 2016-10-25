using UnityEngine;
using System.Collections;
using System;

public class Axe : BaseTool {

	public override void Equip() {
		base.Equip();

		Debug.Log("Me equiped: " + this.name);
	}

	public override void UnEquip() {
		base.UnEquip();

		Debug.Log("Me unequip: " + this.name);
	}

	public override void Use() {
		// Do stuff
	}

	public override void CancelUse() {
		// Cancel doing stuff
	}

	protected override void Animate() {
		// Play my animation when needed
	}
}