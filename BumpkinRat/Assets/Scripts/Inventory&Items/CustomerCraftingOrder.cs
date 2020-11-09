using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;


[Serializable]
public class CustomerOrder: IComparable<Recipe>
{
    public static Queue<CustomerOrder> ActiveOrders { get; private set; } = new Queue<CustomerOrder>();
    private static List<CustomerOrder> orderBacklog = new List<CustomerOrder>();
    private static RewardProvisioner rewardProvisioner;

    public OrderDetails orderDetails;

    private CustomerDialogue orderDialogueCache;

    public int npcId;

    public string CustomerName { get; private set; }

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
                rewardItemIds = new int[] { 3 }
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
                RewardOnCompletedOrder(order);
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
        return orderDetails.orderType.Equals(OrderType.CRAFTING) ? orderDetails.orderLookupId.CompareTo(other.recipeId) : -2;
    }

    static void RewardOnCompletedOrder(CustomerOrder order)
    {
        rewardProvisioner.Reward(order.orderDetails);
    }

    public CustomerDialogue GetCustomerDialogue()
    {
        return orderDialogueCache;
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

        orderDialogueCache = entry.GetCustomerDialogue(level.LevelId, 0);
    }

    public static Queue<CustomerOrder> GenerateCustomerQueue()
    {
        return new Queue<CustomerOrder>();
    }

    public static void QueueCustomers(params CustomerOrder[] orders)
    {
        foreach(var order in orders)
        {
            ActiveOrders.Enqueue(order);
        }
        Debug.Log(VisualizeQueue());

    }

    public static void QueueCustomersIntoFreshQueue(params CustomerOrder[] orders)
    {
        SendActiveOrdersToBacklog();
        QueueCustomers(orders);
    }

    static void SendActiveOrdersToBacklog()
    {
        if(ActiveOrders.Count > 0)
        {
            orderBacklog.AddRange(ActiveOrders);
        }

        ActiveOrders.Clear();
    }

    static string VisualizeQueue()
    {
        int pos = 1;
        return string.Join($" {pos++}. ", ActiveOrders.Select(o => o.CustomerName));
    }

    public override string ToString()
    {
        return $"{CustomerName}: [{orderDetails.orderType.ToString()}, {orderDetails.orderLookupId}]";
    }
}

class RewardProvisioner : IDistributeItems<ItemProvisioner>
{
    public ItemProvisioner ItemDistributor { get; set; }
    public List<ItemDrop> ItemDropData { get; set; }

    public RewardProvisioner()
    {
        ItemDistributor = new ItemProvisioner(this);
        ItemDropData = new List<ItemDrop>();
    }
    public RewardProvisioner(OrderDetails order)
    {
        ItemDistributor = new ItemProvisioner(this);
        ItemDropData = ItemDrop.GetListOfItemsToDrop(order.rewardItemIds);
    }

    public void Reward(OrderDetails order)
    {
        UpdateRewards(order);
        ItemDistributor.Distribute();
    }

    void UpdateRewards(OrderDetails order)
    {
        ItemDropData.Clear();
        ItemDropData = ItemDrop.GetListOfItemsToDrop(order.rewardItemIds);
    }

    public override string ToString()
    {
        return string.Join("-- ", ItemDropData.Select(s => s.ItemToDropName));
    }
}

[Serializable]
public struct OrderDetails
{
    public OrderType orderType;
    public int orderLookupId;

    public int[] rewardItemIds;
    public float cashReward;
}

public enum OrderType
{
    CRAFTING = 0,
    PAINTING = 1,
    REPAIRING = 2
}

public class CustomerDialogueTracker
{
    public CustomerDialogue Tracking { get; private set; }
    public int DialogueIndex { get; private set; } = -1;

    public bool IntroDialogueActive => DialogueIndex < 0;

    public bool DialogueComplete { get; private set; }

    public event EventHandler CustomerDialogueExpired;
    public static event EventHandler IntroDialogueEnded;

    public static CustomerDialogueTracker GetCustomerDialogueTracker(CustomerDialogue dialogueToTrack)
    {
        return new CustomerDialogueTracker { Tracking = dialogueToTrack, DialogueIndex = -1 };
    }

    public PlayerResponse[] GetResponses()
    {
        try
        {

            return DialogueIndex >= 0 ? Tracking.promptedCustomerDialogue[DialogueIndex].possibleResponses : Tracking.introResponses;

        }catch (IndexOutOfRangeException)
        {
            return Array.Empty<PlayerResponse>();
        }
    }

    public void Reset(CustomerDialogue newTracking)
    {
        Tracking = newTracking;
        DialogueIndex = 0;
        DialogueComplete = false;
    }

    public void Advance()
    {
        if(DialogueIndex == -1)
        {
            IntroDialogueEnded.BroadcastEvent(this);
        }

        DialogueIndex++;
        DialogueComplete = Tracking.promptedCustomerDialogue.Length <= DialogueIndex;

        Debug.Log($"Advancing Tracker...{DialogueIndex}: Complete = {DialogueComplete}");

        if (DialogueComplete)
        {
            CustomerDialogueExpired.BroadcastEvent(this);
        }
    }
}

[Serializable]
public struct CustomerDialogue
{
    public int dialogueId;
    public int levelId;

    public string[] introLines;
    public PlayerResponse[] introResponses;
    public CustomerResponse[] promptedCustomerDialogue;
    public bool isValid => introLines.CollectionIsNotNullOrEmpty() 
        && promptedCustomerDialogue.CollectionIsNotNullOrEmpty() 
        && introResponses.CollectionIsNotNullOrEmpty();
    public int IntroLineCount => isValid ? introLines.Length : 0;

    public override string ToString()
    {
        return $"[{dialogueId}@{levelId}]: {introLines[0]}...";
    }
}

[Serializable]
public struct CustomerResponse
{
    public string low, medium, high;

    public DialogueResponse baseResponse;
    public DialogueResponse[] additionalResponses;

    public PlayerResponse[] possibleResponses;

    Dictionary<string, string> GetValidResponses()
    {
        Dictionary<string, string> responseMap = new Dictionary<string, string>();

        if(low.NotNullOrEmpty()) responseMap.Add(nameof(low), low);
        if (medium.NotNullOrEmpty()) responseMap.Add(nameof(medium), medium);
        if (high.NotNullOrEmpty()) responseMap.Add(nameof(high), high);

        return responseMap;
    }

    public string GetCustomerResponse(string level)
    {
        Dictionary<string, string> responses = GetValidResponses();

        if (DialogueX.PriorityLevels.Contains(level) && responses.CollectionIsNotNullOrEmpty())
        {
            try
            {
                return responses[level];
            }
            catch(KeyNotFoundException)
            {
                return responses.ElementAt(0).Value;
            }

        }

        return string.Empty;
    }
}

[Serializable]
public struct PlayerResponse
{
    public string responseLevel;
    public string displayDialogue;
} 

[Serializable]
public struct DialogueResponse
{
    public string displayDialogue;
    public int qualityThreshold;
}


