using System.Collections.Generic;

public class ItemIdToAmountMap
{
    public Dictionary<int, int> ItemMap { get; private set; }
    public ItemIdToAmountMap()
    {
        ItemMap = new Dictionary<int, int>();
    }

    public void IncrementItemAmount(int itemId)
    {
        ItemMap.Increment(itemId);
    }

    public int GetDecrementedItemAmount(int itemId)
    {
        if (!ItemMap.ContainsKey(itemId))
        {
            throw new KeyNotFoundException("Requested Item Id does not exist in this Item to Amount Map.");
        }

        return ItemMap.Decrement(itemId);
    }
}
