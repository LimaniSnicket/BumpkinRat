using System;

[Serializable]
public class OverworldDialogueTracker : DialogueTracker
{
    string condition = "0-0-0";

    public OverworldDialogue dialogue;

    public void GetDialogueFromLevelData(int npcId)
    {
        var npcDialogue = LevelDataHelper.GetOverworldDialogueForNpcInActiveLevel(npcId);

        if(npcDialogue != null)
        {
            dialogue = npcDialogue;
        }
    }

    internal void Advance()
    {
        Quality++;
    }
}
