using System;
using UnityEngine;

[Serializable]
public class Plant: Identifiable
{
    [SerializeField] 
    private string name;
    private Vector3 pos;

    public string IdentifiableName => name;

    [SerializeField]
    private float[] position;

    public Plant(string s)
    {
        name = s;
    }

    public Plant(string s, Vector3 v)
    {
        name = s;
        pos = v;
        position = new float[] { v.x, v.y, v.z };
    }
}
