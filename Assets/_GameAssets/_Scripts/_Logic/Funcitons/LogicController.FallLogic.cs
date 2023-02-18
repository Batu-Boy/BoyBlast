using System.Collections.Generic;

public partial class LogicController
{
    #region FallLogic

    private void FallExistingElements()
    {
        var fallenBlocks = new List<Element>();
        //TODO: MAYBE CHECK COLUMNS FIRST
        for (var x = 0; x < _grid.Width; x++)
        for (var y = 0; y < _grid.Height - 1; y++)
        {
            if (!_grid.TryGetCell(x, y, out var currentCell)) continue;

            if (!currentCell.IsEmpty) continue;

            Cell upMostCell = null;
            for (var i = y; i < _grid.Height; i++)
            {
                if (!_grid.TryGetCell(x, i, out var upperCell)) continue;

                if (upperCell.IsEmpty) continue;

                upMostCell = upperCell;
                break;
            }

            if (upMostCell == null) continue;

            var upMostElement = upMostCell.GetElement();
            if (!upMostElement.canFall) continue;

            currentCell.SetElement(upMostElement);
            upMostElement.SetCell(currentCell);
            upMostCell.ClearCell();
            //TODO: configure as block
            fallenBlocks.Add(upMostElement);
        }

        if (fallenBlocks.Count > 0) EventManager.OnElementsFall?.Invoke(fallenBlocks);
    }

    #endregion
}