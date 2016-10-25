using UnityEngine;
using System.Collections;

public class BaseAnimal : BaseItem {

	// --------------- Tweakables ---------------

	public float baseHealth = 10.0f;
	public float baseMoveSpeed = 1.0f;

	// --------------- Tweakables ---------------

	protected float currentHealth;
	protected float damageOnHit;

	protected bool isInit = false;
	protected bool isDying = false;
	protected bool isHit = false;
	protected bool firstHit = false;

	protected AnimalController parent;
	protected Animator animator;

	protected AnimalState myAnimalState;

	/// <summary>
	/// DecreaseValue 0 = One hit kill
	/// </summary>
	/// <param name="decreaseValue"></param>
	public virtual void Hit(float decreaseValue = 0) {
		if (isDying) {
			return;
		}

		damageOnHit += decreaseValue;

		AudioManager.Instance.PlayByName("deer_hit");

		if (decreaseValue == 0) {
			currentHealth -= currentHealth;
		} else {
			currentHealth -= decreaseValue;
		}

		if (isHit == false) {
			isHit = true; // TODO: Check if hit by arrow
			firstHit = true;
		}

		DecreaseHealthOverTime(damageOnHit);
	}

	public virtual void DecreaseHealthOverTime(float timeFactor) {
		//Debug.Log("Current health: " + currentHealth);

		if (currentHealth <= 0) {
			Die();
		}

		currentHealth -= timeFactor * Time.deltaTime;
	}

	protected virtual void Die() {
		isDying = true;
		DropItems();
	}

	protected virtual void DropItems() {
		gameItemDatabase.CreateItemInstance("14:100", transform.position, Quaternion.identity);

		/*
		Renderer[] renderers = GetComponentsInChildren<Renderer>();
		for (int i = 0; i < renderers.Length; i++) {
			renderers[i].material.color = Color.red;
		}
		*/

		//Destroy(); // TODO: Implement death animation etc
	}
}