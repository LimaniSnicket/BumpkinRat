﻿using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public static class GenericExtensions
{
    public static bool ValidList<T>(this List<T> check)
    {
        if(check == null || check.Count <= 0) { return false; }
        return true;
    }

    public static T InitializeFromJSON<T>(this string path)
    {
        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<T>(json);
    }
}

public static class MathfX
{
    const float TAU = 6.28318530718f;
    public static float PercentOf(this float val, float outOf)
    {
        if (outOf != 0)
        {
            return (val * 100) / outOf;
        }
        return 0;
    }

    public static Vector3 Swizzle(this Vector3 v, Grid.CellSwizzle swizz)
    {
        return Grid.Swizzle(swizz, v);
    }

    public static Vector3 Swizzle(this Vector2 v, Grid.CellSwizzle swizz)
    {
        Vector3 vect = v;
        return vect.Swizzle(swizz);
    }

    public static Vector2 PercentOfVector2(this Vector2 val, Vector2 outOf)
    {
        return new Vector2(val.x.PercentOf(outOf.x), val.y.PercentOf(outOf.y));
    }

    public static float PulseSineFloat(float minValue, float modifier, float freq, float offset, float amplitude)
    {
        float pulseValue = (amplitude * Mathf.Sin(Time.time * TAU* freq)) + offset;
        return minValue + (pulseValue * modifier);
    }

    public static float PulseSineFloat(float modifier, float freq, float offset, float amplitude)
    {
        float pulseValue = (amplitude * Mathf.Sin(Time.time * TAU * freq)) + offset;
        return (pulseValue * modifier);
    }

    public static bool Squeeze(this Vector3 c, Vector3 comp, float threshold = 0.001f)
    {
        return Mathf.Abs(Vector3.Distance(c, comp)) <= threshold;
    }
}

public struct Vect2Delta: IDelta<Vector2>
{
    public Vector2 current { get; set; }
    public Vector2 previous { get; set; }
    public Vector2 GetDelta(Vector2 tracking)
    {
        current = tracking;
        Vector2 delta = current - previous;
        previous = current;
        return delta;
    }
}

public interface IDelta<T>
{
    T current { get; set; }
    T previous { get; set; }
    T GetDelta(T tracking);
}

public static class CraftX
{
    public static Item GetItem(this string id)
    {
        return DatabaseContainer.gameData.GetItem(id);
    }

    public static Item GetItem(this string id, Inventory i)
    {
        Item it = id.GetItem();
        return i.CheckQuantity(it, 1) ? it : null;
    }

   public static string ToID(this string display)
    {
        StringBuilder sb = new StringBuilder(display.ToLowerInvariant());
        sb.Replace(' ', '_');
        return sb.ToString();
    }

    public static int CompareItem(this Item i, Item other, bool byID = true, bool byValue = false)
    {
        if(byID && !byValue) { return i.CompareItemID(other); }
        if(!byID && byValue) { return i.CompareItemValue(other); }
        if(byID && byValue) { return i.CompareItemID(other) + i.CompareItemValue(other); }
        return 1;
    }

    public static int CompareItemID(this Item i, Item other)
    {
        if(other == null) { return 1; }
        return string.Compare(i.ID, other.ID, StringComparison.OrdinalIgnoreCase);
    }

    public static int CompareItemValue(this Item i, Item other)
    {
        if(other == null) { return 1; }
        return Mathf.Abs(i.value - other.value);
    }

}


