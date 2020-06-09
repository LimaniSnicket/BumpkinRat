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

    SerializedObject so;
    SerializedProperty propGeneratedLines;

    private void OnEnable()
    {
        so = new SerializedObject(this);
        propGeneratedLines = so.FindProperty("generatedLines");
    }

    private void OnGUI()
    {
        so.Update();
        assetBase = (TextAsset)EditorGUILayout.ObjectField(assetBase, typeof(TextAsset));
        if(GUILayout.Button("Generate Lines") && assetBase != null)
        {
            generatedLines = new List<string>(assetBase.GetStringArray());
        }
        EditorGUILayout.PropertyField(propGeneratedLines, true);
        so.ApplyModifiedProperties();
    }
}