using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Color List",fileName = "ColorList")]
public class BlockColors : ScriptableObject
{
    public List<ColorData> list;
}
