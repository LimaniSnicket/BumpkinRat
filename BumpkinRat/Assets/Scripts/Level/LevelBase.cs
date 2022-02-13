using BumpkinRat.Crafting;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelBase : MonoBehaviour
{
    [SerializeField]
    protected int levelId;

    protected Dictionary<int, OverworldDialogue> overworldDialogue;

    protected LevelData levelData;

    protected CustomerOrderManager customerOrderManager;

    protected CustomerOrder[] craftingOrders;

    protected IItemDistribution itemDistributer;

    protected ItemDropFactory itemDropFactory;

    public int Id => levelId;

    public string Information { get; private set; }

    public void ActivateLevel()
    {
        levelData = LevelDataHelper.GetLevelDataById(levelId);
        overworldDialogue = levelData.GenericLevelDialogue.ToDictionary(k => k.npcId, k => k.dialogue);

        Information = $"({levelData.LevelId}){levelData.LevelName}. Contains {levelData.OrdersInLevel.Length} order(s) to fulfill.";

        LevelDataHelper.SetActiveLevel(this);
    }

    public void DistributeDropOnStartItems()
    {
        if(itemDropFactory == null)
        {
            itemDropFactory = new ItemDropFactory();
        }

        if (levelData.DropOnStart.CollectionIsNotNullOrEmpty())
        {
            for (int i = 0; i < levelData.DropOnStart.Length; i++)
            {
                var itemDrop = itemDropFactory.CreateFromString(levelData.DropOnStart[i]);
                itemDistributer.AddItemToDrop(itemDrop);
            }

            itemDistributer.Distribute();
        }
    }

    public OverworldDialogue GetOverworldDialogueForNpc(int id)
    {
        if(overworldDialogue.TryGetValue(id, out OverworldDialogue dialogue))
        {
            return dialogue;
        }

        return null;
    }
}
