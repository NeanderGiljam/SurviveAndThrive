using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SceneState : MonoBehaviour {
    [HideInInspector, SerializeField]
    public string selectedScene;

    public GuiState selectedGUIState;

    protected bool isActive = false;
    protected bool isInit = false;

    protected StateManager stateManager;
    protected InputManager inputManager;

    protected virtual void Awake() {
        stateManager = StateManager.Instance;
        inputManager = InputManager.Instance;
        stateManager.OnStateChange += HandleOnStateChange;
    }

    public virtual void Initialize() {
        
    }

    public virtual void Enable() {
        //Debug.LogWarning("Enable Game State: " + this.name);
        this.enabled = true;
        isActive = true;

        Initialize();
    }

    public virtual void Disable() {
        //Debug.LogWarning("Disable Game State: " + this.name);
        this.enabled = false;
        isActive = false;
    }

    public virtual void SetGuiState() {

    }

    protected virtual void HandleOnStateChange() {
        
    }
}
