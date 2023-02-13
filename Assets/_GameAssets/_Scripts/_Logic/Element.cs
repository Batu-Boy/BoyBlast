using System;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Element : MonoBehaviour
{
    [SerializeReference] public ElementGraphic ElementGraphic;
    
    private Cell _currentCell;
    private Vector3Int _currentPosition;
    protected int _currentIconIndex;
    public bool canFall;

    protected void Initialize(Cell currentCell)
    {
        _currentCell = currentCell;
        _currentPosition = currentCell.GetPosition();
    }

    public Vector3Int Position => _currentPosition;

    public Cell GetCell() => _currentCell;
    public void SetCell(Cell cell)
    {
        _currentCell = cell;
        _currentPosition = _currentCell.GetPosition();
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

    public virtual void Destroy(Cell clickedCell)
    {
        if (!_currentCell.IsEmpty)
        {
            _currentCell.ClearCell();
            
        }
        _currentCell = null;
    }
}
