
using UnityEngine.UI;
using TMPro;

public class InventoryButton : Button
{
    public string ItemNameToDisplay => associatedItem == null ? string.Empty : associatedItem.DisplayName;
    public string ItemAmountToDisplay { get; private set; }

    Item associatedItem;

    public TextMeshProUGUI textMesh => gameObject.GetOrAddComponentInChildren<TextMeshProUGUI>();

    protected override void Start()
    {
        base.Start();

        transform.GetChild(0).gameObject.SetActive(true);

        onClick.AddListener(() => OnClickPrintToString());

    }

    public void SetInventoryDisplay(Item i, string itemAmount)
    {
        associatedItem = i;
        ItemAmountToDisplay = itemAmount;

        textMesh.text = ToString();
    }

    void OnClickPrintToString()
    {
        print(ToString());
    }

    internal void OnInventoryAdjustment(object source, InventoryAdjustmentEventArgs args)
    {

        if(args.Listing.item.itemId.Equals(associatedItem.itemId))
        {
            if (args.Removing)
            {
                Destroy(gameObject);
            }

            SetInventoryDisplay(args.Listing.item, args.NewAmountToDisplay);

        }
    }

    public override string ToString()
    {
        return $"{ItemNameToDisplay}: {ItemAmountToDisplay}";
    }

}
