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
        int count = Count;
        if (keys.Count != count) keys = new List<TKey>(count);
        if (values.Count != count) values = new List<TValue>(count);

        keys.Clear();
        values.Clear();

        foreach (var pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        Clear();

        int count = Mathf.Min(keys?.Count ?? 0, values?.Count ?? 0);

        for (int i = 0; i < count; i++)
        {
            var key = keys[i];

            if (key == null) continue;

            if (!ContainsKey(key)) Add(key, values[i]);
        }
    }
}