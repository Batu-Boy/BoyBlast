using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Element
{
    [field: SerializeField] public Vector2Int Range { get; set; }
    [SerializeField] public BombGraphic bombGraphic;
    [SerializeField] private Sprite[] icons;
    
    public void Initialize(int iconIndex, Cell currentCell)
    {
        base.Initialize(currentCell);
        ElementGraphic = bombGraphic;
        _currentIconIndex = iconIndex;
        canFall = true;
        bombGraphic.InitializeGraphic(this, icons, _currentIconIndex);
    }
    
    public void Initialize(int iconIndex, Cell currentCell,Vector3Int pos)
    {
        base.Initialize(currentCell);
        ElementGraphic = bombGraphic;
        _currentIconIndex = 0;
        canFall = true;
        bombGraphic.InitializeGraphic(this, icons, _currentIconIndex, pos);
    }
    
}
