using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField] private List<string> keys = new List<string>();
    [SerializeField] private List<KeyCode> values = new List<KeyCode>();
    public Dictionary<string, KeyCode> table = new Dictionary<string, KeyCode>();

    public void AddTable(string key)
    {
        if(!table.ContainsKey(key))
            table.Add(key, KeyCode.None);
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
