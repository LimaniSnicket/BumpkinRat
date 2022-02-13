using System.Linq;
using System.Collections.Generic;

public class ItemDropFactory
{
    private const char DropSplitter = 'x';
    public List<ItemDrop> GetItemsToDrop(params (int, int)[] dropData)
    {
        return dropData.Select(drop => new ItemDrop(drop.Item1, drop.Item2)).ToList();
    }

    public List<ItemDrop> GetItemsToDrop(int[] drops)
    {
        return drops.Select(d => new ItemDrop(d, 1)).ToList();
    }

    public ItemDrop CreateFromString(string dropData)
    {
        if (dropData.Contains(DropSplitter))
        {
            var split = dropData.Split(DropSplitter);

            return new ItemDrop(Parsed(split[0], 0), Parsed(split[1]));

        }

        return new ItemDrop(Parsed(dropData, 0), 1);
    }

    private int Parsed(string s, int def = 1)
    {
        bool valid = int.TryParse(s, out int result);

        return valid ? result : def;
    }
}
