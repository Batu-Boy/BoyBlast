using System;
using System.Collections.Generic;
using UnityEngine;


public class LogicController : ControllerBase
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

    #region BombExplodeLogic

    private void DestroyBombRangeElements(Bomb bomb, Cell cell)
    {
        var bombs = new List<Bomb> { bomb };

        RecurseBombs(bombs);
    }

    private void RecurseBombs(List<Bomb> bombs)
    {
        List<Element> destroyedElements = new List<Element>();
        List<Bomb> innerBombList = new List<Bomb>();

        foreach (var bomb in bombs)
        {
            DestroyedElements(bomb, ref destroyedElements, ref innerBombList);
            Debug.Log($"Exploded:{bomb.name}");
            bomb.Destroy(null);
        }
        
        if (bombs.Count > 0)
         EventManager.OnElementsExplode?.Invoke(destroyedElements, bombs);

        if (innerBombList.Count > 0)
        {
            RecurseBombs(innerBombList);
        }
    }
    
    private void DestroyedElements(Bomb bomb, ref List<Element> destroyedElements,ref List<Bomb> newBombs)
    {
        Debug.Log($"Calculating:{bomb.name}");

        var xRange = bomb.Range.x;
        var yRange = bomb.Range.y;

        var xStartOffset = Mathf.CeilToInt(xRange / 2f);
        var xFinishOffset = xRange - xStartOffset;

        var yStartOffset = Mathf.CeilToInt(yRange / 2f);
        var yFinishOffset = yRange - yStartOffset;

        var position = bomb.Position;

        for (int x = position.x - xStartOffset + 1; x < position.x + xFinishOffset + 1; x++)
        {
            for (int y = position.y - yStartOffset + 1; y < position.y + yFinishOffset + 1; y++)
            {
                if (!_grid.TryGetCell(x, y, out var currentCell)) continue;

                Element currentElement = currentCell.GetElement();
                if (!currentElement) continue;
                if (currentElement.Position == bomb.Position) continue;

                if (currentElement is Bomb currentBomb)
                {
                    if (!newBombs.Contains(currentBomb))
                    {
                        Debug.Log($"Added:{currentBomb.name}");
                        newBombs.Add(currentBomb);
                    }
                }
                else
                {
                    currentElement.Destroy(bomb.GetCell());
                    destroyedElements.Add(currentElement);
                    Debug.Log($"Element Exploded:{currentElement.name}");
                }
            }
        }
    }

    #endregion

    #region ShuffleLogic
    //Recursion for shuffle correction
    private void ShuffleGrid()
    {
        Debug.LogWarning("No Valid Move. Shuffle!");
        EventManager.OnShuffleGrid?.Invoke();
        while (true)
        {
            List<Element> instantiatedElements = new List<Element>();
            for (int x = 0; x < _grid.Width; x++)
            {
                for (int y = 0; y < _grid.Height; y++)
                {
                    if (!_grid.TryGetCell(x, y, out Cell currentCell)) continue;

                    if(!currentCell.TryGetElementAs<Block>(out var block)) continue;// shuffle only blocks
                    
                    Block newBlock = BlockPool.Instance.GetItem();//Instantiate(blockPrefab);
                    newBlock.Initialize(GridInitializer.GetRandomColor(),currentCell,
                    currentCell.GetPosition() + GridInitializer.LevelModel.N * Vector3Int.up);
                    currentCell.SetElement(newBlock);
                    instantiatedElements.Add(newBlock);
                    //Debug.Log($"Block Created:{newBlock.Position}");
                }
            }

            if (!HasAnyValidMove()) continue;
            EventManager.OnElementsInstantiate?.Invoke(instantiatedElements);
            SetGroupIcons();
            return;
        }
    }

    private bool HasAnyValidMove()
    {
        DetectGroups();
        return _blockGroups.Count > 0;
    }

    #endregion
    
    #region DestroyLogic
    
    private void DestroyBlockGroup(BlockGroup clickedBlockGroup,Cell clickedCell)
    {
        foreach (Block block in clickedBlockGroup.list)
        {
            block.Destroy(clickedCell);
        }
        
        if (clickedBlockGroup.list.Count <= 0) return;
        
        EventManager.OnBlockGroupDestroy?.Invoke(clickedBlockGroup,clickedCell);

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

    #region FillLogic
    
    private void FallExistingElements()
    {
        List<Element> fallenBlocks = new List<Element>();
        //TODO: MAYBE CHECK COLUMNS FIRST
        for (int x = 0; x < _grid.Width; x++)
        {
            for (int y = 0; y < _grid.Height - 1; y++)
            {
                if(!_grid.TryGetCell(x,y,out Cell currentCell)) continue;
                
                if(!currentCell.IsEmpty) continue;
                
                Cell upMostCell = null;
                for (int i = y; i < _grid.Height; i++)
                {
                    if(!_grid.TryGetCell(x,i,out Cell upperCell)) continue;
                    
                    if(upperCell.IsEmpty) continue;

                    upMostCell = upperCell;
                    break;
                }
                if(upMostCell == null) continue;
                
                Element upMostElement = upMostCell.GetElement();
                if (!upMostElement.canFall) continue;

                currentCell.SetElement(upMostElement);
                upMostElement.SetCell(currentCell);
                upMostCell.ClearCell();
                //Debug.LogWarning(upMostCell.GetPosition() + $"Falling to {currentCell.GetPosition()}");
                //TODO: configure as block
                fallenBlocks.Add(upMostElement);
            }
        }

        if (fallenBlocks.Count > 0)
        {
            EventManager.OnElementsFall?.Invoke(fallenBlocks);
        }
    }

    private void FillEmptyCells()
    {
        List<Element> instantiatedBlocks = new List<Element>();
        for (int x = 0; x < _grid.Width; x++)
        {
            for (int y = _grid.Height - 1; y >= 0; y--)
            {
                if (!_grid.TryGetCell(x, y, out Cell currentCell)) continue;

                if (!currentCell.IsEmpty)
                {
                    if(!currentCell.GetElement().canFall) break;
                    
                    continue;
                }
                
                Block newBlock = BlockPool.Instance.GetItem();//Instantiate(blockPrefab);
                newBlock.Initialize(GridInitializer.GetRandomColor(), currentCell,
                    currentCell.GetPosition() + GridInitializer.LevelModel.N * Vector3Int.up);
                currentCell.SetElement(newBlock);
                instantiatedBlocks.Add(newBlock);
            }
        }

        if (instantiatedBlocks.Count > 0)
        {
            EventManager.OnElementsInstantiate?.Invoke(instantiatedBlocks);
        }
    }
    
    #endregion

    #region GroupLogic
    
    [EditorButton]
    private void DetectGroups()
    {
        ClearAllGroups();
        
        for (int x = 0; x < _grid.Width; x++)
        {
            for (int y = 0; y < _grid.Height; y++)
            {
                if(!_grid.TryGetCell(x, y,out Cell currentCell)) continue;
                
                var neighbors = currentCell.GetNeighbors();
                if(!currentCell.TryGetElementAs<Block>(out var currentBlock)) continue;

                for (var i = 0; i < neighbors.Length; i++)
                {
                    if (neighbors[i] == null) continue;

                    if (!neighbors[i].TryGetElementAs<Block>(out var neighborBlock)) continue;

                    if (neighborBlock.Color == currentBlock.Color)
                    {
                        GroupAddHandler(neighborBlock, currentBlock);
                    }
                }
            }
        }
    }

    private void GroupAddHandler(Block a, Block b)
    {
        if (a.Color != b.Color)
        {
            //Debug.Log($"Type does not match {a.Position} with {b.Position}");
            return;
        }

        bool aHasGroup = a.TryGetBlockGroup(out var aGroup);
        bool bHasGroup = b.TryGetBlockGroup(out var bGroup);
        
        // neither has group
        if (!aHasGroup && !bHasGroup )
        {
            BlockGroup newGroup = new BlockGroup { Color = a.Color };
            newGroup.AddBlock(a);
            newGroup.AddBlock(b);
            _blockGroups.Add(newGroup);
            //Debug.Log($"New Group Created: {a.Position} + {b.Position}, Type: {a.Type}");
            return;
        }

        // both have groups
        if (aHasGroup && bHasGroup)
        {
            // both in same group
            if (aGroup == bGroup)
            {
                //Debug.Log($"{a.Position} and {b.Position} are on same group");
                return;
            }
            
            aGroup.MergeWith(bGroup);
            _blockGroups.Remove(bGroup);
            //Debug.Log($"Group {b.Position} merged with {a.Position}");
            return;
        }
        
        // one of them has group
        if (aHasGroup)
        {
            aGroup.AddBlock(b);
            //Debug.Log($"{b.Position} added to {aGroup.Type}");
            return;
        }
        else
        {
            bGroup.AddBlock(a);
            //Debug.Log($"{a.Position} added to {bGroup.Type}");
            return;
        }
    }

    private void ClearAllGroups()
    {
        for (int x = 0; x < _grid.Width; x++)
        {
            for (int y = 0; y < _grid.Height; y++)
            {
                //TODO: configure
                if(!_grid.TryGetElementAs<Block>(x, y, out var block)) continue;
                
                block.SetGroup(null);
            }
        }
        
        _blockGroups.Clear();
    }
    
    #endregion

    #region IconChangeLogic
    
    private void SetGroupIcons()
    {
        List<Element> iconChangedBlocks = new List<Element>();
        foreach (BlockGroup blockGroup in _blockGroups)
        {
            foreach (Block block in blockGroup.list)
            {
                block.SetIconIndex(blockGroup.ComboIndex);
                iconChangedBlocks.Add(block);
            }
        }

        if (iconChangedBlocks.Count > 0)
        {
            EventManager.OnElementsIconChange?.Invoke(iconChangedBlocks);
        }
    }

    private void SetSingleIcons()
    {
        List<Element> singleBlocks = new List<Element>();
        for (int x = 0; x < _grid.Width; x++)
        {
            for (int y = 0; y < _grid.Height; y++)
            {
                //TODO: configure
                if (!_grid.TryGetElementAs<Block>(x, y ,out var currentBlock)) continue;
                if(currentBlock.HasGroup) continue;

                currentBlock.SetDefaultIcon();
                singleBlocks.Add(currentBlock);
            }
        }

        if (singleBlocks.Count > 0)
        {
            EventManager.OnElementsIconChange?.Invoke(singleBlocks);
        }
    }
    
    #endregion
}

