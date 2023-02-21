using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IconChangeGraphicAction : BaseGraphicAction
{
    public override void InitAction(GraphicController graphicController)
    {
        base.InitAction(graphicController);
        EventManager.OnElementsIconChange.AddListener(OnElementsIconChange);
    }

    private void OnElementsIconChange(List<Element> iconChangedElements, List<int> indexes)
    {
        m_Action = () => IconChangeActions(iconChangedElements,indexes);
        _graphicController.AddActionToQueue(MemberwiseClone() as IconChangeGraphicAction);
    }
    
    private void IconChangeActions(List<Element> iconChangedElements, List<int> indexes)
    {
        for (var i = 0; i < iconChangedElements.Count; i++)
        {
            var iconChangedElement = iconChangedElements[i];
            iconChangedElement.ElementGraphic.ChangeIconAtIndex(indexes[i]);
        }
    }
}
