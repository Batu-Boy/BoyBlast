using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BaseAction: IAction
{
    [SerializeField] protected string Name;
    [field: SerializeField] public float Delay { get; set; }
    [field: SerializeField] public float Duration { get; set;}

    public virtual void Execute(BlockGraphic blockGraphic)
    {
        
        
    }

}


