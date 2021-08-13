using UnityEngine;

public class Interactable : MonoBehaviour, IDistributeItems<ItemDropper>
{
    public ItemDropper Distributor { get; private set; }

    float DistanceFromPlayer => Vector3.Distance(transform.position, PlayerBehavior.PlayerPosition);

    void Start()
    {
        Distributor = new ItemDropper(this, this.transform);
        Distributor.SetItemDropData((1, 1), (2, 1));
    }

    void Update()
    {
        if (DistanceFromPlayer <= 1f && Input.GetKeyDown(KeyCode.Space))
        {
            Distributor.Distribute();
        }
    }
}
