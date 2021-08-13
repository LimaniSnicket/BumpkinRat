using System;
using Items;

[Serializable]
public class ItemDrop {

    private readonly ItemListing itemDropData;
    public string ItemToDropName => ToDrop.itemName;
    public Item ToDrop => itemDropData.Item;
    public int AmountToDrop => itemDropData.ListedAmount;

    public ItemDrop(int itemId, int amount)
    {
        int cap = Math.Max(amount, 0);
        Item item = ItemDataManager.GetItemById(itemId);
        itemDropData = new ItemListing(item, cap);
    }

    public ItemDrop(Item item, int amount)
    {
        int cap = Math.Max(amount, 0);
        itemDropData = new ItemListing(item, cap);
    }
}
