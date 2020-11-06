using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;


[Serializable]
public class CustomerOrder
{
    public static Queue<CustomerOrder> ActiveOrders { get; private set; } = new Queue<CustomerOrder>();
    private static List<CustomerOrder> orderBacklog = new List<CustomerOrder>();

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
        return new CustomerOrder
        {
            npcId = npcId,
            orderDetails = new OrderDetails
            {
                orderLookupId = orderId,
                orderType = orderType
            }
        };
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

[Serializable]
public struct OrderDetails
{
    public OrderType orderType;
    public int orderLookupId;
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
    public int DialogueIndex { get; private set; }

    public bool DialogueComplete { get; private set; }

    public event EventHandler CustomerDialogueExpired;

    public static CustomerDialogueTracker GetCustomerDialogueTracker(CustomerDialogue dialogueToTrack)
    {
        return new CustomerDialogueTracker { Tracking = dialogueToTrack, DialogueIndex = 0 };
    }

    public PlayerResponse[] GetResponses()
    {
        try
        {

            return Tracking.promptedCustomerDialogue[DialogueIndex].possibleResponses;

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


