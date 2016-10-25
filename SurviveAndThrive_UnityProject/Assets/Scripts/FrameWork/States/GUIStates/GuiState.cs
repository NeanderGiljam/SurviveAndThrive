using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ButtonActionPair {
    public Button button;
    public UnityAction action;

    public ButtonActionPair(Button button, UnityAction action) {
        this.button = button;
        this.action = action;
    }
}

public class GuiState : MonoBehaviour {

	public bool isInit = false;

	public Transform _Cursor { get; protected set; }
	public GameObject _hoveredObject { get; protected set; }

	public EventSystem _eventSystem { get; private set; }
    public Dictionary<int, ButtonActionPair> menuButtons = new Dictionary<int, ButtonActionPair>();

	protected string guiRootPath = "Prefabs/GUIPrefabs/";

    protected Transform uiRoot;
    protected StateManager stateManager;
    protected InputManager inputManager;
    protected AudioController audioController;

	protected string[] buttonText;
	protected Button[] buttons;
	protected UnityAction[] clickedActions;

	public virtual void Initialize() {
        GameAccesPoint.Instance.managerSystem.stateManager.OnGuiStateChange += SetButtonsDictionary;

        if (GameObject.Find("r_UIRoot")) {
            uiRoot = GameObject.Find("r_UIRoot").transform;
		} else {
            GameObject tmpGO = (GameObject)Resources.Load(guiRootPath + "r_UIRoot");
            uiRoot = (Object.Instantiate(tmpGO) as GameObject).transform;
            uiRoot.name = "r_UIRoot";
            Object.DontDestroyOnLoad(uiRoot);

			GameObject tmpEventSystem = (GameObject)Object.Instantiate(Resources.Load(guiRootPath + "EventSystem"));
			tmpEventSystem.name = "EventSystem";
            Object.DontDestroyOnLoad(tmpEventSystem);
			_eventSystem = tmpEventSystem.GetComponent<EventSystem>();
        }

		if (_eventSystem == null) {
			_eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
		}

        this.transform.SetParent(uiRoot);
        this.transform.localScale = Vector3.one;

        stateManager = GameAccesPoint.Instance.managerSystem.stateManager;
        inputManager = GameAccesPoint.Instance.managerSystem.inputManager;
        audioController = GameAccesPoint.Instance.audioController;
    }

    /// <summary>
    /// Set all the associated object to the state of this object
    /// </summary>
    /// <param name="state"></param>
    public virtual void SetActiveState(bool state) {
        
    }

    public virtual void SetButtonsDictionary(GuiState currentGuiState) {
        if (currentGuiState == null || this != currentGuiState) {
            return;
        }

        SetControlScheme();
    }

    public virtual void SetControlScheme() {
        
    }

	public virtual void RemoveState() {
		
	}

}