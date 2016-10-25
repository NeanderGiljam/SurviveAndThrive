using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(SceneState), true)]
public class SceneStateEditor : Editor {
    private int index = -1;
    private string[] options;

    SceneState manager;

    private void OnEnable() {
        manager = (SceneState)target;
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

        index = EditorGUILayout.Popup("Scene State", index, options);

        if (GUI.changed) {
            manager.selectedScene = options[index];
        }
    }
}
