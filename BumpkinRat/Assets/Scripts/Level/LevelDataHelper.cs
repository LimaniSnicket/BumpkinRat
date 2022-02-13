using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
public static class LevelDataHelper
{
    private static Dictionary<int, LevelData> levelData;

    private static LevelBase activeLevel;

    private const string LevelDataPath = "Assets/Resources/Databases/LevelData.json";

    public static LevelBase ActiveLevel => activeLevel;

    public static OverworldDialogue GetOverworldDialogueForNpcInActiveLevel(int id)
    {
        return activeLevel.GetOverworldDialogueForNpc(id);
    }

    public static void Test()
    {
        string json = File.ReadAllText(LevelDataPath);
        var data = JsonConvert.DeserializeObject(json);
        Debug.Log(data.ToString());
    }

    public static void SetActiveLevel(LevelBase level)
    {
        if (levelData.ContainsKey(level.Id))
        {
            activeLevel = level;
        }
    }

    public static LevelData GetLevelDataById(int id)
    {
        ValidateLevelDataStored();

        if(levelData.TryGetValue(id, out LevelData data))
        {
            return data;
        }

        throw new KeyNotFoundException();
    }

    private static void ValidateLevelDataStored()
    {
        if (!levelData.CollectionIsNotNullOrEmpty())
        {
            var allData = GetLevelDataFromJson();
            levelData = new Dictionary<int, LevelData>();
            
            for (int i = 0; i < allData.Length; i++)
            {
                var level = allData[i];
                levelData.Add(level.LevelId, level);
            }
        }
    }

    private static LevelData[] GetLevelDataFromJson()
    {
        string json = File.ReadAllText(LevelDataPath);
        var levelDataStorage = JsonConvert.DeserializeObject<LevelData[]>(json);

        foreach(var data in levelDataStorage)
        {
            Debug.Log(data.LevelName);
        }

        return levelDataStorage;
    }
}

[Serializable]
public struct DataStorage<T> 
{
    [SerializeField] private T[] data;

    public T[] Data => data ?? Array.Empty<T>();

    public override string ToString()
    {
        return data.CollectionIsNotNullOrEmpty()
            ? $"Data Storage of type {typeof(T)} count is: {data.Length}."
            : $"Data Storage of type {typeof(T)} is null or empty.";
    }
}
