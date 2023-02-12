using UnityEngine;

public abstract class Element : MonoBehaviour
{
    private Cell _currentCell;
    private Vector3Int _currentPosition;

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

    public virtual void Destroy(Cell clickedCell)
    {
        if (!_currentCell.IsEmpty)
        {
            _currentCell.ClearCell();
            
        }
        _currentCell = null;
    }
}

public class TestElement : Element
{

}