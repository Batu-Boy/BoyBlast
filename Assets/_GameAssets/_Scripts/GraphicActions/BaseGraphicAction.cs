using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BaseGraphicAction: IAction
{
    public string name;
    
    [field: Header("Settings")]
    [field: SerializeField] [field: Range(0,1)] public float Delay { get; set; }
    [field: SerializeField] [field: Range(0,1)] public float Duration { get; set;}

    public Action m_Action { get; set; }

    // ReSharper disable Unity.PerformanceAnalysis
    public void Execute()
    {
        m_Action.Invoke();
    }

    protected GraphicController _graphicController;
    
    public virtual void InitAction(GraphicController graphicController)
    {
        _graphicController = graphicController;
        name = GetType().ToString();
    }
}


