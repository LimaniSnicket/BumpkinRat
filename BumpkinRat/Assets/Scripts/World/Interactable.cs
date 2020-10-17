using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour, IDistributeItems<ItemDropper>
{
    public ItemDropper ItemDistributor { get; set; }
    public List<ItemDrop> ItemDropData { get; set; }

    float DistanceFromPlayer => Vector3.Distance(transform.position, PlayerBehavior.playerPosition);

    void Start()
    {
        ItemDistributor = new ItemDropper(this);
        ItemDropData = ItemDrop.GetListOfItemsToDrop(("item_a", 1), ("item_b", 2));
    }

    void Update()
    {
        if(DistanceFromPlayer <= 1f && Input.GetKeyDown(KeyCode.Space))
        {
            ItemDistributor.DistributeAtTransform(transform);
        }
    }


}
