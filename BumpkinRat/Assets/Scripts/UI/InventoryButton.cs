
using UnityEngine.UI;
using TMPro;
using System;

public class InventoryButton : Button
{
    public string ItemNameToDisplay => associatedItemListing == null ? string.Empty : associatedItemListing.item.DisplayName;
    public string ItemAmountToDisplay { get; private set; }

    ItemListing associatedItemListing;

    public TextMeshProUGUI textMesh => gameObject.GetOrAddComponentInChildren<TextMeshProUGUI>();

    public static event EventHandler<InventoryButtonArgs> InventoryButtonPressed, FinalPossiblePress;

    protected override void Start()
    {
        base.Start();

        transform.GetChild(0).gameObject.SetActive(true);

        onClick.AddListener(() => OnClickBroadcastPressed());
    }

    public void SetItemListing(ItemListing listing)
    {
        listing.ItemListingChanged += OnItemListingChange;
        associatedItemListing = listing;
        UpdateDisplay();
    }

    void OnItemListingChange(object source, EventArgs args)
    {
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        textMesh.text = associatedItemListing.ToString();
    }

    void OnClickBroadcastPressed()
    {
        associatedItemListing.Remove(1);

        UpdateDisplay();


        if (associatedItemListing.EmptyListing)
        {
            FinalPossiblePress.BroadcastEvent(this,
                new InventoryButtonArgs { ItemId = associatedItemListing.item.itemId });

            Destroy(gameObject);
        } 
        
            InventoryButtonPressed.BroadcastEvent(this,
                new InventoryButtonArgs { 
                    ItemToPass = associatedItemListing.item , 
                    ItemId = associatedItemListing.item.itemId
                });
        

    }

    protected override void OnDestroy()
    {
        if(associatedItemListing != null)
        {
            associatedItemListing.ItemListingChanged -= OnItemListingChange;
        }
    }
}

public class InventoryButtonArgs: ItemEventArgs
{
    public int ItemId { get; set; }
    public bool PassByItemId => ItemToPass == null;
}
