using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : Element, IDamageable
{
    [field: SerializeField] public int Health { get; set; }

    [SerializeField] private Sprite[] crateSprites;
    [SerializeField] public CrateGraphic crateGraphic;
    
    public void Initialize(int health, Cell currentCell)
    {
        base.Initialize(currentCell);
        ElementGraphic = crateGraphic;
        Health = health;
        _currentIconIndex = Health;
        CanFall = false;
        crateGraphic.InitializeGraphic(this, crateSprites, Health);
    }
    
    public void Initialize(int health, Cell currentCell,Vector3Int pos)
    {
        base.Initialize(currentCell);
        ElementGraphic = crateGraphic;
        Health = health;
        _currentIconIndex = Health;
        CanFall = false;
        crateGraphic.InitializeGraphic(this, crateSprites, Health, pos);
    }
    
    public void TakeDamage(Cell from, int amount = 1)
    {
        Health -= amount;
        if (Health <= 0)
        {
            Health = 0;
            Destroy(from);
        }

        _currentIconIndex = Health;
    }
}
