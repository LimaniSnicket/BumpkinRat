using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public struct LevelData
{
    static LevelData ActiveLevel;
    static Dictionary<int, OverworldDialogue> genericLevelDialogueLookup;

    [SerializeField] string levelName;
    [SerializeField] int levelId;
    [SerializeField] OverworldDialogueStorage[] genericLevelDialogue;

    public int LevelId => levelId;
    public string LevelName => levelName;

    public CustomerOrder[] ordersInLevel;

    //todo Customer Entities, Dialogue Paths, etc

    public static LevelData GetFromPath(string path)
    {
        if (!string.IsNullOrWhiteSpace(path))
        {
            try
            {
                LevelData data = path.InitializeFromJSON<LevelData>();
                ActiveLevel = data;
                genericLevelDialogueLookup = ActiveLevel.genericLevelDialogue.ToDictionary(k => k.npcId, k => k.genericDialogue);
                return data;
            }
            catch (NullReferenceException) { }
        }

        return new LevelData();
    }

    public static bool TryGetNpcDialogueForNpc(int npcId, out OverworldDialogue dialogue)
    {
        if (genericLevelDialogueLookup.CollectionIsNotNullOrEmpty())
        {
            bool tryGet = genericLevelDialogueLookup.TryGetValue(npcId, out dialogue);
            return tryGet;
        }
        dialogue = null;
        return false;
    }
}

[Serializable]
public struct OverworldDialogueStorage
{
    [SerializeField] internal int npcId;
    public OverworldDialogue genericDialogue;
}
