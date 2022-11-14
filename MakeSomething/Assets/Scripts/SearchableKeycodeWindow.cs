#if UNITY_EDITOR
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SearchableKeycodeWindow : EditorWindow
{
    public static void Open(Action<KeyCode> ac)
    {
        var window = EditorWindow.CreateInstance<SearchableKeycodeWindow>();

        window.ac = ac;
        window.elements = Enum.GetNames(typeof(KeyCode));
        
        var mousePos = EditorGUIUtility.GUIToScreenPoint(Event.current.mousePosition);
        window.ShowAsDropDown(new Rect(mousePos, Vector2.zero), new Vector2(200,300));
    }

    string search;
    Action<KeyCode> ac;
    string[] elements;

    Vector2 scrollPos = new Vector2();

    private void OnGUI() 
    {
        EditorGUI.BeginChangeCheck();
        search = EditorGUILayout.TextField(search, EditorStyles.toolbarSearchField);
        if(EditorGUI.EndChangeCheck())
            scrollPos = new Vector2(0,0);

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
                ac.Invoke((KeyCode)Enum.Parse(typeof(KeyCode), showElements[i]));
                this.Close();
            }
        }
        EditorGUILayout.EndScrollView();
    }
}
#endif