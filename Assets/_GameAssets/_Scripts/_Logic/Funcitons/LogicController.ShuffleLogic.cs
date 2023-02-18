using System.Collections.Generic;
using UnityEngine;

public partial class LogicController
{
    #region ShuffleLogic

    private void ShuffleGrid()
    {
        Debug.LogWarning("No Valid Move. Shuffle!");
        EventManager.OnShuffleGrid?.Invoke();
        while (true)
        {
            var instantiatedElements = new List<Element>();
            for (var x = 0; x < _grid.Width; x++)
            for (var y = 0; y < _grid.Height; y++)
            {
                if (!_grid.TryGetCell(x, y, out var currentCell)) continue;

                if (!currentCell.TryGetElementAs<Block>(out var block)) continue; // shuffle only blocks

                var newBlock = BlockPool.Instance.GetItem(); //Instantiate(blockPrefab);
                newBlock.Initialize(GridInitializer.GetRandomColor(), currentCell,
                    currentCell.GetPosition() + GridInitializer.LevelModel.N * Vector3Int.up);
                currentCell.SetElement(newBlock);
                instantiatedElements.Add(newBlock);
            }

            if (!HasAnyValidMove()) continue;
            EventManager.OnElementsInstantiate?.Invoke(instantiatedElements);
            SetGroupIcons();
            return;
        }
    }

    private bool HasAnyValidMove()
    {
        DetectGroups();
        return _blockGroups.Count > 0;
    }

    #endregion
}