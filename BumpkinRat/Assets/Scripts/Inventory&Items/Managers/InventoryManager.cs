using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // public string itemDataPath;
    private Inventory activeInventory;

    public GameObject inventoryMenuObject;

    public Inventory ActiveInventory => activeInventory;

    private void OnEnable()
    {
        Collectable.CollectItem += OnCollectedItem;
        ItemCrafter.CraftedItem += OnCraftedItem;

        ItemProvisioner.ItemProvisioning += OnCollectedItem;
        InventoryButton.FinalPossiblePress += OnInventoryButtonPressed;

        ItemObjectBehaviour.PlaceItemBackInInventory += OnPlaceItemObjectBack;

        activeInventory = new Inventory();
    }
    void OnCollectedItem(object source, ItemEventArgs args)
    {
        activeInventory.AddToInventory(args);
        Debug.Log(args.ItemToPass.DisplayName + " Collected. Adding to Inventory");
    }

    void OnCraftedItem(object source, CraftingEventArgs args)
    {
        if (!args.craftedRecipe.CraftableWithCurrentInventory(activeInventory)) 
        { 
            Debug.Log("Insufficient materials for crafting"); 
            return; 
        }

        activeInventory.AddCraftedRecipeOutputToInventory(args.craftedRecipe, args.craftedAmount);
        
        Debug.LogFormat("Created {0} {1}(s) and adding to Inventory", args.craftedAmount, args.craftedRecipe.IdentifiableName);
    }

    void OnInventoryButtonPressed(object source, ItemEventArgs args)
    {
        activeInventory.RemoveFromInventory(args.ItemToPass.itemId);
    }

    void OnPlaceItemObjectBack(object source, ItemEventArgs args)
    {
        activeInventory.AddToInventory(args);
    }

    private void OnDestroy()
    {
        Collectable.CollectItem -= OnCollectedItem;
        ItemCrafter.CraftedItem -= OnCraftedItem;
        ItemProvisioner.ItemProvisioning -= OnCollectedItem;
        InventoryButton.FinalPossiblePress -= OnInventoryButtonPressed;

        ItemObjectBehaviour.PlaceItemBackInInventory -= OnPlaceItemObjectBack;

    }
}