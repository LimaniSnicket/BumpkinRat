using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ItemScrollView : MonoBehaviour, IUiFunctionality<InventoryMenu>
{
    public InventoryManager inventoryManager;

    public GameObject inventoryButtonPrefab;

    public InventoryMenu MenuFunctionObject { get; set; }

    public ScrollRect itemScroller;

    List<InventoryButton> inventoryButtons;
    Dictionary<string, InventoryButton> inventoryButtonLookup => inventoryButtons.ToDictionary(k => k.ItemNameToDisplay);

    public Transform spawnAtTransform; 
    public GameObject spawnPrefab;

    public Workbench workbench;

    //todo replace with Dictionary<ItemCategory, List<Button>> if/when we categorize buttons! 
    //or a gameobject that is the root of the button list?
    //OR Lookup buttons by item name but when they're spawned just spawn them on the appropriate item tab!
    //multiple content fitter game objects?

    private void Start()
    {
        itemScroller = GetComponentInChildren<ScrollRect>();

        MenuFunctionObject = new InventoryMenu(itemScroller.gameObject);
       

        if(inventoryManager == null)
        {
            try
            {
                inventoryManager = FindObjectOfType<InventoryManager>();
            } catch (NullReferenceException)
            {
                Destroy(this);
            }
        }

        inventoryManager.activeInventory.InventoryAdjusted += OnInventoryAdjustment;

    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            MenuFunctionObject.ChangeMenuStatus();
        }
    }

    void OnInventoryAdjustment(object source, InventoryAdjustmentEventArgs args)
    {

        if (args.Adding)
        {
            //SpawnButtonForItem(args.Listing.item, args.NewAmountToDisplay);
            SpawnButtonForItemListing(args.Listing);
        }
    }

    void SpawnButtonForItem(Item i, string amountLabel)
    {
        if (inventoryButtons == null)
        {
            inventoryButtons = new List<InventoryButton>();
        }

        InventoryButton inventoryButton = SpawnInventoryButtonFromPrefab();
        //inventoryButton.SetInventoryDisplay(i, amountLabel);

        inventoryButtons.Add(inventoryButton);
    }

    void SpawnButtonForItemListing(ItemListing i)
    {
        if (inventoryButtons == null)
        {
            inventoryButtons = new List<InventoryButton>();
        }

        InventoryButton inventoryButton = SpawnInventoryButtonFromPrefab();
        inventoryButton.SetItemListing(i);

        inventoryButtons.Add(inventoryButton);
    }

    public InventoryButton SpawnInventoryButtonFromPrefab()
    {
        return Instantiate(inventoryButtonPrefab, itemScroller.content.transform).GetOrAddComponent<InventoryButton>();
    }

    private void OnDestroy()
    {
        inventoryManager.activeInventory.InventoryAdjusted -= OnInventoryAdjustment;
    }
}
