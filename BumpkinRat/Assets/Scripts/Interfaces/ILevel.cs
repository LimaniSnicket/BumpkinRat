using UnityEngine;

public interface ILevel 
{
  string LevelDataPath { get; }
  LevelData LevelData { get; }

   MonoBehaviour LevelBehavior { get; }
}


