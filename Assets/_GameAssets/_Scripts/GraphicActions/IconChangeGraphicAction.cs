using System.Collections.Generic;

[System.Serializable]
public class IconChangeGraphicAction : BaseGraphicAction
{
    public override void InitAction(GraphicController graphicController)
    {
        base.InitAction(graphicController);
        EventManager.OnElementsIconChange.AddListener(OnElementsIconChange);
    }

    private void OnElementsIconChange(List<Element> iconChangedElements)
    {
        m_Action = () => IconChangeActions(iconChangedElements);
        _graphicController.AddActionToQueue(MemberwiseClone() as IconChangeGraphicAction);
    }

    private void IconChangeActions(List<Element> iconChangedElements)
    {
        foreach (Element iconChangedElement in iconChangedElements)
        {
            iconChangedElement.ElementGraphic.ChangeIconAtIndex(iconChangedElement.GetIconIndex());
        }
    }
}
