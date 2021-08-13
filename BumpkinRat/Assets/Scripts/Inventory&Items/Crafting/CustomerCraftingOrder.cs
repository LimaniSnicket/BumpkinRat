using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[Serializable]
public class CustomerOrder : IComparable<Recipe>
{
    public static Queue<CustomerOrder> ActiveOrders { get; private set; } = new Queue<CustomerOrder>();
    private static List<CustomerOrder> orderBacklog = new List<CustomerOrder>();
    private static RewardProvisioner rewardProvisioner;

    [SerializeField] private OrderDetails orderDetails;

    private CustomerDialogue orderDialogueCache;

    [SerializeField] private int npcId;

    public string CustomerName { get; private set; }

    public string DialogueDetails => $"{npcId}-{orderDialogueCache.levelId}-{orderDialogueCache.dialogueId}";

    public void Initialize<T>(T host) where T : MonoBehaviour, ILevel
    {
        host.StartCoroutine(WaitForNpcDatabase(host));
    }

    public static CustomerOrder[] CreateCustomerOrders(params (int, OrderType, int)[] orderParams)
    {
        return orderParams.Select(o => CreateCustomerOrder(o.Item1, o.Item2, o.Item3)).ToArray();
    }

    public static CustomerOrder CreateCustomerOrder(int npcId, OrderType orderType, int orderId)
    {
        if(rewardProvisioner == null)
        {
            rewardProvisioner = new RewardProvisioner();
        }

        CustomerOrder order = new CustomerOrder
        {
            npcId = npcId,
            orderDetails = new OrderDetails
            {
                orderLookupId = orderId,
                orderType = orderType,
                cashReward = 10,
                rewardItemIds = new int[] { 3 },
                orderTitle = "Nunchuck Nightmare!",
                orderPrompt = "<b><color=red>Attach</color></b> the <b><color=red>Broken Links</color></b> to fix it for your cuzzo!"
            }
        };

        return order;
    }

    public static void InitializeAll<T>(T host, params CustomerOrder[] orders) where T: MonoBehaviour, ILevel
    {
        if (orders.CollectionIsNotNullOrEmpty())
        {
            foreach(CustomerOrder order in orders)
            {
                order.Initialize(host);
            }
        }
    }

    public static void EvaluateAgainstRecipe(Recipe r)
    {
        CustomerOrder order;
        if (TryGetNextUpOrder(out order))
        {
            if (order.CompareTo(r) == 0)
            {
                CompleteActiveOrder(order);
            }
        }
    }

    static bool TryGetNextUpOrder(out CustomerOrder order)
    {
        bool valid = ActiveOrders.CollectionIsNotNullOrEmpty();
        order = valid ? ActiveOrders.Peek() : null;
        return valid;
    }

    public int CompareTo(Recipe other)
    {
        return orderDetails.orderType.Equals(OrderType.CRAFTING) ? orderDetails.orderLookupId.CompareTo(other.id) : -2;
    }

    static void CompleteActiveOrder(CustomerOrder order)
    {
        RewardOnCompletedOrder(order);
        order.CacheCustomerDialogue();
    }

    static void RewardOnCompletedOrder(CustomerOrder order)
    {
        rewardProvisioner.DropItemRewards(order.orderDetails);
    }

    void CacheCustomerDialogue()
    {
        DialogueTracker.CacheCompletedDialogue(DialogueDetails);
    }

    public CustomerDialogue GetCustomerDialogue()
    {
        return orderDialogueCache;
    }

    public static string GetActiveOrderPromptDetails()
    {
        if (!ActiveOrders.CollectionIsNotNullOrEmpty())
        {
            return string.Empty;
        }
        OrderDetails deets = ActiveOrders.Peek().orderDetails;
        return $"<b>{deets.orderTitle}</b>\n{deets.orderPrompt}";
    }

    IEnumerator WaitForNpcDatabase(ILevel level)
    {
        while (!NpcData.CanRead)
        {
            Debug.Log("Waiting for NpcData to load");
            yield return new WaitForEndOfFrame();
        }
        NpcDatabaseEntry entry = NpcData.GetDatabaseEntry(npcId);
        CustomerName = entry.NpcName;

        orderDialogueCache = entry.GetCustomerDialogue(level.LevelData.LevelId, 0);
    }

    public static void QueueCustomers<T>(T host, params CustomerOrder[] orders) where T: MonoBehaviour, ILevel
    {
        if(ActiveOrders == null)
        {
            ActiveOrders = new Queue<CustomerOrder>();
        }

        foreach(var order in orders)
        {
            ActiveOrders.Enqueue(order);
            ActiveOrders.Peek().Initialize(host);
        }
    }

    public static void QueueAndSpawnCustomers<T>(T host, string queueHead = "", params CustomerOrder[] orders) where T: MonoBehaviour, ILevel
    {
        if(ActiveOrders == null)
        {
            ActiveOrders = new Queue<CustomerOrder>();
        }

        for(int i = 0; i < orders.Length; i++)
        {
            ActiveOrders.Enqueue(orders[i]);

            CustomerNpc spawned = CustomerNpc.GetCustomerNpc(orders[i].npcId);
            CustomerQueueHead.EnqueueToTagged(spawned, queueHead);
            ActiveOrders.Peek().Initialize(host);
        }

    }

    public static void QueueCustomersIntoFreshQueue<T>(T host, params CustomerOrder[] orders) where T: MonoBehaviour, ILevel
    {
        SendActiveOrdersToBacklog();
        QueueCustomers(host, orders);
    }

    static void SendActiveOrdersToBacklog()
    {
        if(ActiveOrders.Count > 0)
        {
            orderBacklog.AddRange(ActiveOrders);
        }

        ActiveOrders.Clear();
    }

    public override string ToString()
    {
        return $"{CustomerName}: [{orderDetails.orderType}, {orderDetails.orderLookupId}]";
    }
}

class RewardProvisioner : IDistributeItems<ItemProvisioner>
{
    public ItemProvisioner Distributor { get; set; }

    public RewardProvisioner()
    {
        Distributor = new ItemProvisioner(this);
    }

    public void DropItemRewards(OrderDetails order)
    {
        GetRewardsToDrop(order);
        Distributor.Distribute();
    }

    private void GetRewardsToDrop(OrderDetails order)
    {
        Distributor.SetItemDropData(order.rewardItemIds);
    }

    public override string ToString()
    {
        return string.Join("-- ", Distributor.ItemsToDrop.Select(s => s.ItemToDropName));
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
}

public enum OrderType
{
    CRAFTING = 0,
    PAINTING = 1,
    REPAIRING = 2
}
