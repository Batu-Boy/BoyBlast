using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class RuleViewModel : ScreenElement
{
    [Header("References")]
    [SerializeField] private BlockColors _blockColors;
    [SerializeField] private GoalUI _goalUIPrefab;
    
    [Header("Elements")]
    [SerializeField] private TextMeshProUGUI moveText;
    [SerializeField] private Transform goalLayout;
    
    private Dictionary<Goal, GoalUI> _goalLink = new ();
    public override void Initialize()
    {
        base.Initialize();
    }

    public void UpdateMoveText(int count)
    {
        moveText.text = count.ToString();
    }

    public void AddGoalToLayout(Goal goal)
    {
        var newGoalUI = Instantiate(_goalUIPrefab, goalLayout);
        newGoalUI.Setup(_blockColors.list[(int)goal.BlockColor].Icons[0], goal.Count);

        _goalLink.Add(goal, newGoalUI);
    }

    public void UpdateGoal(Goal goal)
    {
        _goalLink[goal].UpdateCount(goal.Count);
        if (goal.Success)
        {
            _goalLink[goal].SetSuccess();
        }
    }

    public void OnClear()
    {
        foreach (var keyValuePair in _goalLink)
        {
#if UNITY_EDITOR
            DestroyImmediate(keyValuePair.Value.gameObject);
#else
            Destroy(keyValuePair.Value.gameObject);
#endif
        }
        _goalLink.Clear();
        UpdateMoveText(99);
    }
}
