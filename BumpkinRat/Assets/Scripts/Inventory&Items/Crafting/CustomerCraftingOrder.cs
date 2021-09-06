using System;
using System.Collections;
using UnityEngine;


[Serializable]
public class CustomerOrder : IComparable<Recipe>
{
    [SerializeField] private OrderDetails orderDetails;

    private CustomerDialogue orderDialogueCache;

    [SerializeField] private int npcId;

    private NpcDatabaseEntry customerData;

    public OrderDetails OrderDetails => orderDetails;

    public int NpcId => npcId;
    public string CustomerName { get; private set; }

    public string DialogueDetails => $"{npcId}-{orderDialogueCache.levelId}-{orderDialogueCache.dialogueId}";

    public CustomerOrder(int npcId, OrderDetails details)
    {
        this.npcId = npcId;
        this.orderDetails = details;
    }

    public void InitializeCustomerData(ILevel level)
    {
        level.LevelBehavior.StartCoroutine(this.GetCustomerDataFromNpcDatabase(level));
    }

    public int CompareTo(Recipe other)
    {
        return orderDetails.orderType.Equals(OrderType.CRAFTING) ? orderDetails.orderLookupId.CompareTo(other.id) : -2;
    }

    public void CacheCustomerDialogue()
    {
        DialogueTracker.CacheCompletedDialogue(DialogueDetails);
    }

    public CustomerDialogue GetCustomerDialogue()
    {
        return orderDialogueCache;
    }

    private IEnumerator GetCustomerDataFromNpcDatabase(ILevel level)
    {
        yield return new WaitUntil(() => NpcData.CanRead);

        this.customerData = NpcData.GetDatabaseEntry(npcId);
        orderDialogueCache = this.customerData.GetCustomerDialogue(level.LevelData.LevelId, 0);
    }

    public override string ToString()
    {
        return $"{CustomerName}: [{orderDetails.orderType}, {orderDetails.orderLookupId}]";
    }
}

class RewardProvisioner 
{
    private IItemDistribution itemProvisioner;

    public RewardProvisioner()
    {
        itemProvisioner = new ItemProvisioner();
    }

    public void DropItemRewards(OrderDetails order)
    {
        GetRewardsToDrop(order);
        itemProvisioner.Distribute();
    }

    private void GetRewardsToDrop(OrderDetails order)
    {
        itemProvisioner.AddItemsToDrop(order.rewardItemIds);
    }
}

[Serializable]
public struct OrderDetails
{
    public OrderType orderType;
    public int orderLookupId;

    public string orderTitle;
    public string orderPrompt;

    public int[] rewardItemIds;
    public float cashReward;

    public override string ToString()
    {
        return $"Order Details {orderLookupId}: TYPE = {orderType}; \n{DetailsToString()}";
    }
    public string DetailsToString()
    {
        return $"<b>{orderTitle}</b>\n{orderPrompt}";
    }
}

public enum OrderType
{
    CRAFTING = 0,
    PAINTING = 1,
    REPAIRING = 2
}
