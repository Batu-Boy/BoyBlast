using System;
using DG.Tweening;

[Serializable]
public class DefaultDestroyAction : BaseAction
{
    public override void Execute(BlockGraphic blockGraphic)
    {
        /*//TODO: ParticleHandler Default Destroy Particle
        blockGraphic.CloseAllIcons();
        DOVirtual.DelayedCall(.25f, () =>
        {
            blockGraphic.ResetIconValues();
            BlockPool.Instance.ReturnItem(blockGraphic);
        });*/
    }
}

