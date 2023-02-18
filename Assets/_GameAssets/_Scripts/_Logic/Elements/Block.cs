using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Block : Element
{
    [SerializeField] public BlockGraphic blockGraphic;

    [SerializeField]
    private BlockColors blockColors; //TODO: take it to the inited script and give parameter in initialize methods

    [NonSerialized] private BlockGroup _group;
    public bool HasGroup => _group != null;

    public BlockColor Color { get; private set; }

    public void Initialize(BlockColor color, Cell currentCell)
    {
        base.Initialize(currentCell);
        ElementGraphic = blockGraphic;
        Color = color;
        _currentIconIndex = 0;
        canFall = true;
        var icons = blockColors.list[(int)Color].Icons;
        blockGraphic.InitializeGraphic(this, icons, _currentIconIndex);
    }

    public void Initialize(BlockColor color, Cell currentCell, Vector3Int pos)
    {
        base.Initialize(currentCell);
        ElementGraphic = blockGraphic;
        Color = color;
        _currentIconIndex = 0;
        canFall = true;
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
    }

    public override void Destroy(Cell clickedCell)
    {
        base.Destroy(clickedCell);
        SetGroup(null);
    }

    public bool TryGetBlockGroup(out BlockGroup outBlockGroup)
    {
        outBlockGroup = GetGroup();
        return GetGroup() != null;
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
            block.SetGroup(null);
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