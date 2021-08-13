using System.Linq;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System.Collections;

public class CustomerQueueHead : MonoBehaviour
{
    public int capacity;
    public string key;
    Queue<CustomerNpc> customersInQueue;
    static Dictionary<string, CustomerQueueHead> queueHeadLookupDict;

    public Vector3[] positionOffsets;

    private void Awake()
    {
        InitializeQueueHeadLookup();
        SetNameAndAdjustForDuplicates();
        InitializeQueue();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            LeaveQueueOnCustomerOrderComplete("", true);
        }
    }

    static void InitializeQueueHeadLookup()
    {
        if (queueHeadLookupDict == null)
        {
            queueHeadLookupDict = new Dictionary<string, CustomerQueueHead>();
        }
    }

    public static void EnqueueToTagged(CustomerNpc npc, string queueHeadLookup = "")
    {
        if (queueHeadLookupDict.CollectionIsNotNullOrEmpty())
        {
            CustomerQueueHead queueHead;
            if (string.IsNullOrEmpty(queueHeadLookup) || !queueHeadLookupDict.ContainsKey(queueHeadLookup))
            {
                queueHead = queueHeadLookupDict.ElementAt(0).Value;
            }
            else
            {
                queueHead = queueHeadLookupDict[queueHeadLookup];
            }

            queueHead.EnqueueCustomers(npc);
        }
    }

    static CustomerQueueHead GetCustomerQueueHead(string queueHeadLookup = "") 
    {

        if (queueHeadLookupDict.CollectionIsNotNullOrEmpty())
        {
            CustomerQueueHead queueHead;
            if (string.IsNullOrEmpty(queueHeadLookup) || !queueHeadLookupDict.ContainsKey(queueHeadLookup))
            {
                queueHead = queueHeadLookupDict.ElementAt(0).Value;
            }
            else
            {
                queueHead = queueHeadLookupDict[queueHeadLookup];
            }

            return queueHead;

        }

        return null;
    }

    void SetNameAndAdjustForDuplicates()
    {
        string tryName = string.IsNullOrEmpty(key) ? gameObject.name: key;
        int iterations = 0;
        while (queueHeadLookupDict.ContainsKey(tryName))
        {
            iterations++;
            tryName = $"{gameObject.name}_{iterations}";
        }

        gameObject.name = tryName;
        queueHeadLookupDict.Add(tryName, this);
    }

    void InitializeQueue()
    {
        customersInQueue = new Queue<CustomerNpc>(capacity);
    }
    void EnqueueCustomers(CustomerNpc npc)
    {
        if (!AtCapacity())
        {
            int count = customersInQueue.Count;
            customersInQueue.Enqueue(npc);
            PositionCustomer(count, npc);
        }

    }

    bool AtCapacity()
    {
        return customersInQueue.CollectionCountEquals(capacity);
    }

    void PositionCustomer(int positionIndex, CustomerNpc npc)
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

    IEnumerator MoveFromView(bool remain)
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

    public static void LeaveQueueOnCustomerOrderComplete(string queueHead, bool remainInScene)
    {
        CustomerQueueHead head = GetCustomerQueueHead(queueHead);
        head.StartCoroutine(head.MoveFromView(remainInScene));
    }

    Vector3 GetPosition(int index)
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
