using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueRunner : MonoBehaviour
{
    private static DialogueRunner dr;
    public TextMeshPro textmeshDisplay;
    public Dialogue activeDialogue;

    private void OnEnable()
    {
        if(dr == null) { dr = this; } else { Destroy(gameObject); }
    }

    public void GenerateDialogue(TextAsset txt)
    {
        dr.activeDialogue = new Dialogue();
    }
}
