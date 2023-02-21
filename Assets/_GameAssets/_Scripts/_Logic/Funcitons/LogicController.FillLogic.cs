using System.Collections.Generic;
using UnityEngine;

public partial class LogicController
{
    #region FillLogic

    private void FillEmptyCells()
    {
        var instantiatedBlocks = new List<Element>();
        for (var x = 0; x < _grid.Width; x++)
        for (var y = _grid.Height - 1; y >= 0; y--)
        {
            if (!_grid.TryGetCell(x, y, out var currentCell)) continue;

            if (!currentCell.IsEmpty)
            {
                if (!currentCell.GetElement().CanFall) break;

                continue;
            }

            var newBlock = BlockPool.Instance.GetItem(); //Instantiate(blockPrefab);
            newBlock.Initialize(GridInitializer.GetRandomColor(), currentCell,
                currentCell.GetPosition() + GridInitializer.LevelModel.N * Vector3Int.up);
            currentCell.SetElement(newBlock);
            instantiatedBlocks.Add(newBlock);
        }

        if (instantiatedBlocks.Count > 0) EventManager.OnElementsInstantiate?.Invoke(instantiatedBlocks);
    }

    #endregion
}