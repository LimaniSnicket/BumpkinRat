using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Collectable : MonoBehaviour
{
    public string itemName;
    public int amount;
    public static event EventHandler<CollectableEventArgs> Collected;

    private void FixedUpdate()
    {
        float y = MathfX.PulseSineFloat(0.005f, 0.25f, 0, 1);
        Vector3 curr = transform.position + new Vector3(0, y, 0);
        transform.position = curr;
    }

    public virtual void OnCollected(int amnt = 1)
    {
        if(Collected != null) { Collected(this, new CollectableEventArgs() { 
            CollectableName = itemName, 
            CollectedAmount = Math.Max(amnt, amount) 
        }) ; }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            OnCollected();
            Destroy(gameObject);
        }
    }

    public void SetItemName(string item)
    {
        itemName = item;
    }
}

public class CollectableEventArgs : EventArgs
{
    public string CollectableName { get; set; }
    public int CollectedAmount { get; set; }
}
