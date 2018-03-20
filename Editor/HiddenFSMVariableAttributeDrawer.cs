using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(HiddenFSMVariableAttribute))]
public class HiddenFSMVariableAttributeDrawer : PropertyDrawer {

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
        return 0;
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
        var name = property.FindPropertyRelative("name");
        if (name != null)
        {
            name.stringValue = property.displayName;
        }
	}

}
