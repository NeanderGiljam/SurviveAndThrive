using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(StateManager))]
public class GameStateManagerEditor : Editor {
    private int index = -1;
    private string[] options;

    StateManager manager;

    private void OnEnable() {
        manager = (StateManager)target;
        options = ResourceLoader.GetAllScenesInProject();

        if (string.IsNullOrEmpty(manager.selectedScene)) {
            index = System.Array.IndexOf(options, options[0]);
            return;
        }

        if (manager != null)
            index = System.Array.IndexOf(options, manager.selectedScene);
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        index = EditorGUILayout.Popup("Start scene", index, options);

        if (GUI.changed) {
            manager.selectedScene = options[index];
        }
    }
}
