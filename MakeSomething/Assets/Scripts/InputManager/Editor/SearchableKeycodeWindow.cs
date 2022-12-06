using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class SearchableKeycodeWindow : EditorWindow
{
    public static void Open(Action<KeyCode> action)
    {
        var window = EditorWindow.CreateInstance<SearchableKeycodeWindow>();

        window.action = action;
        window.elements = Enum.GetNames(typeof(KeyCode));

        var mousePos = EditorGUIUtility.GUIToScreenPoint(Event.current.mousePosition);
        window.ShowAsDropDown(new Rect(mousePos, Vector2.zero), new Vector2(200,300));
    }

    Action<KeyCode> action;
    string[] elements;
    string search;
    Vector2 scrollPos = new Vector2();

    private void OnGUI() 
    {
        search = EditorGUILayout.TextField(search, EditorStyles.toolbarSearchField);

        var showElements = elements;
        
        if(!string.IsNullOrEmpty(search))
        {
            var searchElements = 
                from ele in showElements
                where ele.Contains(search, StringComparison.OrdinalIgnoreCase)
                orderby ele.Length
                select ele;
            
            showElements = searchElements.ToArray();
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        for(int i = 0; i < showElements.Length; i++)
        {
            if(GUILayout.Button(showElements[i]))
            {
                action.Invoke((KeyCode)Enum.Parse(typeof(KeyCode), showElements[i]));
                this.Close();
            }
        }
        EditorGUILayout.EndScrollView();
    }
}
#endif