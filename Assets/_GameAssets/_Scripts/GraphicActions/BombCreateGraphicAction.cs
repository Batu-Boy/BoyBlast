[System.Serializable]
public class BombCreateGraphicAction : BaseGraphicAction
{
    public override void InitAction(GraphicController graphicController)
    {
        base.InitAction(graphicController);
        EventManager.OnBombCreated.AddListener(OnBombCreated);
    }

    private void OnBombCreated(Bomb bomb, Cell cell)
    {
        m_Action = () => BombCreateActions(bomb,cell);
        _graphicController.AddActionToQueue(MemberwiseClone() as BombCreateGraphicAction);
    }

    private void BombCreateActions(Bomb bomb, Cell cell)
    {
        bomb.bombGraphic.Appear();
    }
}
