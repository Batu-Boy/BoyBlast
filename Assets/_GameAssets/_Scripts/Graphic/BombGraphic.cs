using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BombGraphic : ElementGraphic
{
    //TODO: set and play particle
    [SerializeField] private ParticleSystem fusePartic;
    
    
    //TODO: figure out smt for localscale. And editor mode grid initialization.
    public override void InitializeGraphic(Element element, Sprite[] icons, int iconIndex)
    {
        base.InitializeGraphic(element, icons, iconIndex);
        transform.localScale = Vector3.zero;
    }

    public void Appear()
    {
        DOTween.Sequence().SetId(this)
            .Append(transform.DOScale(1.15f, .1f))
            .Append(transform.DOScale(.85f, .075f))
            .Append(transform.DOScale(1, .1f).SetEase(Ease.OutBack));
    }

    public override void Explode()
    {
        base.Explode();
        DOTween.Sequence().SetId(this)
            .Append(transform.DOScale(2, .2f))
            .Append(transform.DOScale(0, .2f).SetEase(Ease.InBack));
    }

    private void OnDisable()
    {
        DOTween.Complete(this);
    }
}
