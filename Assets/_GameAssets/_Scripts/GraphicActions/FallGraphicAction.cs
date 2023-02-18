using System.Collections.Generic;

[System.Serializable]
public class FallGraphicAction : BaseGraphicAction
{
    public override void InitAction(GraphicController graphicController)
    {
        base.InitAction(graphicController);
        EventManager.OnElementsFall.AddListener(OnElementsFall);
    }

    private void OnElementsFall(List<Element> fallingElements)
    {
        m_Action = () => FallActions(fallingElements);
        _graphicController.AddActionToQueue(MemberwiseClone() as FallGraphicAction);
    }

    private void FallActions(List<Element> fallingElements)
    {
        foreach (Element element in fallingElements)
        {
            //print(_blockLink[block]);
            element.ElementGraphic.OnElementFall(Duration);
        }
        //DOVirtual.DelayedCall(.2f, () => AudioManager.Instance.PlaySFX(5));
    }
}
