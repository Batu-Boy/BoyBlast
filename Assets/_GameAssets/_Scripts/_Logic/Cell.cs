using System;
using JetBrains.Annotations;
using UnityEngine;

[Serializable]
public class Cell
{
    private readonly int _x;
    private readonly int _y;
    private readonly Grid _grid;

    [SerializeReference] private Element _element;
    public bool IsEmpty => _element == null;
    
    [NonSerialized]
    //DOWN = 0, LEFT = 1, UP = 2, RIGHT = 3
    private Cell[] _neighbors = new Cell[4];

    public Cell(int x, int y, Grid grid)
    {
        _x = x;
        _y = y;
        _grid = grid;
        _element = null;
    }

    public Cell(int x, int y, Grid grid, Element element)
    {
        _x = x;
        _y = y;
        _grid = grid;
        _element = element;
    }

    public Element GetElement() => _element;
    public void SetElement(Element element)
    {
        _element = element;
    }
    
    public bool TryGetElementAs<T>(out T outElement) where T : Element
    {
        var element = GetElement();
        outElement = element as T;
        if (!element) return false;
        
        return element.GetType() == typeof(T);
    }

    public Vector3Int GetPosition() => new Vector3Int(_x, _y);
    public Cell GetBottomCell() => _grid.GetCell(_x, _y - 1);
    public Cell GetUpperCell() => _grid.GetCell(_x, _y + 1);

    public void DestroyCell()
    {
        ClearCell();
    }
    
    public void ClearCell() => _element = null;
    
    //DOWN = 0, LEFT = 1, UP = 2, RIGHT = 3
    public Cell[] GetNeighbors() => _neighbors;
    public void SetNeighbors(Cell[] neighbors)
    {
        _neighbors = neighbors;
    }
    
    public Cell GetNeighborAt(int directionIndex) => _neighbors[directionIndex];
    public void SetNeighborAt(Cell neighbor, int directionIndex)
    {
        _neighbors[directionIndex] = neighbor;
    }

}
