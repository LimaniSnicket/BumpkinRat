using UnityEngine.UI;
using TMPro;
using System;

public class InventoryButton : Button
{
    private ItemListing associatedItemListing;

    private TextMeshProUGUI textMesh;

    public static event EventHandler<ItemEventArgs> InventoryButtonPressed, FinalPossiblePress;

    public static bool CanSpawnItems { get; private set; }

    protected override void Awake()
    {
        textMesh = gameObject.GetOrAddComponentInChildren<TextMeshProUGUI>();
    }

    protected override void Start()
    {
        base.Start();

        transform.GetChild(0).gameObject.SetActive(true);

        onClick.AddListener(() => OnClickBroadcastButtonPress());
    }

    private void Update()
    {
        interactable = CanSpawnItems;
    }

    public static void EnableItemSpawning()
    {
        CanSpawnItems = true;
    }

    public static void DisableItemSpawning()
    {
        CanSpawnItems = false;
    }

    public void SetFromItemListing(ItemListing listing)
    {
        listing.ItemListingChanged += OnItemListingChange;
        associatedItemListing = listing;
        this.UpdateDisplay();
    }

    private void OnItemListingChange(object source, EventArgs args)
    {
        this.UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        try
        {
            if (textMesh == null)
            {
                textMesh = this.GetComponentInChildren<TextMeshProUGUI>();
            }
        }
        catch (NullReferenceException)
        {
            return;
        }

        textMesh.text = associatedItemListing.ToString();
    }

    private void OnClickBroadcastButtonPress()
    {
        associatedItemListing.Remove(1);

        this.UpdateDisplay();

        ItemEventArgs args = new ItemEventArgs
        {
            ItemToPass = associatedItemListing.Item,
            AmountToPass = 1
        };

        if (associatedItemListing.IsEmpty)
        {
            FinalPossiblePress.BroadcastEvent(this, args);

            Destroy(gameObject);
        }

        InventoryButtonPressed.BroadcastEvent(this, args);
    }

    protected override void OnDestroy()
    {
        if(associatedItemListing != null)
        {
            associatedItemListing.ItemListingChanged -= OnItemListingChange;
        }

        onClick.RemoveAllListeners();
    }
}
