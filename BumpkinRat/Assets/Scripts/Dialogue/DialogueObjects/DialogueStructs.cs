using System;

[Serializable]
public struct DialogueResponse
{
    public string[] displayDialogue;

    public int qualityThreshold;
    public static DialogueResponse Null => new DialogueResponse 
    { 
        qualityThreshold = -1, 
        displayDialogue = Array.Empty<string>() ,
        isNull = true
    };
    public bool IsNull => isNull;

    private bool isNull;
}

[Serializable]
public struct PlayerResponse
{
    public int responseLevel;
    public string displayDialogue;
}