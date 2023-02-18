using System;
using System.Collections.Generic;
using UnityEngine;

public partial class LogicController : ControllerBase
{
    [Header("Debug")]
    [SerializeReference] private List<BlockGroup> _blockGroups = new ();
    
    private Grid _grid;

    public void OnGridInitialized(Grid grid)
    {
        _grid = grid;
        DetectGroups();
        SetGroupIcons();
    }

    private void OnDestroy()
    {
        _blockGroups.Clear();
    }

    public override void OnStateChanged(GameStates state)
    {
        if (state == GameStates.WaitInput)
        {
            if(HasAnyValidMove())
            {
                return;
            }
            GameController.Instance.ChangeState(GameStates.LogicAction);
            
            ShuffleGrid();
            
            GameController.Instance.ChangeState(GameStates.GraphicAction);
        }
    }
    
    public void CheckInput(Vector3Int floorInput)
    {
        if (!_grid.TryGetCell(floorInput, out Cell cell)) return;

        Element element = cell.GetElement();
        if(!element) return;

        if (element is Block block)
        {
            if (!block.TryGetBlockGroup(out BlockGroup blockGroup)) return;
            GameController.Instance.ChangeState(GameStates.LogicAction);
            EventManager.OnValidMove?.Invoke();
            OnBlockClicked(blockGroup, cell);
        }
        else if (element is Bomb bomb)
        {
            GameController.Instance.ChangeState(GameStates.LogicAction);
            EventManager.OnValidMove?.Invoke();
            OnBombClicked(bomb, cell);
        }
        
        ValidMoveActions();
        GameController.Instance.ChangeState(GameStates.GraphicAction);
    }
    
    private void ValidMoveActions()
    {
        FallExistingElements();
        FillEmptyCells();
        DetectGroups();//
        SetGroupIcons();
        SetSingleIcons();
    }
    
    private void OnBombClicked(Bomb bomb, Cell cell)
    {
        DestroyBombRangeElements(bomb,cell);
    }

    private void OnBlockClicked(BlockGroup blockGroup, Cell cell)
    {
        DestroyBlockGroup(blockGroup, cell);
        CheckCombo(blockGroup, cell);
    }

    //Recursion for shuffle correction

    #region DestroyLogic

    private void DestroyBlockGroup(BlockGroup clickedBlockGroup, Cell clickedCell)
    {
        foreach (Block block in clickedBlockGroup.list)
        {
            block.Destroy(clickedCell);
        }

        if (clickedBlockGroup.list.Count <= 0) return;

        EventManager.OnBlockGroupDestroy?.Invoke(clickedBlockGroup, clickedCell);

        _blockGroups.Remove(clickedBlockGroup);
    }
    
    #endregion

    #region ComboLogic
    
    private void CheckCombo(BlockGroup clickedBlockGroup, Cell clickedCell)
    {
        if (clickedBlockGroup.ComboIndex <= 0) return;

        var bomb = BombPool.Instance.GetItem();
        bomb.Initialize(clickedBlockGroup.ComboIndex - 1, clickedCell);
        clickedCell.SetElement(bomb);
        
        EventManager.OnBombCreated?.Invoke(bomb, clickedCell);
    }

    #endregion
}