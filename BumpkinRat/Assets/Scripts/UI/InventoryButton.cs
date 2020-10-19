
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
        associatedItemListing = listing;
        UpdateDisplay();
    }


    void UpdateDisplay()
    {
        textMesh.text = associatedItemListing.ToString();
    }

    public void SetUpWorkbenchSpawning(Workbench workbench)
    {
        workbench.SpawnOnWorkbench(associatedItemListing.item);
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
                new InventoryButtonArgs { AssociatedItem = associatedItemListing.item });
        

    }
}

public class InventoryButtonArgs: EventArgs
{
    public int ItemId { get; set; }
    public Item AssociatedItem { get; set; }

    public bool PassByItemId => AssociatedItem == null;
}
