using System.Collections.Generic;

[System.Serializable]
public class LevelList
{
    public List<LevelModel> list;

/*#if UNITY_EDITOR
    [EditorButton]
    public void GetLevels()
    {
        //TODO: get levels from path
        for (var i = 0; i < list.Count; i++)
        {
            list[i].index = i;
        }
    }
#endif*/
}
