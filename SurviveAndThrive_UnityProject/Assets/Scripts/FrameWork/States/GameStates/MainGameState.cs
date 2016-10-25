using UnityEngine;
using System.Collections;

public class MainGameState : SceneState, IGameTimeListener {

	public int _currentSessionIndex { get; private set; }
	public bool _newGame { get; private set; }

    public GameSpeedController _gameSpeedController { get; private set; }
	public GameItemDatabase _gameItemDatabase { get; private set; }
	public AnimalController _animalController { get; private set; }
	public PlayerController _playerController { get; private set; }
	public WorldController _worldController { get; private set; }
	public CameraController _cameraController { get; private set; }
	public AudioController _audioController { get; private set; }
	public FNAAS.AmbientAudioController _ambientAudioController { get; private set; }

	// --------------- Tweakables --------------- //

	// TODO: Move to settings file
	private float saveIntervalTime = 120f; // In seconds

	// --------------- Tweakables --------------- //

	protected override void Awake() {
        base.Awake();

        GameAccesPoint.Instance.mainGameState = this;
	}

	public override void Initialize() {
		GameAccesPoint.Instance.managerSystem.imageEffectManager.OnScreenFaded += ScreenFadedHandler;
		GameAccesPoint.Instance.managerSystem.imageEffectManager.FadeScreen(Direction.In, 0.3f);

		// --------------- State Collision Settings ---------------

		Physics.IgnoreLayerCollision(11, 9, true); // IgnoreCollision between weapons and the player
		Physics.IgnoreLayerCollision(11, 11, true); //  IgnoreCollision between weapons and weapons

		// --------------- State Collision Settings ---------------

		_gameSpeedController = SystemHelper.FindOrCreateController<GameSpeedController>();

		_gameItemDatabase = SystemHelper.FindOrCreateController<GameItemDatabase>();
		_animalController = SystemHelper.FindOrCreateController<AnimalController>();
		_playerController = SystemHelper.FindOrCreateController<PlayerController>();
		_worldController = SystemHelper.FindOrCreateController<WorldController>();
		_cameraController = SystemHelper.FindOrCreateController<CameraController>();
		_audioController = SystemHelper.FindOrCreateController<AudioController>();
		_ambientAudioController = SystemHelper.FindOrCreateController<FNAAS.AmbientAudioController>();

		if (selectedGUIState == null) {
			selectedGUIState = SystemHelper.FindOrCreateGuiState<HUDGUIState>();
		}

		SaveManager.Instance.LoadGameState(_newGame); // TODO: Move to load game screen -> LoadGameState(); -> SetGameState( MainGameState );

		_gameSpeedController.Play();

		_gameItemDatabase.Initialize();
		_animalController.Initialize();
		_playerController.Initialize();
		_worldController.Initialize();
		_cameraController.Initialize();
		_audioController.Initialize();
		_ambientAudioController.Initialize();

		_gameSpeedController.onGameStop += StopGame;
		_gameSpeedController.AddGameTimeListener(this as IGameTimeListener);
	}

	public void SetGameSettings(int sessionIndex, bool newGame) {
		_currentSessionIndex = sessionIndex;
		_newGame = newGame;
	}

    public override void SetGuiState() {
        base.SetGuiState();

        GameAccesPoint.Instance.managerSystem.stateManager.SetGuiState(selectedGUIState);
    }

	private void ScreenFadedHandler() {
		GameAccesPoint.Instance.managerSystem.imageEffectManager.OnScreenFaded -= ScreenFadedHandler;
		isInit = true;
	}

	private void StopGame() {
		_gameSpeedController.onGameStop -= StopGame;
		_gameSpeedController.RemoveGameTimeListener(this as IGameTimeListener);
	}

	private float currentTime = 0;

	public void UpdateGameTime(float globalGameTime, float deltaGameTime) {
		currentTime += deltaGameTime;

		if (currentTime > saveIntervalTime) {
			SaveManager.Instance.SaveGameState();
			Debug.Log("Save @ interval trigger");
			currentTime = 0;
		}

		if (Input.GetKeyDown(KeyCode.X)) {
			_playerController._player.statsHandler.UpdateValue(PlayerStats.Health, -1000); // KILL!
		}
	}
}
