using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : ControllerBase
{
    [SerializeField] private LogicController _logicController;
    [SerializeField] private Camera mainCam;
    
    public override void Initialize()
    {
        base.Initialize();
        mainCam = Camera.main;
    }

    public override void ControllerUpdate(GameStates currentState)
    {
        if (currentState != GameStates.WaitInput) return;

        if (!Input.GetMouseButtonDown(0)) return;
        
        var mousePos = Input.mousePosition;
        mousePos.z = 0;
        var mouseWorldPos = mainCam.ScreenToWorldPoint(mousePos);
        Vector3Int floorInput = new Vector3Int(Mathf.FloorToInt(mouseWorldPos.x), Mathf.FloorToInt(mouseWorldPos.y));
        _logicController.CheckInput(floorInput);
    }
}
