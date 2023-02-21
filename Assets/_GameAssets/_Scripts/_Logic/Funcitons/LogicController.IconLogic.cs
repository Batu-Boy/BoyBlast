using System.Collections.Generic;

public partial class LogicController
{
    #region IconChangeLogic

    private void SetGroupIcons()
    {
        var iconChangedBlocks = new List<Element>();
        var indexes = new List<int>();
        foreach (var blockGroup in _blockGroups)
        {
            foreach (var block in blockGroup.list)
            {
                block.SetIconIndex(blockGroup.ComboIndex);
                iconChangedBlocks.Add(block);
                indexes.Add(blockGroup.ComboIndex);
            }
        }

        if (iconChangedBlocks.Count > 0)
            EventManager.OnElementsIconChange?.Invoke(iconChangedBlocks, indexes);
    }

    private void SetSingleIcons()
    {
        var singleBlocks = new List<Element>();
        var indexes = new List<int>();
        for (var x = 0; x < _grid.Width; x++)
        for (var y = 0; y < _grid.Height; y++)
        {
            //TODO: configure
            if (!_grid.TryGetElementAs<Block>(x, y, out var currentBlock)) continue;
            if (currentBlock.HasGroup) continue;

            currentBlock.SetDefaultIcon();
            singleBlocks.Add(currentBlock);
            indexes.Add(0);
        }

        if (singleBlocks.Count > 0)
            EventManager.OnElementsIconChange?.Invoke(singleBlocks, indexes);
    }

    #endregion
}