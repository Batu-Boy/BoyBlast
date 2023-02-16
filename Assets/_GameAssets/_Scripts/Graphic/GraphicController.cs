using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class GraphicController : ControllerBase
{
    [Header("Settings")]
    [SerializeField] private GraphicActionSettings ActionSettings;
    
    [Header("References")]
    [SerializeField] private BlockColors blockColors;
    [SerializeField] private GraphicData _graphicData;
    [SerializeField] private BlockPool _blockPool;

    [Header("Parents")] 
    [SerializeField] private Transform cellParent;
    [SerializeField] private Transform blockParent;
    
    private GridGraphic _gridGraphic;
    private List<CellGraphic> _cells = new ();
    private List<ElementGraphic> _elements = new ();
    
    private List<Func<float>> _graphicActions = new ();
 
    public override void Initialize()
    {
        base.Initialize();
        EventManager.OnBlockGroupDestroy.AddListener(OnBlockGroupDestroyed);
        EventManager.OnElementsFall.AddListener(OnBlocksFall);
        EventManager.OnElementsInstantiate.AddListener(OnBlocksInstantiated);
        EventManager.OnElementsIconChange.AddListener(OnBlocksIconChange);
        EventManager.OnShuffleGrid.AddListener(OnShuffleGrid);
        EventManager.OnBombCreated.AddListener(OnBombCreated);
        EventManager.OnElementsExplode.AddListener(OnElementsExplode);
    }

    private void OnDestroy()
    {
        EventManager.OnBlockGroupDestroy.RemoveAllListeners();
        EventManager.OnElementsFall.RemoveAllListeners();
        EventManager.OnElementsInstantiate.RemoveAllListeners();
        EventManager.OnElementsIconChange.RemoveAllListeners();
        EventManager.OnShuffleGrid.RemoveAllListeners();
        EventManager.OnBombCreated.RemoveAllListeners();
        EventManager.OnElementsExplode.RemoveAllListeners();
    }
    
    public void OnGridInitialized(Grid grid)
    {
        _gridGraphic = null;
        _cells.Clear();
        _elements.Clear();
        
        _gridGraphic = Instantiate(_graphicData.gridPrefab);
        _gridGraphic.InitializeGraphic(new Vector3Int(grid.Width, grid.Height, 1),
            _graphicData.defaultGridSprite);
        
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                var cell = Instantiate(_graphicData.cellPrefab,cellParent);
                cell.InitializeGraphic(grid.GetCell(x, y), _graphicData.defaultCellSprite);
                _cells.Add(cell);
                
                //TODO: config
                if (grid.TryGetElementAs<Block>(x, y, out var element))
                {
                    var blockGraphic = element.blockGraphic;
                    Sprite[] icons = blockColors.list[(int)element.Color].Icons;
                    blockGraphic.InitializeGraphic(element, icons,element.GetIconIndex());
                    _elements.Add(blockGraphic);
                }
            }
        }
    }

    public override void OnStateChanged(GameStates state)
    {
        if (state == GameStates.GraphicAction)
        {
            StartCoroutine(GraphicActionCoroutine());
        }
        else if (state == GameStates.LogicAction)
        {
            _graphicActions.Clear();
        }
    }

    private IEnumerator GraphicActionCoroutine()
    {
        foreach (var graphicAction in _graphicActions)
        {
            yield return new WaitForSeconds(graphicAction.Invoke());
        }

        yield return new WaitForSeconds(.3f);
        GameController.Instance.ChangeState(GameStates.WaitInput);
    }

    #region GraphicActions

    private float ShuffleAction()
    {
        foreach (var blockGraphic in _elements.OfType<BlockGraphic>())
        {
            blockGraphic.GoShufflePosition(GridInitializer.LevelModel.M + 5,1f);
        }
        
        _elements.Clear();

        return ActionSettings.shuffleDuration;
    }
    
    private float DestroyAction(BlockGroup destroyedGroup, Cell clickedCell)
    {
        foreach (Block destroyedBlock in destroyedGroup.list)
        {
            //Debug.Log($"Graphic {destroyedBlock.Position} destroyed");
            destroyedBlock.blockGraphic.OnBlockGroupDestroy(destroyedGroup, clickedCell);
            _elements.Remove(destroyedBlock.blockGraphic);
        }
        
        var duration = destroyedGroup.ComboIndex switch
        {
            0 => ActionSettings.destroyDefaultDuration,
            1 => ActionSettings.combo1Duration,
            2 => ActionSettings.combo2Duration,
            3 => ActionSettings.combo3Duration,
            _ => ActionSettings.destroyDefaultDuration
        };
        //not happy with these lines
        DOVirtual.DelayedCall(duration, () =>
        {
            if (destroyedGroup.ComboIndex > 0)
                ParticleManager.Instance.PlayStarPoof(clickedCell.GetPosition() + new Vector3(.5f, .5f, 0));
            AudioManager.Instance.PlaySFX(4);
        });
        return duration;
    }

    private float FallAction(List<Element> movedElements)
    {
        foreach (Element element in movedElements)
        {
            //print(_blockLink[block]);
            element.ElementGraphic.OnElementFall(ActionSettings.blockFallDuration);
        }

        //DOVirtual.DelayedCall(.2f, () => AudioManager.Instance.PlaySFX(5));
        
        return ActionSettings.fallDuration;
    }

    private float InstantiateAction(List<Element> instantiatedElements)
    {
        //Debug.LogWarning("Count:" + instantiatedBlocks.Count);
        foreach (Element element in instantiatedElements)
        {
            var elementGraphic = element.ElementGraphic;
            
            _elements.Add(elementGraphic);
            elementGraphic.OnElementFall(ActionSettings.instantiatedBlockFallDuration);
        }

        return ActionSettings.instantiateDuration;
    }

    private float IconChangeAction(List<Element> iconChangedElements)
    {
        foreach (Element iconChangedElement in iconChangedElements)
        {
            iconChangedElement.ElementGraphic.ChangeIconAtIndex(iconChangedElement.GetIconIndex());
        }

        return ActionSettings.iconChangeDuration;
    }

    private float BombCreateAction(Bomb bomb, Cell cell)
    {
        bomb.bombGraphic.Appear();
            
        return ActionSettings.bombCreateDuration;
    }
    
    private float BombExplodeAction(List<Element> elements, List<Bomb> bombs)
    {
        foreach (var element in elements)
        {
            element.ElementGraphic.Explode();
        }
        
        foreach (var bomb in bombs)
        {
            bomb.bombGraphic.Explode();
        }

        return ActionSettings.bombExplodeDuration;
    }

    #endregion

    //Connection to Logic
    #region Event Subsribers

    private void OnShuffleGrid()
    {
        _graphicActions.Add(ShuffleAction);
    }
    
    private void OnBlockGroupDestroyed(BlockGroup destroyedGroup, Cell clickedCell)
    {
        _graphicActions.Add(() => DestroyAction(destroyedGroup, clickedCell));
    }

    private void OnBlocksFall(List<Element> movedBlocks)
    {
        _graphicActions.Add(() => FallAction(movedBlocks));

    }

    private void OnBlocksInstantiated(List<Element> instantiatedBlocks)
    {
        _graphicActions.Add(() => InstantiateAction(instantiatedBlocks));
    }

    private void OnBlocksIconChange(List<Element> iconChangedBlocks)
    {
        _graphicActions.Add(() => IconChangeAction(iconChangedBlocks));
    }

    private void OnBombCreated(Bomb bomb, Cell cell)
    {
        _graphicActions.Add(() => BombCreateAction(bomb, cell));
    }
    
    private void OnElementsExplode(List<Element> elements, List<Bomb> bombs)
    {
        _graphicActions.Add(()=> BombExplodeAction(elements,bombs));
    }

    #endregion

    
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
        
        for (int i = 0; i < _cells.Count; i++)
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
        _graphicActions.Clear();
        _cells.Clear();
        _elements.Clear();
    }
}
