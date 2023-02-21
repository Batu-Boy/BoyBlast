using System.Collections;
using DG.Tweening;
using UnityEngine;

public class BlockGraphic : ElementGraphic
{
    [SerializeField] protected BlockParticleHandler _particleHandler;

    #region GraphicActions
    
    public void GoShufflePosition(float offsetX,float duration = 1)
    {
        transform.DOMove(transform.position + Vector3.right * offsetX, duration).SetEase(Ease.InSine)
            .OnComplete(()=> Destroy(gameObject));
    }
    
    public void OnBlockGroupDestroy(BlockGroup blockGroup, Cell clickedCell)
    {
        switch (blockGroup.ComboIndex)
        {
            case 0: 
                StartCoroutine(DestroyDefaultCoroutine());
                break;
            case 1:
                StartCoroutine(DestroyCombo1Coroutine(blockGroup.ComboIndex, clickedCell.GetPosition()));
                break;
            case 2:
                StartCoroutine(DestroyCombo1Coroutine(blockGroup.ComboIndex, clickedCell.GetPosition()));
                break;
            case 3:
                StartCoroutine(DestroyCombo1Coroutine(blockGroup.ComboIndex, clickedCell.GetPosition()));
                break;
        }
    }

    public override void Explode()
    {
        base.Explode();
        //TODO: config
        StartCoroutine(DestroyDefaultCoroutine());
    }

    private IEnumerator DestroyDefaultCoroutine()
    {
        _particleHandler.PlayDestroy();
        CloseAllIcons();
        yield return new WaitForSeconds(.25f);
        ResetIconValues();
        BlockPool.Instance.ReturnItem(_element as Block);
    }
    
    private IEnumerator DestroyCombo1Coroutine(int combo, Vector3 clickedPos)
    {
        _particleHandler.PlayStarTrail();
        BringForward();
        var position = transform.position;
        Vector3 targetDirection = (clickedPos - position).normalized;
        float distanceFactor = .25f;
        DOTween.Sequence()
            .Append(transform.DOMove(position - (targetDirection * distanceFactor), .08f).SetEase(Ease.OutSine))
            .Append(transform.DOMove(clickedPos, .16f).SetEase(Ease.InSine));
        yield return new WaitForSeconds(.4f);
        ResetIconValues();
        yield return new WaitForSeconds(.25f);
        BlockPool.Instance.ReturnItem(_element as Block);
    }
    
    #endregion

}