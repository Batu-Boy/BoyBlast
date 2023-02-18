using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : Element, IDamageable
{
    [field: SerializeField] public int Health { get; set; }
    
    [SerializeField] private CrateGraphic crateGraphic;
    
    public void Initialize(BlockColor color, Cell currentCell)
    {
        base.Initialize(currentCell);
        ElementGraphic = crateGraphic;
        _currentIconIndex = 0;
        canFall = true;
        //Sprite[] icons = blockColors.list[(int)Color].Icons;
        //crateGraphic.InitializeGraphic(this, icons, _currentIconIndex);
    }
    
    public void Initialize(BlockColor color, Cell currentCell,Vector3Int pos)
    {
        base.Initialize(currentCell);
        ElementGraphic = crateGraphic;
        _currentIconIndex = 0;
        canFall = true;
        //Sprite[] icons = blockColors.list[(int)Color].Icons;
        //crateGraphic.InitializeGraphic(this, icons, _currentIconIndex, pos);
    }
    
    public void TakeDamage(Cell from, int amount = 1)
    {
        Health -= amount;
        if (Health <= 0)
        {
            Health = 0;
            Destroy(from);
        }
    }
}
