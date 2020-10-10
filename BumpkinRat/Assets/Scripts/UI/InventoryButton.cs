using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class InventoryButton : Button
{
    public string ItemNameToDisplay { get; private set; }
    public string ItemAmountToDisplay { get; private set; }

    public TextMeshProUGUI textMesh => gameObject.GetOrAddComponentInChildren<TextMeshProUGUI>();

    protected override void Start()
    {
        base.Start();

        transform.GetChild(0).gameObject.SetActive(true);

        onClick.AddListener(() => OnClickPrintToString());

    }

    public void SetInventoryDisplay(string itemName, string itemAmount)
    {
        ItemNameToDisplay = itemName;
        ItemAmountToDisplay = itemAmount;

        textMesh.text = ToString();
    }


    void OnClickPrintToString()
    {
        print(ToString());
    }

    internal void OnInventoryAdjustment(object source, InventoryAdjustmentEventArgs args)
    {

        if (args.ItemToAdjust.Equals(ItemNameToDisplay))
        {
            if (args.Removing)
            {
                Destroy(gameObject);
            }

            SetInventoryDisplay(args.ItemToAdjust, args.NewAmountToDisplay);
        }
    }

    public override string ToString()
    {
        return $"{ItemNameToDisplay}: {ItemAmountToDisplay}";
    }

}
