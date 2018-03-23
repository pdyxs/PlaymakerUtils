using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FSMWrapper), true)]
public class FSMWrapperEditor : Editor {

	public FSMWrapper fsmWrapper
	{
		get
		{
			return target as FSMWrapper;
		}
	}
	
	protected override void OnHeaderGUI()
	{
		if (serializedObject.FindProperty("m_fsm").objectReferenceValue == null)
		{
			serializedObject.FindProperty("m_fsm").objectReferenceValue =
				fsmWrapper.GetComponent<PlayMakerFSM>();
		}
		
		base.OnHeaderGUI();
	}
}
