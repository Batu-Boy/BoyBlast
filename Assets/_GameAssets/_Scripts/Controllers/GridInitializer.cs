using UnityEngine;
using UnityEngine.Events;

public class GridInitializer : MonoBase
{
    [Header("Events")] public UnityEvent<Grid> OnGridInitialized;
    public UnityEvent OnClear;

    [SerializeField] private Block blockPrefab;
    [SerializeField] private Crate cratePrefab;

    [Header("Debug")] [SerializeField] private bool gizmos;
    [SerializeField] private int Rows;
    [SerializeField] private int Columns;

    private Grid _grid;

    public static LevelModel LevelModel;

    public void InitializeGrid(LevelModel levelModel)
    {
        LevelModel = levelModel;

        if (!levelModel.IsRandom)
            //TODO: make editor level maker later.
            //InitDefinedGrid(levelModel);
            InitRandomGrid(levelModel);
        else
            InitRandomGrid(levelModel);

        OnGridInitialized?.Invoke(_grid);
    }

    private void InitRandomGrid(LevelModel levelModel)
    {
        Rows = levelModel.M;
        Columns = levelModel.N;
        _grid = new Grid(Rows, Columns);

        for (var x = 0; x < Rows; x++)
        for (var y = 0; y < Columns; y++)
        {
            var cell = _grid.GetCell(x, y);
            Element element;
            if (x == 0)
            {
                var crate = Instantiate(cratePrefab);
                crate.Initialize(3,cell);
                element = crate;
            }
            else
            {
                var block = Instantiate(blockPrefab);
                block.Initialize(GetRandomColor(), cell);
                element = block;
            }
            
            cell.SetElement(element);
        }

        ArrangeNeighbors();
    }

    //DOWN = 0, LEFT = 1, UP = 2, RIGHT = 3
    private void ArrangeNeighbors()
    {
        for (var x = 0; x < Rows; x++)
        for (var y = 0; y < Columns; y++)
        {
            //Don't need controls actually
            var tempNeighbors = new Cell[] { null, null, null, null };
            tempNeighbors[0] = _grid.GetCell(x, y - 1);
            tempNeighbors[1] = _grid.GetCell(x - 1, y);
            tempNeighbors[2] = _grid.GetCell(x, y + 1);
            tempNeighbors[3] = _grid.GetCell(x + 1, y);

            for (var i = 0; i < tempNeighbors.Length; i++)
            {
                Debug.Log($"{i}:{tempNeighbors[i]}");
            }
            
            _grid.GetCell(x, y).SetNeighbors(tempNeighbors);
        }
    }

    private void InitDefinedGrid(LevelModel levelModel)
    {
        Debug.Log("DefinedGrid");
    }

    public void ClearScene()
    {
        OnClear?.Invoke();
        _grid = null;
    }

    public static BlockColor GetRandomColor()
    {
        return LevelModel.SelectedColors.GetRandom();
    }

    private void OnDrawGizmos()
    {
        if (!gizmos) return;
        if (_grid == null) return;

        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube(new Vector3(_grid.Width / 2f - .5f, _grid.Height / 2f - .5f),
            new Vector3(_grid.Width, _grid.Height));

        var offset = new Vector3(-.5f, -.5f);

        Gizmos.color = new Color(.3f, .3f, .3f, 1);
        for (var x = 0; x < _grid.Width; x++)
        for (var y = 0; y < Columns; y++)
        {
            var leftDownPos = new Vector3(x, y) + offset;
            Gizmos.DrawLine(leftDownPos, leftDownPos + Directions.UP);
            Gizmos.DrawLine(leftDownPos, leftDownPos + Directions.RIGHT);
            var rightUpPos = leftDownPos + Directions.RIGHT + Directions.UP;
            Gizmos.DrawLine(rightUpPos, rightUpPos + Directions.DOWN);
            Gizmos.DrawLine(rightUpPos, rightUpPos + Directions.LEFT);
        }
    }
}