[Serializable]
public class BlockGroup
{
    public BlockColor Color;
    public int ComboIndex;
    public List<Block> list = new ();

    public void AddBlock(Block block)
    {
        if (block.Color != Color)
        {
            //Debug.Log($"Type of {block.Position} is not matching with {Type}");
            return;
        }

        if (!list.Contains(block))
        {
            list.Add(block);
            block.SetGroup(this);
        }
        
        UpdateCombo();
    }
    
    public void RemoveBlock(Block block)
    {
        if (list.Contains(block))
        {
            list.Remove(block);
            block.SetGroup(null);
        }
        else
        {
            //Debug.Log($"{block.Position} is not in the list of {Type}");
        }
        
        UpdateCombo();
    }

    public void MergeWith(BlockGroup blockGroup)
    {
        if (blockGroup.Color != Color)
        {
            //Debug.Log("Type does not watch while merging groups");
            return;
        }

        foreach (var block in blockGroup.list)
        {
            block.SetGroup(this);
        }
        list.AddRange(blockGroup.list);
        UpdateCombo();
    }

    private void UpdateCombo()
    {
        if (list.Count > GridInitializer.LevelModel.C) //Third icon
        {
            ComboIndex = 3;
        }
        else if (list.Count > GridInitializer.LevelModel.B) //Second icon
        {
            ComboIndex = 2;
        }
        else if (list.Count > GridInitializer.LevelModel.A) //First icon
        {
            ComboIndex = 1;
        }
        else //Default icon
        {
            ComboIndex = 0;
        }
    }
}