using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ItemScrollView : MonoBehaviour, IUiFunctionality<InventoryMenu>
{
    private Inventory activeInventory;

    public GameObject inventoryButtonPrefab;

    public InventoryMenu UiMenu { get; set; }

    private ScrollRect itemScroller;

    private List<InventoryButton> inventoryButtons;

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

        UiMenu = new InventoryMenu(itemScroller.gameObject);
       

        if(activeInventory == null)
        {
            try
            {
                InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
                activeInventory = inventoryManager.ActiveInventory;
            } catch (NullReferenceException)
            {
                Destroy(this);
            }
        }

        activeInventory.InventoryAdjusted += OnInventoryAdjustment;

    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            UiMenu.ToggleMenu();
        }
    }

    void OnInventoryAdjustment(object source, InventoryAdjustmentEventArgs args)
    {
        if (args.AdjustmentType.Equals(InventoryAdjustment.ADD))
        {
            SpawnButtonForItemListing(args.Listing);
        }
    }

    private void SpawnButtonForItemListing(ItemListing i)
    {
        if (inventoryButtons == null)
        {
            inventoryButtons = new List<InventoryButton>();
        }

        InventoryButton inventoryButton = SpawnInventoryButtonFromPrefab();
        inventoryButton.SetFromItemListing(i);

        inventoryButtons.Add(inventoryButton);
    }

    public InventoryButton SpawnInventoryButtonFromPrefab()
    {
        return Instantiate(inventoryButtonPrefab, itemScroller.content.transform).GetOrAddComponent<InventoryButton>();
    }

    private void OnDestroy()
    {
        activeInventory.InventoryAdjusted -= OnInventoryAdjustment;
    }
}
