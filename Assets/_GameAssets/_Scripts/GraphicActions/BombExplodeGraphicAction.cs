using System.Collections.Generic;

[System.Serializable]
public class BombExplodeGraphicAction : BaseGraphicAction
{
    public override void InitAction(GraphicController graphicController)
    {
        base.InitAction(graphicController);
        EventManager.OnElementsExplode.AddListener(OnElementsExplode);
    }

    private void OnElementsExplode(List<Element> explodedElements, List<Bomb> bombs)
    {
        m_Action = () => ExplodeAction(explodedElements, bombs);
        _graphicController.AddActionToQueue(MemberwiseClone() as BombExplodeGraphicAction);
    }

    private void ExplodeAction(List<Element> explodedElements, List<Bomb> bombs)
    {
        foreach (var element in explodedElements)
        {
            element.ElementGraphic.Explode();
        }
        
        foreach (var bomb in bombs)
        {
            bomb.bombGraphic.Explode();
        }
    }
}
