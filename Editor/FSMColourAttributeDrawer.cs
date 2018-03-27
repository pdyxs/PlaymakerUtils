using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(FSMColourAttribute))]
public class FSMColourAttributeDrawer : PropertyDrawer
{

	private readonly string[] popupOptions =
		{"Default", "Blue", "Cyan", "Green", "Yellow", "Orange", "Red", "Purple"};
	
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return EditorGUIUtility.singleLineHeight;
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginChangeCheck();
		property.intValue = EditorGUI.Popup(position, label.text, property.intValue, popupOptions);
		if (EditorGUI.EndChangeCheck())
		property.serializedObject.ApplyModifiedProperties();
	}
}
