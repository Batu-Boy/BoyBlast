using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/New Graphic Data",fileName = "GraphicData")]
public class GraphicData : ScriptableObject
{
    [Header("Sprites")]
    public Sprite defaultCellSprite;
    public Sprite defaultGridSprite;

    [Header("Prefabs")]
    public GridGraphic gridPrefab;
    public CellGraphic cellPrefab;
    public BlockGraphic blockPrefab;
}
