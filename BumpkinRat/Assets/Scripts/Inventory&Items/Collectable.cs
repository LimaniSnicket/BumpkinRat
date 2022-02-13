using System;
using Items;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Collectable : MonoBehaviour
{
    public string itemName;
    public int amount;

    int itemId;

    public static event EventHandler<ItemEventArgs> CollectItem;

    private void FixedUpdate()
    {
        float y = MathfX.PulseSineFloat(0.005f, 0.25f, 0, 1);
        Vector3 curr = transform.position + new Vector3(0, y, 0);
        transform.position = curr;
    }

    public virtual void OnCollected(int amnt = 1)
    {
        if (CollectItem != null) 
        {
            ItemEventArgs args = new ItemEventArgs
            {
                ItemToPass = ItemDataManager.GetItemById(itemId),
                AmountToPass = Math.Max(0, amnt)
            };

            CollectItem(this, args) ; 
        }
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

