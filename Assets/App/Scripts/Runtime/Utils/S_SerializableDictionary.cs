using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class S_SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> keys = new();
    [SerializeField] private List<TValue> values = new();

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        keys.Capacity = this.Count;
        values.Capacity = this.Count;

        foreach (var kvp in this)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        this.Clear();

        int count = Mathf.Min(keys.Count, values.Count);

        for (int i = 0; i < count; i++)
        {
            if (!this.ContainsKey(keys[i]))
            {
                this.Add(keys[i], values[i]);
            }
        }
    }
}