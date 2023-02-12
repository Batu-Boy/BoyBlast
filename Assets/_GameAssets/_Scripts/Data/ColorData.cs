using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "ScriptableObjects/New Block Data", fileName = "BlockData")]
public class ColorData : ScriptableObject
{
    [FormerlySerializedAs("Type")] public BlockColor color;
    public Sprite[] Icons;
}

