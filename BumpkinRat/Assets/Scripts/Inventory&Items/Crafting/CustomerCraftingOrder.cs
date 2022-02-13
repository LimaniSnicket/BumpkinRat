using System;
using System.Collections;
using UnityEngine;


[Serializable]
public class CustomerOrder : IComparable<Recipe>
{
    [SerializeField] private OrderDetails orderDetails;

    private CustomerDialogue orderDialogueCache;

    public NpcDatabaseEntry CustomerData { get; private set; }

    public OrderDetails OrderDetails => orderDetails;

    public int NpcId => orderDetails.npc;
    public string CustomerName { get; private set; }

    public string DialogueDetails => $"{NpcId}-{orderDialogueCache.dialogueId}";

    public CustomerOrder(OrderDetails details)
    {
        this.orderDetails = details;
    }

    public void InitializeCustomerData(LevelBase level)
    {
        level.StartCoroutine(this.GetCustomerDataFromNpcDatabase(level));
    }

    public int CompareTo(Recipe other)
    {
        return orderDetails.orderType.Equals(OrderType.CRAFTING) ? orderDetails.id.CompareTo(other.id) : -2;
    }

    public void CacheCustomerDialogue()
    {
        DialogueTracker.CacheCompletedDialogue(DialogueDetails);
    }

    public CustomerDialogue GetCustomerDialogue()
    {
        return orderDialogueCache;
    }

    private IEnumerator GetCustomerDataFromNpcDatabase(LevelBase level)
    {
        yield return new WaitUntil(() => NpcData.CanRead);

        this.CustomerData = NpcData.GetDatabaseEntry(NpcId);
        orderDialogueCache = this.CustomerData.GetCustomerDialogue(level.Id, 0);
    }

    public override string ToString()
    {
        return $"{CustomerName}: [{orderDetails.orderType}, {orderDetails.id}]";
    }
}

class RewardProvisioner 
{
    private readonly IItemDistribution itemProvisioner;

    public RewardProvisioner()
    {
        itemProvisioner = new ItemProvisioner();
    }

    public void DropItemRewards(OrderDetails order)
    {
        this.GetRewardsToDrop(order);
        itemProvisioner.Distribute();
    }

    private void GetRewardsToDrop(OrderDetails order)
    {
        itemProvisioner.AddItemsToDrop(order.rewards);
    }
}

[Serializable]
public struct OrderDetails
{
    public int npc;
    public OrderType orderType;
    public int id;

    public string infoDisplay;

    public int[] rewards;
    public float cashReward;

    public string GetPromptDisplay()
    {
        var split = infoDisplay.Split('_');
        return $"<b>{split[0]}</b>\n{split[1]}";
    }
}

public enum OrderType
{
    CRAFTING = 0,
    PAINTING = 1,
    REPAIRING = 2
}
