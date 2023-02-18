using UnityEngine;

public abstract class Element : MonoBehaviour
{
    [SerializeReference] public ElementGraphic ElementGraphic;

    public bool canFall;
    protected int _currentIconIndex;
    [SerializeField] private Cell _currentCell;

    protected void Initialize(Cell currentCell)
    {
        _currentCell = currentCell;
        Position = currentCell.GetPosition();
    }

    public Vector3Int Position { get; private set; }

    public Cell GetCell()
    {
        return _currentCell;
    }

    public void SetCell(Cell cell)
    {
        _currentCell = cell;
        Position = _currentCell.GetPosition();
    }

    public int GetIconIndex()
    {
        return _currentIconIndex;
    }

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
        if (_currentCell == null) return;
        if (!_currentCell.IsEmpty) _currentCell.ClearCell();
        _currentCell = null;
    }
}