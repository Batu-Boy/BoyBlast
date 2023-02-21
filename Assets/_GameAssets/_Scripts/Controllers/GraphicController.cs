using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicController : ControllerBase
{
    [Header("Settings")] 
    [SerializeField] private GraphicActionSettings ActionSettings;

    [Header("References")] 
    [SerializeField] private BlockColors blockColors;
    [SerializeField] private GraphicData _graphicData;

    [Header("Parents")] 
    [SerializeField] private Transform cellParent;

    [Space]
    [SerializeReference] public List<BaseGraphicAction> GraphicActions;

    [HideInInspector] public List<ElementGraphic> _elements = new();
    private GridGraphic _gridGraphic;
    private readonly List<CellGraphic> _cells = new();

    public override void Initialize()
    {
        base.Initialize();
        EventManager.ClearAllListeners();
        ActionSettings.Initialize(this);
    }

    public void OnGridInitialized(Grid grid)
    {
        _gridGraphic = null;
        _cells.Clear();
        _elements.Clear();

        _gridGraphic = Instantiate(_graphicData.gridPrefab);
        _gridGraphic.InitializeGraphic(new Vector3Int(grid.Width, grid.Height, 1),
            _graphicData.defaultGridSprite);

        for (var x = 0; x < grid.Width; x++)
        for (var y = 0; y < grid.Height; y++)
        {
            var cell = Instantiate(_graphicData.cellPrefab, cellParent);
            cell.InitializeGraphic(grid.GetCell(x, y), _graphicData.defaultCellSprite);
            _cells.Add(cell);

            //TODO: config
            if (grid.TryGetElementAs<Block>(x, y, out var element))
            {
                var blockGraphic = element.blockGraphic;
                var icons = blockColors.list[(int)element.Color].Icons;
                blockGraphic.InitializeGraphic(element, icons, element.GetIconIndex());
                _elements.Add(blockGraphic);
            }
        }
    }

    public override void OnStateChanged(GameStates state)
    {
        if (state == GameStates.GraphicAction)
            StartCoroutine(GraphicActionCoroutine());
        else if (state == GameStates.LogicAction)
            GraphicActions.Clear();
    }

    private IEnumerator GraphicActionCoroutine()
    {
        foreach (var baseAction in GraphicActions)
        {
            yield return new WaitForSeconds(baseAction.Delay);
            baseAction.Execute();
            yield return new WaitForSeconds(baseAction.Duration);
        }

        GraphicActions.Clear();
        yield return new WaitForSeconds(.3f);
        GameController.Instance.ChangeState(GameStates.WaitInput);
    }

    public void AddActionToQueue(BaseGraphicAction baseGraphicAction)
    {
        GraphicActions.Add(baseGraphicAction);
    }

    public void OnClear()
    {
        if (_gridGraphic)
        {
#if UNITY_EDITOR
            DestroyImmediate(_gridGraphic.gameObject);
#else
            Destroy(_gridGraphic.gameObject);
#endif
            _gridGraphic = null;
        }

        for (var i = 0; i < _cells.Count; i++)
        {
#if UNITY_EDITOR
            DestroyImmediate(_cells[i].gameObject);
            DestroyImmediate(_elements[i].gameObject);
#else
            Destroy(_cells[i].gameObject);
            Destroy(_blocks[i].gameObject);
#endif
        }

        StopAllCoroutines();
        _cells.Clear();
        _elements.Clear();
    }
}