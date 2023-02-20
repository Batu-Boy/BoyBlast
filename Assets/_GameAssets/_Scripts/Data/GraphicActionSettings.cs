using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class GraphicActionSettings : ScriptableObject
{
    [FormerlySerializedAs("_blockDestroyAction")]
    [Header("SETTINGS")] 
    [SerializeField] private BlockDestroyGraphicAction blockDestroyGraphicAction;
    [FormerlySerializedAs("_bombCreateAction")] [SerializeField] private BombCreateGraphicAction bombCreateGraphicAction;
    [FormerlySerializedAs("_bombExplodeAction")] [SerializeField] private BombExplodeGraphicAction bombExplodeGraphicAction;
    [FormerlySerializedAs("_fallAction")] [SerializeField] private FallGraphicAction fallGraphicAction;
    [FormerlySerializedAs("_iconChangeAction")] [SerializeField] private IconChangeGraphicAction iconChangeGraphicAction;
    [FormerlySerializedAs("_instantiateAction")] [SerializeField] private InstantiateGraphicAction instantiateGraphicAction;
    [FormerlySerializedAs("_shuffleAction")] [SerializeField] private ShuffleGraphicAction shuffleGraphicAction;
    [SerializeField] private DamageGraphicAction _damageGraphicAction;
    

    private void Awake()
    {
        Debug.Log("Awake");
    }

    private void OnEnable()
    {
        Debug.Log("Enabled");
    }

    private void OnDisable()
    {
        Debug.Log("Disabled");
    }

    private void OnDestroy()
    {
        Debug.Log("Destroyed");
    }

    public void Initialize(GraphicController graphicController)
    {
        blockDestroyGraphicAction.InitAction(graphicController);
        bombCreateGraphicAction.InitAction(graphicController);
        bombExplodeGraphicAction.InitAction(graphicController);
        fallGraphicAction.InitAction(graphicController);
        iconChangeGraphicAction.InitAction(graphicController);
        instantiateGraphicAction.InitAction(graphicController);
        shuffleGraphicAction.InitAction(graphicController);
        _damageGraphicAction.InitAction(graphicController);
    }
}