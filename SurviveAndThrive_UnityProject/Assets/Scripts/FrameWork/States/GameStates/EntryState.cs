using UnityEngine;
using System.Collections;

public class EntryState : SceneState {

    private ManagerSystem managers;

    protected override void Awake() {
 	    base.Awake();

        InitializeGameSystems();
    }

    private void InitializeGameSystems() {
        managers = FindObjectOfType<ManagerSystem>();
        managers.OnManagersInit += HandleOnManagersInit;

        managers.InitManagers();
    }

    /// <summary>
    /// Waneer alle managers zijn geinitialized vanuit het ManagerSystem script voer dan pas deze functie uit
    /// </summary>
    private void HandleOnManagersInit() {
        stateManager.SetGameState<MainMenuState>();
    }
}
