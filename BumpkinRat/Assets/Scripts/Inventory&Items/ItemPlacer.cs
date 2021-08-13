using System.Collections.Generic;
using UnityEngine;
using Items;

public class ItemPlacer : ItemDistrubutionSettings
{
    private readonly IDistributeItems<ItemPlacer> placer;

    private Vector3[] placementPositions = new Vector3[0];

    private OccupiablePositionContainer occupiablePositionContainer;

    public bool spawnPrefab;

    GameObject prefab;

    public ItemPlacer(IDistributeItems<ItemPlacer> placer)
    {
        this.placer = placer;
        clearOnDistribute = true;
        occupiablePositionContainer = new OccupiablePositionContainer(placementPositions);
        //RecipeProgressTracker.onRecipeCompleted.AddListener(PlaceFromRecipe);
    }

    public void SetPrefabToPlace(GameObject p)
    {
        prefab = p;
    }

    public override void Distribute()
    {
        if (!CanDistribute)
        {
            return;
        }

        for(int i = 0; i < ItemsToDrop.Count; i++)
        {
            var position = occupiablePositionContainer.GetNext();
            if (position != null)
            {
                ItemObjectWorldElement itemObject = ItemDataManager.SpawnFromPrefabPath(ItemsToDrop[i].ToDrop);//ItemsToDrop[i].ToDrop.SpawnFromPrefabPath();
                position.Occupy(itemObject);
            } else
            {
                Debug.LogWarning("Can no longer spawn items here!");
                break;
            }
        }

        if (clearOnDistribute)
        {
            ItemsToDrop.Clear();
        }
    }
}