using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[ExecuteInEditMode, CustomEditor(typeof(Player))]
public class PlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var iterator = serializedObject.GetIterator();

        iterator.NextVisible(true); //m_script
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(iterator);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.Space(10);

        for (int i = 0; i < 6; i++)
        {
            iterator.NextVisible(false);
            var currentPropertyName = iterator.name;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(iterator.name);
            if (GUILayout.Button(System.Enum.GetName(typeof(KeyCode), iterator.enumValueFlag), EditorStyles.popup))
            {
                SearchableKeycodeWindow.Open((x) =>
                {
                    serializedObject.FindProperty(currentPropertyName).enumValueFlag = (int)x;
                    serializedObject.ApplyModifiedProperties();
                });
                
            }
            EditorGUILayout.EndHorizontal();
        }

        while (iterator.NextVisible(false)) EditorGUILayout.PropertyField(iterator);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif