using UnityEngine;
using System.Collections.Generic;

public interface IHandler {
	void Initialize(Object obj);
}

public class Player : Entity, IGameTimeListener, ISavable {

	public int _playerID { get; private set; }
	public bool _isDead { get; private set; }

	public CustomUserActions userActions { get; private set; }

	public PlayerState _currentPlayerState { get; private set; }

	public PlayerStatsHandler statsHandler { get; private set; }
	public PlayerMovementHandler movementHandler { get; private set; }
	public PlayerAnimationHandler animationHandler { get; private set; }
	public PlayerInventoryHandler inventoryHandler { get; private set; }
	public PlayerActionHandler actionHandler { get; private set; }

	private bool isInit = false;

	public override void Initialize() {
		base.Initialize();

		_playerID = 1;

		userActions = GameAccesPoint.Instance.managerSystem.inputManager._userActions;

		SaveManager.Instance.onSaveGame += SaveData;
		if (!LoadData()) {
			// TODO: Create new game data
		}

		statsHandler = gameObject.AddComponent<PlayerStatsHandler>();
		movementHandler = gameObject.AddComponent<PlayerMovementHandler>();
		animationHandler = gameObject.AddComponent<PlayerAnimationHandler>();
		inventoryHandler = gameObject.AddComponent<PlayerInventoryHandler>();
		actionHandler = gameObject.AddComponent<PlayerActionHandler>();

		statsHandler.Initialize(this);
		movementHandler.Initialize(this);
		animationHandler.Initialize(this);
		inventoryHandler.Initialize(this);
		actionHandler.Initialize(this);

		statsHandler.onPlayerDeath += OnDeath;
        statsHandler.onPlayerStartDeath += OnStartDeath;

		isInit = true;
	}

	public void UpdateGameTime(float globalGameTime, float deltaGameTime) {
		if (isInit) {
			statsHandler.DecreaseOverTime(deltaGameTime);
			actionHandler.HandleAction();
			movementHandler.MovePlayer(deltaGameTime);
		}
	}

	public bool LoadData() {
		SavableData data = SaveManager.Instance.GetSaveData(SavableIdentifier.Player);
		if (data != null && data.saveData != null) {
			if (data.saveData[0] is PlayerState) {
				_currentPlayerState = (PlayerState)data.saveData[0];
				return true;
			}
		}

		//Debug.LogWarning("No data loaded for " + name);
		return false;
	}

	public void SaveData() {
		SavableInventory saveableInventory = inventoryHandler.GetSavableInventory();
		PlayerState state = new PlayerState(statsHandler._CurrentHealth, statsHandler._CurrentHunger, statsHandler._CurrentThirst, statsHandler._CurrentEnergy, saveableInventory);

		SavableData savableData = new SavableData(SavableIdentifier.Player, new object[] { state });
		SaveManager.Instance.AddSaveData(savableData);

		SaveManager.Instance.onSaveGame -= SaveData;
	}

    private void OnStartDeath() {
        isInit = false;
        statsHandler.onPlayerStartDeath -= OnStartDeath;
    }


    private void OnDeath() {
		GameAccesPoint.Instance.mainGameState._gameSpeedController.Stop();
		GameAccesPoint.Instance.managerSystem.imageEffectManager.FadeScreen(Direction.Out, 1f);
		GameAccesPoint.Instance.managerSystem.imageEffectManager.OnScreenFaded += OnScreenFaded;
		statsHandler.onPlayerDeath -= OnDeath;
	}

	private void OnScreenFaded() {
		GameAccesPoint.Instance.managerSystem.stateManager.SetGuiState<GameOverGUIState>();
		GameAccesPoint.Instance.managerSystem.imageEffectManager.OnScreenFaded -= OnScreenFaded;
	}
}

[System.Serializable]
public class PlayerState {

	public float playerHealth;
	public float playerHunger;
	public float playerThirst;
	public float playerSleep;
	public SavableInventory savableInventory;

	public PlayerState() { }
	public PlayerState(float playerHealth, float playerHunger, float playerThirst, float playerSleep, SavableInventory savableInventory) {
		this.playerHealth = playerHealth;
		this.playerHunger = playerHunger;
		this.playerThirst = playerThirst;
		this.playerSleep = playerSleep;
		this.savableInventory = savableInventory;
	}

}