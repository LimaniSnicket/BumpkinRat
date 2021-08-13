using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameDataManager : MonoBehaviour
{
    public string itemDataPath, plantDataPath, npcDataPath;
    private static GameDataManager database;
    public PlayerData playerData;
    public static GameData gameData { get; private set; }

    public GameObject basicItemPrefab;
    static string placeholder = "Assets/Resources/PlayerData/data.json";

    private void Awake()
    {
        if (database == null) { database = this; } else { Destroy(this); }
        if(playerData == null)
        {
            playerData = new PlayerData { };
        }
        InitializeData();
        DontDestroyOnLoad(this);
    }
     void InitializeData()
    {
        gameData = itemDataPath.InitializeFromJSON<GameData>();
        gameData.SetIdToObjectMappings();

        NpcData.InitializeFromPath(npcDataPath);

        //gameData.plantData = plantDataPath.InitializeFromJSON<PlantDataStorage>();
    }

    public static GameObject InstantiateItem(Vector3 position)
    {
        GameObject createItem = Instantiate(database.basicItemPrefab);
        createItem.transform.position = position;
        return createItem;
    }

    public static void SetPlayerName(string name)
    {
        database.playerData.SetName(name);
    }

    private void OnApplicationQuit()
    {
      //  string sessionDataPath = Application.persistentDataPath;
        string saveSession = JsonUtility.ToJson(playerData, true);
        Debug.Log(saveSession);
        File.WriteAllText(placeholder, saveSession);
    }
}

[Serializable]
public struct PlantDataStorage
{
    public List<string> plantNames;
}

[Serializable]
public class PlayerData
{
    [SerializeField] string playerSetName;
    public string PlayerName => playerSetName;

    internal void SetName(string name)
    {
        playerSetName = name;
    }
}