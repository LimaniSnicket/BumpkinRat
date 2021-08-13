using System;

[Serializable]
public class OverworldDialogueTracker : DialogueTracker
{
    string condition = "0-0-0";

    public OverworldDialogue dialogue;

    public void GetDialogueFromLevelData(int npcId)
    {
        OverworldDialogue d;
        bool valid = LevelData.TryGetNpcDialogueForNpc(npcId, out d);
        if (valid)
        {
            dialogue = d;
        }
    }

    internal void Advance()
    {
        QualityIndex++;
    }

    internal string[] GetDisplayDialogue()
    {
        if (dialogue == null || QualityIndex < 0)
        {
            return Array.Empty<string>();
        }

        return GetDialogueResponseOfQuality(dialogue.DialogueResponses, QualityIndex).displayDialogue;
    }
}
