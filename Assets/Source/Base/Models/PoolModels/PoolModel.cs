using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PoolModel<T> : MonoBase where T : Component
{
    public static PoolModel<T> Instance;

    [SerializeField] private Transform _parent;
    [SerializeField] private T prefab;
    [SerializeField] private int capacity = 100;
    [SerializeField] List<T> items;
    
    public override void Initialize()
    {
        base.Initialize();
        Instance = this;
    }

    public T GetItem(Transform parent = null)
    {
        if (items.Count <= 0)
        {
            return Instantiate(prefab, parent);
        }

        var item = items[0];
        item.SetActiveGameObject(true);
        items.Remove(item);
        item.transform.SetParent(parent != null ? parent : _parent);
        return item;
    }

    public void ReturnItem(T item)
    {
        if (items.Count < capacity)
        {
            item.transform.SetParent(transform);
            item.SetActiveGameObject(false);
            items.Add(item);
        }
        else
        {
            Destroy(item.gameObject);
        }
    }
}
