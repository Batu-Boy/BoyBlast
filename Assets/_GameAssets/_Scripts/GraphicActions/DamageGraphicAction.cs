using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[Serializable]
public class DamageGraphicAction : BaseGraphicAction
{
    public override void InitAction(GraphicController graphicController)
    {
        base.InitAction(graphicController);
        EventManager.OnElementsDamaged.AddListener(OnElementsDamaged);
    }
    
    public void OnElementsDamaged(List<Element> damagedElements)
    {
        m_Action = () => DamageAction(damagedElements);
        _graphicController.AddActionToQueue(MemberwiseClone() as BaseGraphicAction);
    }

    private void DamageAction(List<Element> damagedElements)
    {
        //TODO: implement DamageableElement Class 
        foreach (var damagedElement in damagedElements)
        {
            if (damagedElement is Crate crate)
            {
                crate.crateGraphic.OnCrateDamaged(crate.Health);
            }
        }

        AudioManager.Instance.PlaySFX(4);
    }
}
