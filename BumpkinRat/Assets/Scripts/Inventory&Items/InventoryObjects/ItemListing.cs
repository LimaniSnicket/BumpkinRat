using System;

[Serializable]
public class ItemListing
{
    public Item Item { get; set; }
    private int amount;

    public int ListedAmount => amount;
    public bool IsEmpty => ListedAmount <= 0;

    public event EventHandler ItemListingEmpty;

    public event EventHandler ItemListingChanged;

    public ItemListing(Item i, int amount)
    {
        Item = i;
        this.amount = amount;
    }

    public void Add(int adding)
    {
        amount += adding;
        Broadcast();
    }

    public void Remove(int removing)
    {
        amount -= removing;
        Broadcast();
    }

    void Broadcast()
    {
        if (ItemListingChanged != null)
        {
            ItemListingChanged(this, new EventArgs());
        }
    }

    public override string ToString()
    {
        return $"{Item.DisplayName}: {amount}";
    }
}

