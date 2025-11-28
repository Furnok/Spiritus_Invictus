using System.Collections.Generic;
using UnityEngine;

public class S_ObjectPool<T> where T : MonoBehaviour
{
    private readonly T prefab;
    private readonly Transform parentTransform;
    private readonly Queue<T> pool = new();
    private readonly HashSet<T> poolSet = new();

    public int Count => pool.Count;
    public bool AllowExpand { get; set; } = true;

    public System.Action<T> OnGet { get; set; }
    public System.Action<T> OnRelease { get; set; }

    public S_ObjectPool(T prefab, int initialSize, Transform parentTransform = null)
    {
        if (prefab == null)
        {
            return;
        }

        this.prefab = prefab;
        this.parentTransform = parentTransform;

        Prewarm(initialSize);
    }

    private T CreateInstance(bool active = false)
    {
        T instance = Object.Instantiate(prefab, parentTransform);
        instance.gameObject.SetActive(active);
        return instance;
    }

    public T Get()
    {
        if (prefab == null)
            return null;

        T instance;

        if (pool.Count == 0)
        {
            if (!AllowExpand)
                return null;

            instance = CreateInstance(true);
        }
        else
        {
            instance = pool.Dequeue();
            poolSet.Remove(instance);

            if (instance == null)
                instance = CreateInstance(true);
            else
                instance.gameObject.SetActive(true);
        }

        if (instance.transform.parent != parentTransform)
            instance.transform.SetParent(parentTransform);

        OnGet?.Invoke(instance);
        return instance;
    }

    public void ReturnToPool(T instance)
    {
        if (instance == null || poolSet.Contains(instance))
            return;

        OnRelease?.Invoke(instance);

        instance.gameObject.SetActive(false);
        instance.transform.SetParent(parentTransform);

        pool.Enqueue(instance);
        poolSet.Add(instance);
    }

    public void Prewarm(int count)
    {
        if (prefab == null) return;

        for (int i = 0; i < count; i++)
        {
            T instance = CreateInstance(false);
            pool.Enqueue(instance);
            poolSet.Add(instance);
        }
    }

    public void Clear()
    {
        foreach (var item in pool)
        {
            if (item != null)
            {
                Object.Destroy(item.gameObject);
            }
        }

        pool.Clear();
        poolSet.Clear();
    }
}