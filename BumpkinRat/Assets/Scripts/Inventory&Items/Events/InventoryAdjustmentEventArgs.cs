using System;

public class InventoryAdjustmentEventArgs: EventArgs
{
    public ItemListing Listing { get; set; }
    public string NewAmountToDisplay => Listing.ListedAmount.ToString() ?? "0";
    public InventoryAdjustment AdjustmentType { get; set; }

    public InventoryAdjustmentEventArgs(ItemListing listing, InventoryAdjustment adjustment)
    {
        Listing = listing;
        AdjustmentType = adjustment;
    }
}