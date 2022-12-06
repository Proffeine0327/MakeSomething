using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class InputManagerTableAddWindow : EditorWindow
{
    public static void Open(string[] tableKeys, Action<string> action)
    {
        var window = EditorWindow.CreateInstance<InputManagerTableAddWindow>();
        window.tableKeys = tableKeys;
        window.action = action;

        var mousePos = EditorGUIUtility.GUIToScreenPoint(Event.current.mousePosition);
        window.ShowAsDropDown(new Rect(mousePos, Vector2.zero), new Vector2(250,20));
    }

    Action<string> action;
    string key;
    string[] tableKeys;

    private void OnGUI() 
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("key : ", GUILayout.Width(30));
        key = EditorGUILayout.TextField(key);

        EditorGUI.BeginDisabledGroup(tableKeys.Contains(key));
        if(GUILayout.Button("Add", GUILayout.MaxWidth(40)))
        {
            action.Invoke(key);
            this.Close();
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();
    }
}
#endif