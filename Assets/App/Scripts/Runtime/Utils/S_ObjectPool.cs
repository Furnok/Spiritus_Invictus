using System.Collections.Generic;
using UnityEngine;

public class S_ObjectPool<T> where T : MonoBehaviour
{
    private readonly T prefab;
    private readonly Transform parentTransform;
    private readonly Queue<T> pool = new();

    public int Count => pool.Count;
    public bool AllowExpand { get; set; } = true;

    public S_ObjectPool(T prefab, int initialSize, Transform parentTransform = null)
    {
        if (prefab == null)
        {
            return;
        }

        this.prefab = prefab;
        this.parentTransform = parentTransform;

        for (int i = 0; i < initialSize; i++)
        {
            T instance = Object.Instantiate(prefab, parentTransform);
            instance.gameObject.SetActive(false);
            pool.Enqueue(instance);
        }
    }

    public T Get()
    {
        if (pool.Count == 0)
        {
            if (!AllowExpand)
            {
                return null;
            }

            T extra = Object.Instantiate(prefab, parentTransform);
            extra.gameObject.SetActive(true);
            return extra;
        }

        T instance = pool.Dequeue();

        if (instance == null)
        {
            return Object.Instantiate(prefab, parentTransform);
        }

        instance.gameObject.SetActive(true);
        return instance;
    }

    public void ReturnToPool(T instance)
    {
        if (instance == null)
        {
            return;
        }

        if (pool.Contains(instance))
        {
            return;
        }

        instance.gameObject.SetActive(false);
        pool.Enqueue(instance);
    }

    public void Prewarm(int count)
    {
        for (int i = 0; i < count; i++)
        {
            T instance = Object.Instantiate(prefab, parentTransform);
            instance.gameObject.SetActive(false);
            pool.Enqueue(instance);
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
    }
}