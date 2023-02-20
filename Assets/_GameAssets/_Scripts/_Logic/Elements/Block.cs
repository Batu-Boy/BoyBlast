using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Block : Element, IClickable
{
    [SerializeField] public BlockGraphic blockGraphic;

    [SerializeField]
    private BlockColors blockColors; //TODO: take it to the inited script and give parameter in initialize methods

    public bool Clickable { get; set; }
    public BlockColor Color { get; private set; }
    public bool HasGroup => _group != null;

    [NonSerialized] private BlockGroup _group;

    public void Initialize(BlockColor color, Cell currentCell)
    {
        base.Initialize(currentCell);
        ElementGraphic = blockGraphic;
        CanFall = true;
        Clickable = false;
        _currentIconIndex = 0;
        Color = color;
        var icons = blockColors.list[(int)Color].Icons;
        blockGraphic.InitializeGraphic(this, icons, _currentIconIndex);
    }

    public void Initialize(BlockColor color, Cell currentCell, Vector3Int pos)
    {
        base.Initialize(currentCell);
        ElementGraphic = blockGraphic;
        CanFall = true;
        Clickable = false;
        _currentIconIndex = 0;
        Color = color;
        var icons = blockColors.list[(int)Color].Icons;
        blockGraphic.InitializeGraphic(this, icons, _currentIconIndex, pos);
    }

    public BlockGroup GetGroup()
    {
        return _group;
    }

    public void SetGroup(BlockGroup blockGroup)
    {
        _group = blockGroup;
        Clickable = true;
    }

    public void ClearGroup()
    {
        _group = null;
        Clickable = false;
    }

    public override void Destroy(Cell clickedCell)
    {
        base.Destroy(clickedCell);
        ClearGroup();
    }

    public bool TryGetBlockGroup(out BlockGroup outBlockGroup)
    {
        outBlockGroup = GetGroup();
        return GetGroup() != null;
    }

    public void ClickAction()
    {
        Debug.Log("Block Click Action");
        LogicController.Instance.OnBlockClicked(_group, _currentCell);
    }
}

[Serializable]
public class BlockGroup
{
    public BlockColor Color;
    public int ComboIndex;
    public List<Block> list = new();

    public void AddBlock(Block block)
    {
        if (block.Color != Color) return;

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
            block.ClearGroup();
        }
        UpdateCombo();
    }

    public void MergeWith(BlockGroup blockGroup)
    {
        if (blockGroup.Color != Color) return;

        foreach (var block in blockGroup.list) 
            block.SetGroup(this);
        
        list.AddRange(blockGroup.list);
        UpdateCombo();
    }

    private void UpdateCombo()
    {
        if (list.Count > GridInitializer.LevelModel.C) //Third icon
            ComboIndex = 3;
        else if (list.Count > GridInitializer.LevelModel.B) //Second icon
            ComboIndex = 2;
        else if (list.Count > GridInitializer.LevelModel.A) //First icon
            ComboIndex = 1;
        else //Default icon
            ComboIndex = 0;
    }
}