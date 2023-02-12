using System;
using UnityEngine;

[System.Serializable]
public class PoolItemModel : ItemModel<PoolItemDataModel>
{
    public int multiplePoolIndex;
    public int poolIndex;

    private void OnValidate()
    {
        id = transform.GetSiblingIndex();
        var parent = transform.parent;
        multiplePoolIndex = parent.parent.GetSiblingIndex();
        poolIndex = parent.GetSiblingIndex();
    }

    public override void SetValues(PoolItemDataModel data)
    {
        id = data.Id;
        multiplePoolIndex = data.multiplePoolIndex;
        poolIndex = data.poolIndex;
        transform.position = data.Position;
        transform.rotation = data.Rotation;
        transform.localScale = data.Scale;
    }
    public override PoolItemDataModel GetData()
    {
        PoolItemDataModel dataModel = new PoolItemDataModel();
        dataModel.Id = id;
        dataModel.poolIndex = poolIndex;
        dataModel.multiplePoolIndex = multiplePoolIndex;
        dataModel.Position = transform.position;
        dataModel.Rotation = transform.rotation;
        dataModel.Scale = transform.localScale;
        return dataModel;
    }
}

[System.Serializable]
public class PoolItemDataModel : ItemDataModel
{
    public int multiplePoolIndex;
    public int poolIndex;
    public Vector3 Position;
    public Vector3 Scale;
    public Quaternion Rotation;
}