using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PlayerStats {
	Health,
	Hunger,
	Thirst,
	Sleep
}

public delegate void OnPlayerDeath();
public delegate void OnPlayerStartDeath();

public class PlayerStatsHandler : MonoBehaviour, IHandler {

	private bool debug = false;

	public event OnPlayerDeath onPlayerDeath;
    public event OnPlayerStartDeath onPlayerStartDeath;

    public float _CurrentHealth { get; private set; }
	public float _CurrentHunger { get; private set; }
	public float _CurrentThirst { get; private set; }
	public float _CurrentEnergy { get; private set; }

    //--------------------Ragdoll Presets--------------------//
    private bool killRagdollActive = false;
    public Component[] bones;
    public Animator anim;
    private Rigidbody mainBody;
    private Collider mainCollider;
    //--------------------Ragdoll Presets--------------------//

    private bool isInit;
	private HUDGUIState hudGUIState;

	public void Initialize(Object obj) {
		// TODO: Load from loaded PlayerState else generate new data from PlayerSettings
		PlayerState playerState = (obj as Player)._currentPlayerState;
		if (playerState == null) {
			_CurrentHealth = PlayerSettings.health;
			_CurrentHunger = PlayerSettings.hunger;
			_CurrentThirst = PlayerSettings.thirst;
			_CurrentEnergy = PlayerSettings.energy;
		} else {
			_CurrentHealth = playerState.playerHealth;
			_CurrentHunger = playerState.playerHunger;
			_CurrentThirst = playerState.playerThirst;
			_CurrentEnergy = playerState.playerSleep;
		}

		hudGUIState = GameAccesPoint.Instance.hudGUIState;

		if (!debug)
			isInit = true;
	}

	/// <summary>
	/// Amount 0 - PlayerSettings.(Stat base max value)
	/// </summary>
	/// <param name="stat"></param>
	/// <param name="amount"></param>
	public void UpdateValue(PlayerStats stat, float amount) {
		switch (stat) {
			case PlayerStats.Health:
				_CurrentHealth += amount;
				break;
			case PlayerStats.Hunger:
				_CurrentHunger += amount;
				break;
			case PlayerStats.Thirst:
				_CurrentThirst += amount;
				break;
			case PlayerStats.Sleep:
				_CurrentEnergy += amount;
				break;
			default:
				break;
		}
	}

	// TODO: Add pause logic
	public void DecreaseOverTime(float deltaGameTime) {
		if (!isInit)
			return;

		_CurrentHunger -= deltaGameTime * PlayerSettings.hungerDecreaseSpeed;
		_CurrentThirst -= deltaGameTime * PlayerSettings.thirstDecreaseSpeed;
		_CurrentEnergy -= deltaGameTime * PlayerSettings.energyDecreaseSpeed;
		//Debug.Log("Hunger: " + _CurrentHunger + " | Thirst: " + _CurrentThirst + " | Sleep: " + _CurrentSleep);
		CheckBars(deltaGameTime);
	}

	float damageTime = 0;
	float damageTaken = 0;

	private void CheckBars(float deltaGameTime) {
		if (_CurrentHunger <= 0) {
			damageTaken += deltaGameTime * PlayerSettings.hungerDamage;
		}

		if (_CurrentThirst <= 0) {
			damageTaken += deltaGameTime * PlayerSettings.thirstDamage;
		}

		if (_CurrentEnergy <= 0) {
			damageTaken += deltaGameTime * PlayerSettings.energyDamage;
		}

		damageTime += deltaGameTime;

		if (damageTime >= PlayerSettings.hpDamageInterval) {
			UpdateValue(PlayerStats.Health, -damageTaken);
			damageTaken = 0;
			damageTime = 0;
		}

		ClampValues();
		UpdateGUI();
		CheckDeath();
	}

	private void CheckDeath() {
		if (_CurrentHealth <= 0) {
			//Debug.Log("Death");
            if (onPlayerStartDeath != null) {
                onPlayerStartDeath();
            }

			if (onPlayerDeath != null && killRagdollActive == false) {
                KillRagdoll();
                killRagdollActive = true;
                StartCoroutine(triggerPlayerDeath());
            }
		}
	}

    private void KillRagdoll()
    {
        mainBody = GetComponent<Rigidbody>();
        mainCollider = GetComponent<CapsuleCollider>();
        anim = GetComponent<Animator>();
        bones = gameObject.GetComponentsInChildren<Rigidbody>();
        mainBody.isKinematic = true;
        mainBody.useGravity = false;
        //mainCollider.enabled = false;
        
        //-------------------------Destroying Collider-------------------------//
        Destroy(mainCollider);
        //-------------------------Destroying Collider-------------------------//

        anim.enabled = false;
        foreach (Rigidbody ragdoll in bones)
        {
            ragdoll.isKinematic = false;
            ragdoll.gameObject.GetComponent<Collider>().enabled = true;
        }
    }
    IEnumerator triggerPlayerDeath(){
        yield return new WaitForSeconds(5);
        if (onPlayerDeath != null)
        {
            onPlayerDeath();
        }
    }
    private void UpdateGUI() {
		if (hudGUIState == null)
			return;

		hudGUIState.UpdateStats(_CurrentHealth / PlayerSettings.health, _CurrentHunger / PlayerSettings.hunger, _CurrentThirst / PlayerSettings.thirst, _CurrentEnergy / PlayerSettings.energy);
	}

	private void ClampValues() {
		_CurrentHealth = Mathf.Clamp(_CurrentHealth, 0, PlayerSettings.health);
		_CurrentHunger = Mathf.Clamp(_CurrentHunger, 0, PlayerSettings.hunger);
		_CurrentThirst = Mathf.Clamp(_CurrentThirst, 0, PlayerSettings.thirst);
		_CurrentEnergy = Mathf.Clamp(_CurrentEnergy, 0, PlayerSettings.energy);
	}

}