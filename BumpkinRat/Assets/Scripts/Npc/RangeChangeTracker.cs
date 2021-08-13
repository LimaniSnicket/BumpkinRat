using UnityEngine;

public struct RangeChangeTracker
{
    public float maxRange;

    private readonly Transform transform;

    public bool PlayerInRange => InRange(GetDistanceFromPlayer(transform));

    public float DistanceFromPlayer => GetDistanceFromPlayer(transform);

    public RangeChangeTracker(Transform t, float max)
    {
        maxRange = max;
        transform = t;
    }
    public bool InRange(float distance)
    {
        return distance <= maxRange;
    }
    public static float GetDistanceFromPlayer(Transform obj)
    {
        return Vector3.Distance(obj.position, PlayerBehavior.PlayerPosition);
    }
}

