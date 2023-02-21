using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAction
{
    public float Delay { get; set; }
    public float Duration { get; set;}
    public Action m_Action { get; set; }
    public void Execute();
}
