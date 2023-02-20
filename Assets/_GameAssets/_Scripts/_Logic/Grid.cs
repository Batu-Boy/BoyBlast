using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Grid
{
    public int Width { get; }
    public int Height { get; }
    
    private readonly Cell[,] _gridArray;

    private readonly CellGroup[] Rows;
    private readonly CellGroup[] Columns;
    
    public Grid(int width, int height)
    {
        Width = width;
        Height = height;
        
        Rows = new CellGroup[Height];
        Columns = new CellGroup[Width];

        for (int i = 0; i < Height; i++)
        {
            Rows[i] = new CellGroup();
        }

        for (int j = 0; j < Width; j++)
        {
            Columns[j] = new CellGroup();
        }
        
        _gridArray = new Cell[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = new Cell(x, y, this);
                _gridArray[x, y] = cell;
                Rows[y].Add(cell);
                Columns[x].Add(cell);
            }
        }
    }
    
    public Cell GetCell(int x, int y)
    {
        if (x >= Width || y >= Height || x < 0 || y < 0)
        {
            return null;
        }
        
        return _gridArray[x, y];
    }
    
    private Cell GetCell(Vector3Int pos)
    {
        if (pos.x >= Width || pos.y >= Height || pos.x < 0 || pos.y < 0)
        {
            return null;
        }
        return _gridArray[pos.x, pos.y];
    }

    /*public Element GetElementAt(int x, int y)
    {
        if (x >= Width || y >= Height || x < 0 || y < 0)
        {
            return null;
        }
        
        return _gridArray[x, y].GetElement();
    }
    
    public bool TryGetElementAt(int x, int y, out Element element)
    {
        if (x >= Width || y >= Height || x < 0 || y < 0)
        {
            element = null;
            return false;
        }

        element = _gridArray[x, y].GetElement();
        return true;
    }*/
    
    public bool TryGetElementAs<T>(int x, int y, out T outElement) where T : Element
    {
        if (x >= Width || y >= Height || x < 0 || y < 0)
        {
            outElement = null;
            return false;
        }
        
        var success = _gridArray[x, y].TryGetElementAs<T>(out var element);

        outElement = element;
        return success;
    }

    public CellGroup GetColumn(int columnNumber)
    {
        return Columns[columnNumber];
    }

    public CellGroup GetRow(int rowNumber)
    {
        return Rows[rowNumber];
    }

    public Cell[,] GetArray()
    {
        return _gridArray;
    }

    public void ClearAllCells()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int i = 0; i < Height; i++)
            {
                
            }
        }
    }

    public List<Cell> GetBelowCells(Cell cell)
    {
        var currentColumn = Columns[cell.GetPosition().x].list;
        var tempList = new List<Cell>();
        for (int i = cell.GetPosition().y -1; i >= 0; i--)
        {
            tempList.Add(currentColumn[i]);
        }

        return tempList;
    }

    public bool TryGetCell(Vector3Int position, out Cell outCell)
    {
        if (position.x >= Width || position.y >= Height || position.x < 0 || position.y < 0)
        {
            Debug.Log($"The {position} is not on Grid");
            outCell = null;
            return false;
        }
        else
        {
            outCell = GetCell(position);
            return true;
        }
    }
    
    public bool TryGetCell(int x, int y, out Cell outCell)
    {
        if (x >= Width || y >= Height || x < 0 || y < 0)
        {
            //Debug.Log($"The {x},{y} is not on Grid");
            outCell = null;
            return false;
        }
        else
        {
            outCell = GetCell(x,y);
            return true;
        }
    }
}

public class CellGroup
{
    public List<Cell> list;

    public CellGroup()
    {
        list = new List<Cell>();
    }

    public void Add(Cell cell)
    {
        if (!list.Contains(cell))
        {
            list.Add(cell);
        }
    }
}
