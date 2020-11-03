using System;
using System.Collections;
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

    public static Dictionary<int, NpcDatabaseEntry> GetNpcEntryLookup()
    {
        if(npcEntryLookup == null)
        {
            npcEntryLookup = npcData.npcEntries.ToDictionary(n => n.NpcId.GetValueOrDefault(-1));
        }

        return npcEntryLookup;
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
    [SerializeField] string conversationDataPath;

    CustomerDialogueStorage dialogueStorage;

    public int? NpcId => npcId;
    public string NpcName => npcName;

    public CustomerDialogue GetCustomerDialogue(int levelId, int dialogueId)
    {
        try
        {
            return GetStoredDialogueStorage().GetCustomerDialogue(levelId, dialogueId);

        } catch (NullReferenceException)
        {
            return new CustomerDialogue();
        }
    }

    public CustomerDialogueStorage GetStoredDialogueStorage()
    {
        Debug.Log(ToString());
        if (dialogueStorage == null && !string.IsNullOrWhiteSpace(conversationDataPath))
        {
            dialogueStorage = conversationDataPath.InitializeFromJSON<CustomerDialogueStorage>();
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
public class CustomerDialogueStorage
{
    [SerializeField] CustomerDialogue[] associatedDialogue;

    Dictionary<int, Dictionary<int, CustomerDialogue>> DialogueToIds;

    public CustomerDialogue GetCustomerDialogue(int level, int dialogueId)
    {
        Dictionary<int, CustomerDialogue> levelLookup = GetCustomerDialogueForLevel(level);
        if (levelLookup.CollectionIsNotNullOrEmpty())
        {
            CustomerDialogue dialogue;
            bool valid = levelLookup.TryGetValue(dialogueId, out dialogue);

            if (valid) return dialogue;

        }
        return new CustomerDialogue();
    }

    public Dictionary<int, CustomerDialogue> GetCustomerDialogueForLevel(int levelId)
    {
        if (DialogueToIds == null) InitializeDialogueLookup();
        return DialogueToIds.ContainsKey(levelId) ? DialogueToIds[levelId] : null;
    }
    
    void InitializeDialogueLookup()
    {
        DialogueToIds = new Dictionary<int, Dictionary<int, CustomerDialogue>>();
        if (associatedDialogue.CollectionIsNotNullOrEmpty())
        {
            foreach (var c in associatedDialogue)
            {
                if (!DialogueToIds.ContainsKey(c.levelId))
                {
                    DialogueToIds.Add(c.levelId, new Dictionary<int, CustomerDialogue>());
                }

                DialogueToIds[c.levelId].AddOrReplaceKeyValue(c.dialogueId, c);
            }
        }
    }
}
