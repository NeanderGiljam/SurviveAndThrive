using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(ConsumableStat))]
public class ConsumableStatsEditor : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		//base.OnGUI(position, property, label);

		SerializedProperty statToFill = property.FindPropertyRelative("statToFill");
		SerializedProperty value = property.FindPropertyRelative("value");

		EditorGUI.LabelField(new Rect(position.x, position.y, position.width * 0.2f, position.height), "Stat");
		EditorGUI.PropertyField(new Rect(position.x + (position.width * 0.2f), position.y, position.width * 0.3f, position.height), statToFill, GUIContent.none);
		EditorGUI.LabelField(new Rect(position.x + (position.width / 2), position.y, position.width * 0.2f, position.height), "Value");
		EditorGUI.PropertyField(new Rect(position.x + ((position.width / 2) + (position.width * 0.2f)), position.y, position.width * 0.3f, position.height), value, GUIContent.none);
	}
}