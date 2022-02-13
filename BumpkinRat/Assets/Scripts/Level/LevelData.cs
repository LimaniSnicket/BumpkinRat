using System;
using UnityEngine;

[Serializable]
public struct LevelData
{
    public string LevelName { get; set; }
    public int LevelId { get; set; }
    public OverworldDialogueStorage[] GenericLevelDialogue { get; set; }

    // Drop format = {Id}x{Amount}
    public string[] DropOnStart { get; set; }

    public OrderDetails[] OrdersInLevel { get; set; }

    //todo Customer Entities, Dialogue Paths, etc

    public static LevelData Default => new LevelData()
    {
        LevelName = "Default Level Name",
        LevelId = -1
    };
}

[Serializable]
public struct OverworldDialogueStorage
{
    [SerializeField] internal int npcId;
    public OverworldDialogue dialogue;
}

