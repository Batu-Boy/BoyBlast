[System.Serializable]
public class PlayerDataModel : DataModel
{
    public static PlayerDataModel Data;
    public int LevelIndex;

    public PlayerDataModel Load()
    {
        if (Data == null)
        {
            Data = this;
            object data = LoadData();

            if (data != null)
            {
                Data = (PlayerDataModel)data;
            }
        }

        return Data;
    }


    public void Save()
    {
        Save(Data);
    }
}