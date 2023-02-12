using System.Collections.Generic;
using UnityEngine.Events;

public static class EventManager
{
    public static UnityEvent<BlockGroup,Cell> OnBlockGroupDestroy = new ();
    public static UnityEvent<List<Block>> OnBlocksFall = new ();
    public static UnityEvent<List<Block>> OnBlocksInstantiate = new ();
    public static UnityEvent<List<Block>> OnBlocksIconChange = new ();
    public static UnityEvent OnShuffleGrid = new ();
    public static UnityEvent OnValidMove = new ();
}
