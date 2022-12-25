using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;

[ExecuteInEditMode, CustomEditor(typeof(InputManager))]
public class InputManagerEditor : Editor
{
    SerializedProperty _keys;
    SerializedProperty _values;
    ReorderableList list;
    int selectedIndex;

    private void OnEnable()
    {
        _keys = serializedObject.FindProperty("keys");
        _values = serializedObject.FindProperty("values");

        list = new ReorderableList(serializedObject, _values, true, true, true, true);

        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
            var i = index;

            var key = _keys.GetArrayElementAtIndex(i);
            var value = _values.GetArrayElementAtIndex(i);

            rect.y += 2;

            EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 10, EditorGUIUtility.singleLineHeight), "key : ");

            EditorGUI.BeginChangeCheck();
            var k = EditorGUI.DelayedTextField(new Rect(rect.x + rect.width / 10, rect.y, rect.width * 0.375f, EditorGUIUtility.singleLineHeight), key.stringValue);
            if(EditorGUI.EndChangeCheck()) 
            {
                if(!(target as InputManager).table.ContainsKey(k))
                {
                    var beforeValue = value.enumValueFlag;
                    _keys.GetArrayElementAtIndex(i).stringValue = k;
                    serializedObject.ApplyModifiedProperties();
                }
            }

            if(GUI.Button(new Rect(rect.x + rect.width / 2, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight), Enum.GetName(typeof(KeyCode), value.enumValueFlag), EditorStyles.popup))
            {
                SearchableKeycodeWindow.Open((x) => {
                    _values.GetArrayElementAtIndex(i).enumValueFlag = (int)x;
                    serializedObject.ApplyModifiedProperties();
                });
            }
        };

        list.onAddCallback += (list) => {
            InputManagerTableAddWindow.Open((target as InputManager).table.Keys.ToArray(), (x) => {
                (target as InputManager).AddTable(x);
                serializedObject.ApplyModifiedProperties(); //
            });
        };

        list.onRemoveCallback = list => {
            var i = list.index;

            _keys.DeleteArrayElementAtIndex(i);
            serializedObject.ApplyModifiedProperties();
        };

        list.onSelectCallback = list => {
            selectedIndex = list.index;
        };

        list.onReorderCallback = list => {
            var i = list.index;

            _keys.MoveArrayElement(selectedIndex, i);
            serializedObject.ApplyModifiedProperties();
        };
    }

    public override void OnInspectorGUI()
    {
        var iterator = serializedObject.GetIterator();
        iterator.NextVisible(true);
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(iterator);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.Space(10);

        serializedObject.Update();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
