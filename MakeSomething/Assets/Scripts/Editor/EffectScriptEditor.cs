using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[ExecuteInEditMode, CustomEditor(typeof(EffectScript))]
public class EffectScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var iterator = serializedObject.GetIterator();
        iterator.NextVisible(true);

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(iterator);
        EditorGUI.EndDisabledGroup();

        //isParticleSystem
        iterator.NextVisible(false);
        EditorGUILayout.PropertyField(iterator);

        var isParticleSystem = iterator.boolValue;

        for(int i = 0; i < 2; i++) 
        {
            iterator.NextVisible(false);
            if(!isParticleSystem)
                EditorGUILayout.PropertyField(iterator);
        }
        iterator.NextVisible(false);
        EditorGUILayout.PropertyField(iterator);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
