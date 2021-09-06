using System.Linq;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System.Collections;

public class CustomerQueueHead : MonoBehaviour
{
    [SerializeField] private int capacity;
    [SerializeField] private string key;

    private Queue<CustomerNpc> customersInQueue;

    public Vector3[] positionOffsets;

    public string Key => string.IsNullOrEmpty(key) ? gameObject.name : key;

    private void Awake()
    {
        CustomerQueueHeadManager.SetNameAndAdjustForDuplicates(this);
        InitializeQueue();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            LeaveQueueOnCustomerOrderComplete("", true);
        }
    }

    private void InitializeQueue()
    {
        customersInQueue = new Queue<CustomerNpc>(capacity);
    }

    internal void AdjustKeyForDuplicate(int append)
    {
        key = $"{gameObject.name}_{append}";
        gameObject.name = key;
    }

    public void EnqueueCustomers(CustomerNpc npc)
    {
        if (!AtCapacity())
        {
            int count = customersInQueue.Count;
            customersInQueue.Enqueue(npc);
            this.PositionCustomer(count, npc);
        }
    }

    private bool AtCapacity()
    {
        return customersInQueue.CollectionCountEquals(capacity);
    }

    private void PositionCustomer(int positionIndex, CustomerNpc npc)
    {
        if(positionIndex > capacity)
        {
            //waitlist position
        } else
        {
            if (positionIndex <= 0)
            {
                npc.transform.position = transform.position;
            }
            else
            {
                Vector3 position = GetPosition(positionIndex);
                npc.transform.position = position;
            }
        }
    }

    void SendToWaitList(CustomerNpc npc)
    {

    }

    private IEnumerator MoveFromView(bool remain)
    {
        if (customersInQueue.CollectionIsNotNullOrEmpty())
        {
            CustomerNpc handling = customersInQueue.Dequeue();
            Vector3 right = handling.transform.right;
            handling.transform.DOMove(handling.transform.position + right * 4, 1);
            yield return new WaitForSeconds(1);
            if (!remain)
            {
                Destroy(handling.gameObject);
            }
        } else
        {
            yield return null;
        }
    }

    private void LeaveQueueOnCustomerOrderComplete(string key, bool remainInScene)
    {
        if (CustomerQueueHeadManager.TryGetCustomerQueueHead(key, out CustomerQueueHead queueHead))
        {
            queueHead.StartCoroutine(MoveFromView(remainInScene));
        }
    }

    private Vector3 GetPosition(int index)
    {
        if (positionOffsets.CollectionIsNotNullOrEmpty() && index > 0)
        {
            if(index > positionOffsets.Length)
            {
                int diff = index - positionOffsets.Length;
                return (positionOffsets[positionOffsets.Length - 1] - Vector3.forward * 1/diff) + transform.position;
            }

            return positionOffsets[index - 1] + transform.position;
        }
        return transform.position;
    }

    private void OnDrawGizmos()
    {
        if (!positionOffsets.CollectionIsNotNullOrEmpty()) { return; }
        Gizmos.color = ColorX.DeepPink;
        Gizmos.DrawSphere(transform.position, 0.3f);
        foreach (Vector3 v in positionOffsets)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(v + transform.position, 0.2f);
        }
    }
}

public class CustomerQueueHeadManager
{
    private static Dictionary<string, CustomerQueueHead> queueHeadLookupDict;
    public static bool LookupValid => queueHeadLookupDict.CollectionIsNotNullOrEmpty();

/*    public static void EnqueueCustomerToQueueHead(CustomerNpc npc, string key = "")
    {
        if (LookupValid)
        {
            if (TryGetCustomerQueueHead(key, out CustomerQueueHead queueHead))
            {
                queueHead.EnqueueCustomers(npc);
            }
        }
    }*/
    public static void SetNameAndAdjustForDuplicates(CustomerQueueHead queueHead)
    {
        InitializeLookup();

        int iterations = 0;
        while (queueHeadLookupDict.ContainsKey(queueHead.Key))
        {
            iterations++;
            queueHead.AdjustKeyForDuplicate(iterations);
        }
        queueHeadLookupDict.Add(queueHead.Key, queueHead);
    }

    public static bool TryGetCustomerQueueHead(string key, out CustomerQueueHead queueHead)
    {
        queueHead = GetCustomerQueueHead(key);
        return queueHead != null;
    }

    private static CustomerQueueHead GetCustomerQueueHead(string queueHeadLookup = "")
    {
        CustomerQueueHead head = null;

        if (LookupValid)
        {
            if (string.IsNullOrEmpty(queueHeadLookup) || !queueHeadLookupDict.TryGetValue(queueHeadLookup, out head))
            {
                return queueHeadLookupDict.ElementAt(0).Value;
            }
        }

        return head;
    }

    private static void InitializeLookup()
    {
        if (queueHeadLookupDict == null)
        {
            queueHeadLookupDict = new Dictionary<string, CustomerQueueHead>();
        }
    }
}