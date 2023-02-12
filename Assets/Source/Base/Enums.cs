public enum GameStates
{
    Main,
    Game,
    End,
    WaitInput,
    LogicAction,
    GraphicAction,
    Loading
}

public enum VibrationTypes
{
    None,
    Light,
    Medium,
    Heavy,
    Succes,
    Fail,
    RigidImpact,
    Soft,
    Warning
}

public enum BlockColor
{
    Yellow,
    Blue,
    Green,
    Pink,
    Purple,
    Red
}

//THE ORDER MUST DOWN>LEFT>UP>RIGHT
public enum Direction
{
    Down = 0,
    Left = 1,
    Up = 2,
    Right = 3
}