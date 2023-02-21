using System;
using DG.Tweening;
using UnityEngine;

[System.Serializable]
public class BlockDestroyGraphicAction : BaseGraphicAction
{
    [field: Header("Additional Settings")]
    [field: SerializeField] [field: Range(0,1)] public float DefaultDuration { get; set; }
    [field: SerializeField] [field: Range(0,1)] public float Combo1Duration { get; set; }
    [field: SerializeField] [field: Range(0,1)] public float Combo2Duration { get; set; }
    [field: SerializeField] [field: Range(0,1)] public float Combo3Duration { get; set; }

    public override void InitAction(GraphicController graphicController)
    {
        base.InitAction(graphicController);
        EventManager.OnBlockGroupDestroy.AddListener(OnBlockGroupDestroyed);
    }
    
    public void OnBlockGroupDestroyed(BlockGroup destroyedGroup, Cell clickedCell)
    {
        Duration = destroyedGroup.ComboIndex switch
        {
            0 => DefaultDuration,
            1 => Combo1Duration,
            2 => Combo2Duration,
            3 => Combo3Duration,
            _ => DefaultDuration
        };

        m_Action = () => DestroyAction(destroyedGroup, clickedCell);
        _graphicController.AddActionToQueue(MemberwiseClone() as BaseGraphicAction);
    }

    private void DestroyAction(BlockGroup destroyedGroup, Cell clickedCell)
    {
        foreach (var destroyedBlock in destroyedGroup.list)
        {
            //Debug.Log($"Graphic {destroyedBlock.Position} destroyed");
            destroyedBlock.blockGraphic.OnBlockGroupDestroy(destroyedGroup, clickedCell);
            _graphicController._elements.Remove(destroyedBlock.blockGraphic);
        }

        //not happy with these lines
        DOVirtual.DelayedCall(Duration, () =>
        {
            if (destroyedGroup.ComboIndex > 0)
                ParticleManager.Instance.PlayStarPoof(clickedCell.GetPosition() + new Vector3(.5f, .5f, 0));
            AudioManager.Instance.PlaySFX(4);
        });
    }
}