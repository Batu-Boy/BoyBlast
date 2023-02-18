using System.Collections.Generic;
using UnityEngine;

public partial class LogicController
{
    #region BombExplodeLogic

    private void DestroyBombRangeElements(Bomb bomb, Cell cell)
    {
        var bombs = new List<Bomb> { bomb };

        RecurseBombs(bombs);
    }

    private void RecurseBombs(List<Bomb> bombs)
    {
        var destroyedElements = new List<Element>();
        var innerBombList = new List<Bomb>();

        foreach (var bomb in bombs)
        {
            BombCalculation(bomb, ref destroyedElements, ref innerBombList);
            bomb.Destroy(null);
        }

        if (bombs.Count > 0)
            EventManager.OnElementsExplode?.Invoke(destroyedElements, bombs);

        if (innerBombList.Count > 0) 
            RecurseBombs(innerBombList);
    }

    private void BombCalculation(Bomb bomb, ref List<Element> destroyedElements, ref List<Bomb> newBombs)
    {
        var xRange = bomb.Range.x;
        var yRange = bomb.Range.y;

        var xStartOffset = Mathf.CeilToInt(xRange / 2f);
        var xFinishOffset = xRange - xStartOffset;

        var yStartOffset = Mathf.CeilToInt(yRange / 2f);
        var yFinishOffset = yRange - yStartOffset;

        var position = bomb.Position;

        for (var x = position.x - xStartOffset + 1; x < position.x + xFinishOffset + 1; x++)
        for (var y = position.y - yStartOffset + 1; y < position.y + yFinishOffset + 1; y++)
        {
            if (!_grid.TryGetCell(x, y, out var currentCell)) continue;

            var currentElement = currentCell.GetElement();
            if (!currentElement) continue;
            if (currentElement.Position == bomb.Position) continue;

            if (currentElement is Bomb currentBomb)
            {
                if (!newBombs.Contains(currentBomb)) newBombs.Add(currentBomb);
            }
            else
            {
                currentElement.Destroy(bomb.GetCell());
                destroyedElements.Add(currentElement);
            }
        }
    }

    #endregion
}