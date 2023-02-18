using System.Collections.Generic;

[System.Serializable]
public class InstantiateGraphicAction : BaseGraphicAction
{
    public override void InitAction(GraphicController graphicController)
    {
        base.InitAction(graphicController);
        EventManager.OnElementsInstantiate.AddListener(OnElementsInstantiated);
    }

    private void OnElementsInstantiated(List<Element> instantiatedElements)
    {
        m_Action = () => InstantiateActions(instantiatedElements);
        _graphicController.AddActionToQueue(MemberwiseClone() as InstantiateGraphicAction);
    }

    private void InstantiateActions(List<Element> instantiatedElements)
    {
        //Debug.LogWarning("Count:" + instantiatedBlocks.Count);
        foreach (var element in instantiatedElements)
        {
            var elementGraphic = element.ElementGraphic;

            _graphicController._elements.Add(elementGraphic);
            elementGraphic.OnElementFall(Duration);
        }
    }
}
