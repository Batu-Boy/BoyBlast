using System.Collections.Generic;
using UnityEngine.Events;

public static class EventManager
{
    public static UnityEvent<List<IDamageable>> OnElementsDamaged = new();
    public static UnityEvent<Bomb,Cell> OnBombCreated = new();
    public static UnityEvent<BlockGroup,Cell> OnBlockGroupDestroy = new ();
    public static UnityEvent<List<Element>> OnElementsFall = new ();
    public static UnityEvent<List<Element>> OnElementsInstantiate = new ();
    public static UnityEvent<List<Element>> OnElementsIconChange = new ();
    public static UnityEvent<List<Element>,List<Bomb>> OnElementsExplode = new();
    public static UnityEvent OnShuffleGrid = new ();
    public static UnityEvent OnValidMove = new ();

    public static void ClearAllListeners()
    {
        OnElementsDamaged.RemoveAllListeners();
        OnBombCreated.RemoveAllListeners();
        OnBlockGroupDestroy.RemoveAllListeners();
        OnElementsFall.RemoveAllListeners();
        OnElementsInstantiate.RemoveAllListeners();
        OnElementsIconChange.RemoveAllListeners();
        OnElementsExplode.RemoveAllListeners();
        OnShuffleGrid.RemoveAllListeners();
        OnValidMove.RemoveAllListeners();
    }
}