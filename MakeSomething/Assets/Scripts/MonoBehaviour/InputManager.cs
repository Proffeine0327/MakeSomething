using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour, ISerializationCallbackReceiver
{
    private static InputManager manager;

    [SerializeField] private List<string> keys = new List<string>();
    [SerializeField] private List<KeyCode> values = new List<KeyCode>();
    public Dictionary<string, KeyCode> table = new Dictionary<string, KeyCode>();

    public static bool GetKey(string key) => Input.GetKey(manager.table[key]);
    public static bool GetKeyDown(string key) => Input.GetKeyDown(manager.table[key]);
    public static bool GetKeyUp(string key) => Input.GetKeyUp(manager.table[key]);

    private void Awake() 
    {
        if(manager == null) manager = this;
    }

    public void AddTable(string key)
    {
        if(!table.ContainsKey(key))
            table.Add(key, KeyCode.None);
    }

    public void RemoveTable(string key)
    {
        table.Remove(key);
    }

    public void ChangeValue(string targetKey, KeyCode changeValue)
    {
        if(!table.ContainsKey(targetKey))
            throw new ArgumentNullException();

        table[targetKey] = changeValue;
    }

    public void ChangeTableKey(string targetKey, string changeKey)
    {
        if(!table.ContainsKey(targetKey))
            throw new ArgumentNullException();
        
        table.Remove(targetKey);
        table.Add(changeKey, KeyCode.None);
    }

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        
        foreach(var kvp in table)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        table = new Dictionary<string, KeyCode>();

        for(int i = 0; i < Mathf.Min(keys.Count, values.Count); i++) table.Add(keys[i], values[i]);
    }
}
