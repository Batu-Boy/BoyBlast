using UnityEngine;

public class EndScreen : ScreenModel
{
    [SerializeField] private GameController _gameController;
    
    public override void Show()
    {
        screenElements[0].SetActiveGameObject(_gameController.IsPlayerWin);
        screenElements[1].SetActiveGameObject(!_gameController.IsPlayerWin);
        base.Show();
    }
}
