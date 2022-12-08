using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[ExecuteInEditMode, CustomEditor(typeof(Slime))]
public class SlimeEditor : Editor
{
    SerializedProperty _slimeColor;

    private void OnEnable() 
    {
        _slimeColor = serializedObject.FindProperty("slimeColor");    
    }

    public override void OnInspectorGUI()
    {
        (target as Slime).GetComponent<SpriteRenderer>().color = _slimeColor.colorValue;
        serializedObject.ApplyModifiedProperties();
        
        base.OnInspectorGUI();
    }
}
#endif
