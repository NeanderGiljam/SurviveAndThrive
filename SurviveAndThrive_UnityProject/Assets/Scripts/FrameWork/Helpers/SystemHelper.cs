using UnityEngine;
using System.Collections;

public static class SystemHelper {

    private static string guiPrefabsPath = "Prefabs/GUIPrefabs";
    private static string systemPrefabsPath = "Prefabs/SystemPrefabs";

    public static T FindOrCreateGuiState<T>() where T : GuiState {
		GuiState guiState = (GuiState)Object.FindObjectOfType(typeof(T));

		if (guiState == null) {
			StateManager stateManager = GameAccesPoint.Instance.managerSystem.stateManager;
			if (stateManager != null) {
				foreach (GuiState state in stateManager.guiStates) {
					if (state is T) {
						guiState = state;
					}
				}
            }
		}

		if (guiState == null) {
			GuiState prefab = (GuiState)Resources.Load(guiPrefabsPath + "/" + typeof(T).Name, typeof(GuiState));

			if (prefab != null) {
				guiState = (GuiState)Object.Instantiate(prefab);
				guiState.Initialize();
			}
		}

		return (T)System.Convert.ChangeType(guiState, typeof(T));
	}

    public static T FindOrCreateController<T>() where T : BaseController {
        BaseController baseController = (BaseController)Object.FindObjectOfType(typeof(T));

        if (baseController == null) {
            BaseController prefab = (BaseController)Resources.Load(systemPrefabsPath + "/" + typeof(T).Name, typeof(BaseController));

            if (prefab != null) {
                baseController = (BaseController)Object.Instantiate(prefab);
                Object.DontDestroyOnLoad(baseController);
            }
        }

        baseController.transform.SetParent(FindOrCreateControllerHolder().transform);

        return (T)System.Convert.ChangeType(baseController, typeof(T));
    }

    private static GameObject FindOrCreateControllerHolder() {
        GameObject controllerHolder = GameObject.Find("ControllerHolder");

        if (controllerHolder == null) {
            controllerHolder = new GameObject("ControllerHolder");
        }

        return controllerHolder;
    }
}