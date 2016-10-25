using UnityEngine;
using System.Collections;

public abstract class BaseTool : BasePickableItem {

	// --------------- Tweakables ---------------

	public float baseToolDamage = 5;
	public float baseBuildUpTime = 0.5f;

	// --------------- Tweakables ---------------

	public bool isEquiped;

	protected float currentBuildUpTime = 0;

	protected CameraController cameraController;

	public virtual void Equip() {
		GameAccesPoint.Instance.mainGameState._playerController._player.actionHandler.SetTool(this);
		isEquiped = true;
	}

	public virtual void UnEquip() {
		GameAccesPoint.Instance.mainGameState._playerController._player.actionHandler.SetTool(null);
		isEquiped = false;
	}

	public abstract void Use();
	public abstract void CancelUse();
	protected abstract void Animate();
}