using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class RuleController : ControllerBase
{
    [Header("Editor Options")]
    [SerializeField] private bool canWin = true;
    
    [Header("References")]
    [SerializeField] private RuleViewModel _ruleViewModel;

    [Header("Debug")]
    [SerializeField] private LevelRule _levelRule;


    private bool end;
    
    public bool HasMove => _levelRule.MoveCount > 0;

    public override void Initialize()
    {
        base.Initialize();
        EventManager.OnBlockGroupDestroy.AddListener(OnBlockGroupDestroyed);
        EventManager.OnElementsExplode.AddListener(OnElementsExplode);
        EventManager.OnValidMove.AddListener(OnValidMoveMade);
    }


    public void OnGridInit()
    {
        _levelRule = GridInitializer.LevelModel.LevelRule.Clone(new FieldsCloner(),false);

        foreach (var goal in _levelRule.GoalList)
        {
            _ruleViewModel.AddGoalToLayout(goal);
        }
        _ruleViewModel.UpdateMoveText(_levelRule.MoveCount);
    }
    
    private void OnElementsExplode(List<Element> elements, Bomb bomb)
    {
        foreach (var element in elements)
        {
            if (element is Block block)
            {
                OnBlockDestroyed(block);
            }
        }
    }

    private void OnBlockDestroyed(Block block)
    {
        for (var i = 0; i < _levelRule.GoalList.Count; i++)
        {
            var goal = _levelRule.GoalList[i];
            if (goal.BlockColor == block.Color)
            {
                goal.DecreaseCount(1);
                _ruleViewModel.UpdateGoal(goal);
                
                if (goal.Success)
                {
                    _levelRule.GoalList.Remove(goal);
                    CheckLogicWin();
                }
            }
        }
    }
    
    private void OnBlockGroupDestroyed(BlockGroup blockGroup, Cell cell)
    {
        for (var i = 0; i < _levelRule.GoalList.Count; i++)
        {
            var goal = _levelRule.GoalList[i];
            if (goal.BlockColor == blockGroup.Color)
            {
                goal.DecreaseCount(blockGroup.list.Count);
                _ruleViewModel.UpdateGoal(goal);
                
                if (goal.Success)
                {
                    _levelRule.GoalList.Remove(goal);
                    CheckLogicWin();
                }
            }
        }
    }

    private void CheckLogicWin()
    {
        if (AreGoalsSuccess())
        {
            Debug.LogWarning("WIN while last move playing... Maybe implement some extra VFXs");
        }
    }
    
    private void OnValidMoveMade()
    {
        _levelRule.MoveCount--;
        _ruleViewModel.UpdateMoveText(_levelRule.MoveCount);
        //Debug.Log("move count: "+_levelRule.MoveCount);
    }

    public override void OnStateChanged(GameStates state)
    {
        if (!canWin) return;
        if(end) return;
        
        if (state == GameStates.WaitInput)
        {
            if (AreGoalsSuccess())
            {
                end = true;
                Debug.LogWarning("YOU WIN!.. Loading Next Level");
                ParticleManager.Instance.PlayConfetti();
                AudioManager.Instance.PlaySFX(0);
                GameController.Instance.EndState(true);
                return;
            }

            if (_levelRule.MoveCount<= 0)
            {
                Debug.LogWarning("NO MOVE LEFT!.. Reloading Current Level");
                GameController.Instance.EndState(false);
                return;
            }
        }
    }
    
    public bool AreGoalsSuccess() => _levelRule.GoalList.All(goal => goal.Success);

    public void OnClear()
    {
        _ruleViewModel.OnClear();
    }

}
