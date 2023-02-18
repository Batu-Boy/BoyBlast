using System.Linq;

[System.Serializable]
public class ShuffleGraphicAction : BaseGraphicAction
{
    public override void InitAction(GraphicController graphicController)
    {
        base.InitAction(graphicController);
        EventManager.OnShuffleGrid.AddListener(OnShuffleGrid);
    }

    private void OnShuffleGrid()
    {
        m_Action = ShuffleActions;
        _graphicController.AddActionToQueue(MemberwiseClone() as ShuffleGraphicAction);
    }
    
    private void ShuffleActions()
    {
        foreach (var blockGraphic in _graphicController._elements.OfType<BlockGraphic>())
        {
            blockGraphic.GoShufflePosition(GridInitializer.LevelModel.M + 5,1f);
        }
        
        _graphicController._elements.Clear();
    }
}
