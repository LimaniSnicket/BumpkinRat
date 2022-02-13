using UnityEngine;

public interface ILevel 
{
    int Id { get; }
    LevelData LevelData { get; }
    MonoBehaviour LevelBehavior { get; }
}


