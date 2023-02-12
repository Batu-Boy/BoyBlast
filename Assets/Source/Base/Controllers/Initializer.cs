using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBase
{
    [SerializeField] MonoBase[] sceneItems;
    [SerializeField] bool initializeOnAwake;
    private void Awake()
    {
        if (initializeOnAwake)
            Initialize();
    }

    public override void Initialize()
    {
        for (int index = 0; index < sceneItems.Length; index++)
        {
            if (sceneItems[index] == null)
            {
                Debug.LogWarning($"Item index: {index} is null. Please check references.");
                continue;
            }
            sceneItems[index].Initialize();
        }
    }
}
