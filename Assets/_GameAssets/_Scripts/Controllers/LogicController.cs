using System;
using System.Collections.Generic;
using UnityEngine;

public partial class LogicController : ControllerBase
{
    public static LogicController Instance;
    
    [Header("Debug")]
    [SerializeReference] private List<BlockGroup> _blockGroups = new ();
    
    private Grid _grid;

    public void OnGridInitialized(Grid grid)
    {
        Instance = this;
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
        //if(!element) return;
        if (element is IClickable { Clickable: true } clickable)
        {
            GameController.Instance.ChangeState(GameStates.LogicAction);
            EventManager.OnValidMove?.Invoke();
            
            clickable.ClickAction();
            
            ValidMoveActions();
            GameController.Instance.ChangeState(GameStates.GraphicAction);
        }
    }
    
    private void ValidMoveActions()
    {
        FallExistingElements();
        DetectGroups();//
        SetGroupIcons();
        SetSingleIcons();
        FillEmptyCells();
        DetectGroups();//
        SetGroupIcons();
        SetSingleIcons();
    }
    
    public void OnBombClicked(Bomb bomb, Cell cell)
    {
        DestroyBombRangeElements(bomb,cell);
    }

    public void OnBlockClicked(BlockGroup blockGroup, Cell cell)
    {
        DestroyBlockGroup(blockGroup, cell);
        CheckCombo(blockGroup, cell);
    }

    //Recursion for shuffle correction

    #region DestroyLogic

    private void DestroyBlockGroup(BlockGroup clickedBlockGroup, Cell clickedCell)
    {
        if (clickedBlockGroup.list.Count <= 0) return;
        List<Element> damagedElements = new List<Element>();
        foreach (Block block in clickedBlockGroup.list)
        {
            DamageNeighbors(block.GetCell(),ref damagedElements);
            block.Destroy(clickedCell);
        }
        
        EventManager.OnElementsDamaged?.Invoke(damagedElements);
        EventManager.OnBlockGroupDestroy?.Invoke(clickedBlockGroup, clickedCell);
    }
    
    #endregion
    
    #region DamageNeigborLogic
    
    //TODO: implement
    private void DamageNeighbors(Cell currentCell, ref List<Element> damagedElements)
    {
        var neighborCells = currentCell.GetNeighbors();
        foreach (var neighborCell in neighborCells)
        {
            if(neighborCell == null) continue;

            var currentElement = neighborCell.GetElement();
            if (currentElement is IDamageable damageable)
            {
                if (damagedElements.Contains(currentElement)) continue;
                
                Debug.Log($"{neighborCell.GetPosition()} damaged");
                damageable.TakeDamage(currentCell);
                damagedElements.Add(currentElement);
            }
        }
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