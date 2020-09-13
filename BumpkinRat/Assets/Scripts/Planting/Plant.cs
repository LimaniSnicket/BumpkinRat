using System;
using UnityEngine;

[Serializable]
public class Plant: Identifiable
{
    [SerializeField] string name;
    Vector3 Position;

    public string identifier => name;

    [SerializeField]float[] position;

    public Plant() { }

    public Plant(string s)
    {
        name = s;
    }

    public Plant(string s, Vector3 v)
    {
        name = s;
        Position = v;
        position = new float[] { v.x, v.y, v.z };
    }
}

[Serializable]
public class Seed : Identifiable {
    public string plantPointer;
    public string identifier => string.Format("{0}_seed", plantPointer);

    public Seed(string pointer)
    {
        plantPointer = pointer;
    }
}