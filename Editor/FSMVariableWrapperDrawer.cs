using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(FSMVariableWrapper), true)]
public class FSMVariableWrapperDrawer : PropertyDrawer {

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!Application.isPlaying) {
            var prop = property.FindPropertyRelative("initialVal");
            if (prop != null) {
                return EditorGUI.GetPropertyHeight(prop);
            }
        }
        return 0;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var prop = property.FindPropertyRelative("initialVal");
        if (Application.isPlaying || prop == null)
        {
            return;
        }

        EditorGUI.PropertyField(position, prop, label);

        var name = property.FindPropertyRelative("name");
        if (name != null) {
            name.stringValue = property.displayName;
        }
    }
}
