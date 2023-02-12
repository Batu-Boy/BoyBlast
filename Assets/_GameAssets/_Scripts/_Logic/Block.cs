using System;
using UnityEngine;

[Serializable]
public class Block : Element
{
    [SerializeField] public BlockGraphic _blockGraphic;
    [SerializeField] private BlockColors blockColors;

    [NonSerialized]
    private BlockGroup _group;
    public bool HasGroup => _group != null;
    
    private int _currentIconIndex;
    
    private BlockColor _blockColor;
    public BlockColor Color => _blockColor;
    
    public void Initialize(BlockColor color, Cell currentCell)
    {
        base.Initialize(currentCell);
        
        _blockColor = color;
        _currentIconIndex = 0;
        canFall = true;
        Sprite[] icons = blockColors.list[(int)Color].Icons;
        _blockGraphic.InitializeGraphic(this, icons, _currentIconIndex);
    }
    
    public void Initialize(BlockColor color, Cell currentCell,Vector3Int pos)
    {
        base.Initialize(currentCell);
        
        _blockColor = color;
        _currentIconIndex = 0;
        canFall = true;
        Sprite[] icons = blockColors.list[(int)Color].Icons;
        _blockGraphic.InitializeGraphic(this, icons, _currentIconIndex, pos);
    }

    public int GetIconIndex() => _currentIconIndex;
    public void SetIconIndex(int iconIndex)
    {
        _currentIconIndex = iconIndex;
    }
    public void SetDefaultIcon()
    {
        _currentIconIndex = 0;
    }

    public BlockGroup GetGroup() => _group;
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
