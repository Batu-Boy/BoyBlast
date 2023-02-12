using UnityEngine;

public static class ConstantValues
{
    public const int MAXROWS = 10;
    public const int MINROWS = 2;
    public const int MAXCOLUMNS = 10;
    public const int MINCOLUMNS = 2;
    public const int MAXCOLORS = 6;
    public const int MINCOLORS = 1;
}

//THE ORDER MUST DOWN>LEFT>UP>RIGHT
public static class Directions
{
    public static readonly Vector3 DOWN = new (0, -1);
    public static readonly Vector3 LEFT = new (-1, 0);
    public static readonly Vector3 UP = new (0, 1);
    public static readonly Vector3 RIGHT = new (1, 0);
}