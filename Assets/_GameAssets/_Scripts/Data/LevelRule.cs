using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class LevelRule
{
    public int MoveCount = 99;
    public List<Goal> GoalList = new ();
}

[Serializable]
public class Goal
{
    public BlockColor BlockColor = BlockColor.Blue;
    public int Count;
    public bool Success;

    public void DecreaseCount(int amount)
    {
        if (Success)
        {
            //Debug.LogWarning($"GoalColor {BlockColor} is already succeeded");
            return;
        }
        Count -= amount;
        if (Count <= 0)
        {
            Count = 0;
            Success = true;
            //Debug.Log($"{BlockColor} goal Success");
        }
    }
}
