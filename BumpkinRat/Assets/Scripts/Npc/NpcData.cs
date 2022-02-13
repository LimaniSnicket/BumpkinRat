using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class NpcData 
{
    public static NpcData npcData;
    
    public static bool CanRead => npcEntryLookup.CollectionIsNotNullOrEmpty();

    [SerializeField] NpcDatabaseEntry[] npcEntries;

    static Dictionary<int, NpcDatabaseEntry> npcEntryLookup;

    public static void InitializeFromPath(string path)
    {
        if(npcData == null)
        {
            npcData = path.InitializeFromJSON<NpcData>();
        }

        InitializeNpcEntryLookup();
    }

    static void InitializeNpcEntryLookup()
    {
        if (npcEntryLookup == null)
        {
            npcEntryLookup = npcData.npcEntries.ToDictionary(n => n.NpcId.GetValueOrDefault(-1));
        }
    }

    public static string GetTexturePath(int id)
    {
        if (npcEntryLookup.CollectionIsNotNullOrEmpty())
        {
            if (npcEntryLookup.ContainsKey(id))
            {
                return npcEntryLookup[id].TexturePath;
            }
        }

        return string.Empty;
    }

    public static NpcDatabaseEntry GetDatabaseEntry(int npcId)
    {
        if (npcEntryLookup.CollectionIsNotNullOrEmpty())
        {
            if (npcEntryLookup.ContainsKey(npcId))
            {
                return npcEntryLookup[npcId];
            }
        }

        return NpcDatabaseEntry.NullEntry();
    }

    public override string ToString()
    {
        return string.Join(", ", npcData.npcEntries.Select(n => n.ToString()));
    }

}

[Serializable]
public struct NpcDatabaseEntry
{
    [SerializeField] int npcId;
    [SerializeField] string npcName;
    [SerializeField] bool fromGenericConversations;
    [SerializeField] string conversationDataPath;
    [SerializeField] string texturePath;

    private NpcDialogueStorage dialogueStorage;

    private const string GenericConversationPath = "Assets/Resources/{0}.json";

    public int? NpcId => npcId;
    public string NpcName => npcName;

    public string TexturePath => texturePath;

    public CustomerDialogue GetCustomerDialogue(int levelId, int dialogueId)
    {
        try
        {
            return GetStoredDialogueStorage(levelId).GetCustomerDialogue(dialogueId);

        } catch (NullReferenceException)
        {
            return new CustomerDialogue();
        }
    }

    public NpcDialogueStorage GetStoredDialogueStorage(int activeLevel)
    {
        if (dialogueStorage == null && !string.IsNullOrWhiteSpace(conversationDataPath))
        {
            // string path = string.Format(GenericConversationPath, conversationDataPath);
            dialogueStorage = conversationDataPath.InitializeFromJSON<NpcDialogueStorage>();
            dialogueStorage.InitializeForLevel(activeLevel);
        }
        return dialogueStorage;
    }


    public static NpcDatabaseEntry NullEntry()
    {
        return new NpcDatabaseEntry { npcId = -1, npcName = "Null" };
    }

    public override string ToString()
    {
        return $"[{npcName}: {NpcId}]: {conversationDataPath}";
    }
}

[Serializable]
public class NpcDialogueStorage
{
    [SerializeField] public CustomerDialogueForLevel[] dialogue;

    private Dictionary<int, CustomerDialogue> activeLevelDialogue;

    public void InitializeForLevel(int activeLevel)
    {
        var possibleDialogue = this.GetCustomerDialogueForLevelInternal(activeLevel);
        activeLevelDialogue = possibleDialogue.ToDictionary(d => d.dialogueId, d => d);
    }

    public CustomerDialogue GetCustomerDialogue(int dialogueId)
    {
        if (activeLevelDialogue.ContainsKey(dialogueId))
        {
            return activeLevelDialogue[dialogueId];
        }

        return null;
    }

    private CustomerDialogue[] GetCustomerDialogueForLevelInternal(int level)
    {
        foreach (var map in dialogue)
        {
            if (map.levelId == level)
            {
                return map.data;
            }
        }

        return Array.Empty<CustomerDialogue>();
    }
}

[Serializable]
public class CustomerDialogueForLevel
{
    public int levelId;
    public CustomerDialogue[] data;
}
