using System;
using System.Collections;
using System.Collections.Generic;
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
    private List<BlockGraphic> _blocks = new ();

    private List<Func<float>> _graphicActions = new ();
 
    public override void Initialize()
    {
        base.Initialize();
        EventManager.OnBlockGroupDestroy.AddListener(OnBlockGroupDestroyed);
        EventManager.OnBlocksFall.AddListener(OnBlocksFall);
        EventManager.OnBlocksInstantiate.AddListener(OnBlocksInstantiated);
        EventManager.OnBlocksIconChange.AddListener(OnBlocksIconChange);
        EventManager.OnShuffleGrid.AddListener(OnShuffleGrid);
    }

    private void OnDestroy()
    {
        EventManager.OnBlockGroupDestroy.RemoveAllListeners();
        EventManager.OnBlocksFall.RemoveAllListeners();
        EventManager.OnBlocksInstantiate.RemoveAllListeners();
        EventManager.OnBlocksIconChange.RemoveAllListeners();
        EventManager.OnShuffleGrid.RemoveAllListeners();
    }
    
    public void OnGridInitialized(Grid grid)
    {
        _gridGraphic = null;
        _cells.Clear();
        _blocks.Clear();
        
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
                    var blockGraphic = element._blockGraphic;
                    Sprite[] icons = blockColors.list[(int)element.Color].Icons;
                    blockGraphic.InitializeGraphic(element, icons,element.GetIconIndex());
                    _blocks.Add(blockGraphic);
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
        foreach (BlockGraphic blockGraphic in _blocks)
        {
            blockGraphic.GoShufflePosition(GridInitializer.LevelModel.M + 5,1f);
        }
        
        _blocks.Clear();

        return ActionSettings.shuffleDuration;
    }
    
    private float DestroyAction(BlockGroup destroyedGroup, Cell clickedCell)
    {
        foreach (Block destroyedBlock in destroyedGroup.list)
        {
            //Debug.Log($"Graphic {destroyedBlock.Position} destroyed");
            destroyedBlock._blockGraphic.OnBlockGroupDestroy(destroyedGroup, clickedCell);
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

    private float FallAction(List<Block> movedBlocks)
    {
        foreach (Block block in movedBlocks)
        {
            //print(_blockLink[block]);
            block._blockGraphic.OnBlockFall(ActionSettings.blockFallDuration);
        }

        //DOVirtual.DelayedCall(.2f, () => AudioManager.Instance.PlaySFX(5));
        
        return ActionSettings.fallDuration;
    }

    private float InstantiateAction(List<Block> instantiatedBlocks)
    {
        //Debug.LogWarning("Count:" + instantiatedBlocks.Count);
        foreach (Block instantiatedBlock in instantiatedBlocks)
        {
            var blockGraphic = instantiatedBlock._blockGraphic;//_blockPool.GetItem(blockParent);//Instantiate(_graphicData.blockPrefab, blockParent);
            //blockGraphic.InitializeGraphic(instantiatedBlock, blockColors.list[(int)instantiatedBlock.Color].Icons,
              //  instantiatedBlock.GetIconIndex(),
                //instantiatedBlock.Position + Vector3Int.up * GridInitializer.LevelModel.N); // create position is (actual pos + grid height)

            //Debug.LogWarning($"BlockGraphic Created for Block At:{instantiatedBlock.Position}");
            _blocks.Add(blockGraphic);
            blockGraphic.GoBlockPosition(ActionSettings.instantiatedBlockFallDuration);
        }

        return ActionSettings.instantiateDuration;
    }

    private float IconChangeAction(List<Block> iconChangedBlocks)
    {
        foreach (Block iconChangedBlock in iconChangedBlocks)
        {
            iconChangedBlock._blockGraphic.ChangeIconAtIndex(iconChangedBlock.GetIconIndex());
        }

        return ActionSettings.iconChangeDuration;
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

    private void OnBlocksFall(List<Block> movedBlocks)
    {
        _graphicActions.Add(() => FallAction(movedBlocks));

    }

    private void OnBlocksInstantiated(List<Block> instantiatedBlocks)
    {
        _graphicActions.Add(() => InstantiateAction(instantiatedBlocks));
    }

    private void OnBlocksIconChange(List<Block> iconChangedBlocks)
    {
        _graphicActions.Add(() => IconChangeAction(iconChangedBlocks));
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
            DestroyImmediate(_blocks[i].gameObject);
#else
            Destroy(_cells[i].gameObject);
            Destroy(_blocks[i].gameObject);
#endif
        }
        StopAllCoroutines();
        _graphicActions.Clear();
        _cells.Clear();
        _blocks.Clear();
    }
}
