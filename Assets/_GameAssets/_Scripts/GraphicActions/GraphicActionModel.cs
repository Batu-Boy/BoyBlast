using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GraphicActionModel : MonoBehaviour
{
    [FormerlySerializedAs("_destroyAction")] [SerializeField] public DefaultDestroyAction defaultDestroyAction;
    [SerializeField] private List<BaseAction> actionList;
    
    public Dictionary<Block, BlockGraphic> BlockLink = new();
    
    public float ExecuteActions()
    {
        var totalDuration = GetTotalDuration();
        StartCoroutine(ExecuteCoroutine());
        return totalDuration;
    }
    
    private IEnumerator ExecuteCoroutine()
    {
        for (var i = 0; i < actionList.Count; i++)
        {
            yield return new WaitForSeconds(actionList[i].Delay);
            //actionList[i].Execute();
            yield return new WaitForSeconds(actionList[i].Duration);
        }
    }
    
    private float GetTotalDuration()
    {
        float totalDuration = 0;
        foreach (var action in actionList)
        {
            totalDuration += action.Delay;
            totalDuration += action.Duration;
        }

        return totalDuration;
    }
}
