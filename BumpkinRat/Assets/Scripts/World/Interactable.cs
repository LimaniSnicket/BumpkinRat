using UnityEngine;

public class Interactable : MonoBehaviour, IDistributeItems, ITrackDistanceToPlayer
{
    public IItemDistribution ItemDistributor { get; private set; }
    public RangeChangeTracker DistanceTracker { get; private set; }

    [SerializeField]
    private float interactionRadius = 1;
    void Start()
    {
        ItemDistributor = new ItemDropper(this, this.transform);
        ItemDistributor.AddItemsToDrop((1, 1), (2, 1));

        this.DistanceTracker = new RangeChangeTracker(this.transform, interactionRadius);
    }

    void Update()
    {
        if (DistanceTracker.PlayerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            ItemDistributor.Distribute();
        }
    }
}
