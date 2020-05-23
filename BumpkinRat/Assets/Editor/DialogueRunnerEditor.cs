using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogueRunner))]
public class DialogueRunnerEditor : Editor
{
    public TextAsset activeTextAsset;
    SerializedObject so;
    SerializedProperty propTextAsset;

    private void OnEnable()
    {
        so = new SerializedObject(this);
        propTextAsset = so.FindProperty("activeTextAsset");
    }

    public override void OnInspectorGUI()
    {
        DialogueRunner dr = (DialogueRunner) target;
        base.OnInspectorGUI();
    }
}
