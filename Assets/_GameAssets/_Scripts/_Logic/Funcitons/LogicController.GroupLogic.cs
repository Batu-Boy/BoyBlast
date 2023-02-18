public partial class LogicController
{
    #region GroupLogic

    private void DetectGroups()
    {
        ClearAllGroups();

        for (var x = 0; x < _grid.Width; x++)
        for (var y = 0; y < _grid.Height; y++)
        {
            if (!_grid.TryGetCell(x, y, out var currentCell)) continue;

            var neighbors = currentCell.GetNeighbors();
            if (!currentCell.TryGetElementAs<Block>(out var currentBlock)) continue;

            foreach (var neighbor in neighbors)
            {
                if (neighbor == null) continue;

                if (!neighbor.TryGetElementAs<Block>(out var neighborBlock)) continue;

                if (neighborBlock.Color == currentBlock.Color) 
                    GroupAddHandler(neighborBlock, currentBlock);
            }
        }
    }

    private void GroupAddHandler(Block a, Block b)
    {
        if (a.Color != b.Color) return;

        var aHasGroup = a.TryGetBlockGroup(out var aGroup);
        var bHasGroup = b.TryGetBlockGroup(out var bGroup);

        // neither has group
        if (!aHasGroup && !bHasGroup)
        {
            var newGroup = new BlockGroup { Color = a.Color };
            newGroup.AddBlock(a);
            newGroup.AddBlock(b);
            _blockGroups.Add(newGroup);
            return;
        }

        // both have groups
        if (aHasGroup && bHasGroup)
        {
            // both in same group
            if (aGroup == bGroup) return;

            aGroup.MergeWith(bGroup);
            _blockGroups.Remove(bGroup);
            return;
        }

        // one of them has group
        if (aHasGroup)
        {
            aGroup.AddBlock(b);
            return;
        }

        bGroup.AddBlock(a);
    }

    private void ClearAllGroups()
    {
        for (var x = 0; x < _grid.Width; x++)
        for (var y = 0; y < _grid.Height; y++)
        {
            //TODO: configure
            if (!_grid.TryGetElementAs<Block>(x, y, out var block)) continue;

            block.SetGroup(null);
        }

        _blockGroups.Clear();
    }

    #endregion
}