using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class ScreenModel : MonoBase
{
    [SerializeField] protected ScreenElement[] screenElements;
    [SerializeField] protected UnityEvent onShowEvent;
    [SerializeField] protected UnityEvent onHideEvent;
    [SerializeField] protected CanvasGroup canvasGroup;

    public override void Initialize()
    {
        base.Initialize();
        foreach (var item in screenElements)
        {
            item.Initialize();
            
        }
    }

    public virtual void Show()
    {
        DOTween.Complete(this, true);
        DOVirtual.DelayedCall(.21f, () => gameObject.SetActive(true)).SetId(this);
        canvasGroup.DOFade(1, .2f).SetDelay(.21f).SetId(this)
            .OnComplete(() =>onShowEvent?.Invoke() );
    }

    public virtual void Hide()
    {
        canvasGroup.DOFade(0, .2f).SetId(this)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                onHideEvent?.Invoke();
            });
    }
}