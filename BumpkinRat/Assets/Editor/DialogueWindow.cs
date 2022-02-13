using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class DialogueWindow : EditorWindow
{
    [MenuItem("Tools/Dialogue Editor")]
    public static void OpenWindow() => GetWindow<DialogueWindow>();

    public List<string> generatedLines;
    public TextAsset assetBase;

    public string jsonPath;

    public NpcDialogueStorage customerDialogue;

    SerializedObject so;
    SerializedProperty propGeneratedLines;

    SerializedProperty dialogue;

    private void OnEnable()
    {
        so = new SerializedObject(this);
        propGeneratedLines = so.FindProperty("generatedLines");

        dialogue = so.FindProperty("customerDialogue");
    }

    private void OnGUI()
    {
        so.Update();
        assetBase = (TextAsset)EditorGUILayout.ObjectField(assetBase, typeof(TextAsset));
        jsonPath = EditorGUILayout.TextField(jsonPath);
        if(GUILayout.Button("Generate Lines") && assetBase != null)
        {
            generatedLines = new List<string>(assetBase.GetStringArray());
        }

        if(GUILayout.Button("Get Customer Dialogue"))
        {
            customerDialogue = !string.IsNullOrEmpty(jsonPath) ? jsonPath.InitializeFromJSON<NpcDialogueStorage>() : null;
        }

        EditorGUILayout.PropertyField(propGeneratedLines, true);

        EditorGUILayout.PropertyField(dialogue, true);

        so.ApplyModifiedProperties();
    }
}