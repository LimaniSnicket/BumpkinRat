using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Text;
using System.Linq;

public static class GenericX
{
    public static T InitializeStaticInstance<T>(this T instance, T staticVar)
    {
        if(staticVar == null)
        {
            return instance;
        }

        return staticVar;
    }
    public static bool ValidList<T>(this List<T> check)
    {
        return !(check == null || check.Count <= 0);
    }

    public static bool CollectionIsNotNullOrEmpty<T>(this IEnumerable<T> collection)
    {
        return collection != null && collection.Count() > 0;
    }

    public static bool CollectionCountEquals<T>(this IEnumerable<T> collection, int count)
    {
        if (!collection.CollectionIsNotNullOrEmpty())
        {
            return false;
        }

        return collection.Count() == count;
    }

    public static bool ValidArray<T>(this T[] check)
    {
        if (check == null || check.Length <= 0) { return false; }
        return true;
    }

    public static void HandleInstanceObjectInList<T>(this List<T> list, T instance, bool add)
    {
        if(list != null) {
            if (add)
            {
                if (!list.Contains(instance)) 
                { 
                    list.Add(instance); 
                }
            } else
            {
                if (list.Contains(instance)) { 
                    list.Remove(instance); 
                }
            }
        }
    }

    public static T GetOrAddComponent<T>(this GameObject gameObject) where T: Component
    {
        try
        {
            return gameObject.GetComponent<T>();
        }
        catch (NullReferenceException)
        {
            return gameObject.AddComponent<T>();
        }
    }

    public static T GetOrAddComponentInChildren<T>(this GameObject gameObject, int childIndex = 0) where T: Component
    {
        try
        {
            T child = gameObject.GetComponentInChildren<T>();
            if (!child.gameObject.activeSelf)
            {
                child.gameObject.SetActive(true);
            }
            return child;
        }
        catch (NullReferenceException)
        {
            GameObject addTo;
            if (gameObject.transform.childCount <= childIndex)
            {
                addTo = new GameObject($"{gameObject.name}_Child");
                addTo.transform.SetParent(gameObject.transform);
            } else
            {
                addTo = gameObject.transform.GetChild(childIndex).gameObject;
            }

            return addTo.GetOrAddComponent<T>();
        }
    }

    public static GameObject[] GetChildren(this Transform parent)
    {
        if(parent.childCount <= 0)
        {
            return Array.Empty<GameObject>();
        }

        int length = parent.childCount;
        GameObject[] arr = new GameObject[length];

        for (int i = 0; i < length; i++)
        {
            arr[i] = parent.GetChild(i).gameObject;
        }

        return arr;
    }

    public static T InitializeFromJSON<T>(this string path)
    {
        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<T>(json);
    }

    public static void DebugTuple<T, U>(this (T, U) tuple)
    {
        Debug.LogFormat("{0}, {1}", tuple.Item1.ToString(), tuple.Item2.ToString());
    }

    public static object GetPropertyOrField(this object t, string desired)
    {
        if (t.isField(desired))
        {
            FieldInfo fieldInfo = t.GetType().GetField(desired);
            return fieldInfo.GetValue(t);
        }

        if (t.isProperty(desired))
        {
            PropertyInfo property = t.GetType().GetProperty(desired);
            return property.GetValue(t);
        }

        return null;
    }

    static bool EvaluatePropertyOrField(this object t, string evaluate, string desired)
    {
        if (t.isField(evaluate))
        {
            FieldInfo fieldInfo = t.GetType().GetField(evaluate);
            return fieldInfo.GetValue(t).ToString() == desired; 
        }
        if (t.isProperty(evaluate))
        {
            PropertyInfo property = t.GetType().GetProperty(evaluate);
            return property.GetValue(t).ToString() == desired;
        }
        return false;
    }

    public static bool EvaluateValue(this object t, string evaluate, string desired, bool property = true)
    {
        bool nested = evaluate.IndexOf('.') > 0;
        if (!nested) { return t.EvaluatePropertyOrField(evaluate, desired); }
        try
        {

            if (property)
            {
                object temp = t;
                foreach (var eval in evaluate.Split('.').Select(s => temp.GetType().GetProperty(s)))
                {
                    temp = eval.GetValue(temp, null);
                }
                return temp.ToString() == desired;
            }
            else
            {
                object temp = t;
                foreach (var eval in evaluate.Split('.').Select(s => temp.GetType().GetField(s)))
                {
                    temp = eval.GetValue(temp);
                }
                return temp.ToString() == desired;
            }
        }
        catch (NullReferenceException)
        {
            Debug.LogWarningFormat("Warning! Trying to evaluate {0} as {1} resulted in a Null Reference Exception!", evaluate, desired);
            return false;
        }
    }

    public static void SetPropertyOrField(this object t, string toChange, string setValue)
    {
        if (t.isProperty(toChange)) {
            PropertyInfo propInfo = t.GetType().GetProperty(toChange);
            Type targetType = propInfo.PropertyType;
            var set = Convert.ChangeType(setValue, targetType);
            propInfo.SetValue(t, set);
        } else if (t.isField(toChange))
        {
            FieldInfo fieldInfo = t.GetType().GetField(toChange);
            Type targetType = fieldInfo.FieldType;
            var set = Convert.ChangeType(setValue, targetType);
            fieldInfo.SetValue(t, set);
        } else
        {
            Debug.LogWarningFormat("Couldn't set value, {0} isn't a valid Property or Field.", toChange);
        }
    }

    public static void SetValue(this object t, string toChange, string set, bool property = true)
    {
        bool nested = toChange.IndexOf('.') > 0;
        if (!nested) { t.SetPropertyOrField(toChange, set); return; }
        string[] bits = toChange.Split('.');
        if (property)
        {
            for (int i = 0; i < bits.Length - 1; i++)
            {
                PropertyInfo propertyToGet = t.GetType().GetProperty(bits[i]);
                t = propertyToGet.GetValue(t, null);
            }
            PropertyInfo propertyToSet = t.GetType().GetProperty(bits.Last());
            propertyToSet.SetValue(t, set, null);

        }
        else
        {
            for (int i = 0; i < bits.Length-1; i++)
            {
                FieldInfo fieldToGet = t.GetType().GetField(bits[i]);
                t = fieldToGet.GetValue(t);
            }
            FieldInfo fieldToSet = t.GetType().GetField(bits.Last());
            fieldToSet.SetValue(t, set);
        }

    }

    public static bool isProperty<T>(this T t, string p) => t.GetType().GetProperty(p) != null;

    public static bool isField<T>(this T t, string f) => t.GetType().GetField(f) != null;

    public static List<int> IndicesOf(this string s, char search)
    {
        List<int> indices = new List<int>();
        int start = 0;
        while(start >= 0)
        {
            start = s.IndexOf(search, start);
            indices.Add(start);
            start += 1;
        }
        return indices;
    }

    public static string GetCapitalizedString(this string toCap)
    {
        StringBuilder builder = new StringBuilder(toCap);

        string startingChar = toCap.Substring(0, 1);
        string replacing = startingChar.ToUpper();

        builder.Replace(startingChar, replacing, 0, 1);

        return builder.ToString();
    }

    public static void Increment<T>(this Dictionary<T, int> dict, T key)
    {
        if (!dict.ContainsKey(key))
        {
            dict.Add(key, 1);
        } else
        {
            dict[key]++;
        }
    }

    public static int Decrement<T>(this Dictionary<T, int> dict, T key, bool removeLessThanEqualToZero = true)
    {
        int outValue = 0;

        if (dict.ContainsKey(key))
        {
            int newAmount = dict[key] - 1;
            if(newAmount <= 0 && removeLessThanEqualToZero)
            {
                dict.Remove(key);
            } else
            {
                dict[key] = newAmount;
                outValue = newAmount;
            }
        }

        return outValue;
    }

    public static void AddMany<T, U>(this Dictionary<T, U> dict, IEnumerable<KeyValuePair<T, U>> adding)
    {
        if (dict != null && adding.CollectionIsNotNullOrEmpty())
        {
            foreach(var a in adding)
            {
                dict.Add(a.Key, a.Value);
            }
        }
    }

    public static void FilterOut<T, U>(this Dictionary<T, U> dict, Func<T, bool> predicate)
    {
        try
        {
            if (dict.CollectionIsNotNullOrEmpty())
            {
                foreach (var kp in dict)
                {
                    if (predicate(kp.Key))
                    {
                        dict.Remove(kp.Key);
                    }
                }
            }
        }
        catch(InvalidOperationException)
        {

        }
    }

    public static void AddOrReplaceKeyValue<T, U>(this Dictionary<T, U> dict, T key, U value)
    {
        if(dict != null)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = value;
            } else
            {
                dict.Add(key, value);
            }
        }
    }

    public static void RemoveKeys<T, U, K>(this Dictionary<T, U> dict, Func<K, bool> predicate, Func<K, T> output, params K[] keys)
    {
        if (!dict.CollectionIsNotNullOrEmpty() || !keys.CollectionIsNotNullOrEmpty())
        {
            return;
        }

        for(int i = 0; i < keys.Length; i++)
        {
            if (predicate(keys[i])) {
            
            }
        }

     /*   foreach(T key in keys)
        {
            if (dict.ContainsKey(key))
            {
                dict.Remove(key);
            }
        }*/
    }

/*    public static void FilterOutRemoveListeners<TKey, TValue, T>(this Dictionary<TKey, TValue> dict, Func<TKey, bool> predicate, UnityEvent<T> eventCall, string callback)
    {
        if (dict.CollectionIsNotNullOrEmpty())
        {
            foreach(var kp in dict)
            {
                if (predicate(kp.Key))
                {
                }
            }
        }
    }*/

    public static void BroadcastEvent<T>(this EventHandler<T> handler, object source, T eventArgs) where T: EventArgs
    {
        if(handler != null)
        {
            handler(source, eventArgs);
        }
    }

    public static void BroadcastEvent(this EventHandler handler, object source)
    {
        if(handler != null)
        {
            handler(source, new EventArgs());
        }
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
        float pulseValue = (amplitude * Mathf.Sin(Time.time * TAU * freq));
        return (pulseValue * modifier) + offset;
    }

    public static Vector3 PulseVector3(float modifier, float freq, float offset, float amplitude)
    {
        return Vector3.one * PulseSineFloat(modifier, freq, offset, amplitude);
    }

    public static Vector3 PulseVector3(this Vector3 scale, float modifier, float freq, float offset, float amplitude)
    {
        return scale * PulseSineFloat(modifier, freq, offset, amplitude);
    }

    public static bool Squeeze(this Vector3 c, Vector3 comp, float threshold = 0.001f)
    {
        return Mathf.Abs(Vector3.Distance(c, comp)) <= threshold;
    }

    public static bool Squeeze(this float f, float comp, float threshold = 0.001f)
    {
        return Mathf.Abs(f - comp) <= threshold;
    }

    public static bool SqueezeBetween(this float f, float lowerBound, float upperBound)
    {
        return f >= lowerBound && f <= upperBound;
    }

    public static bool SqueezeBetween(this int f, int lowerBound, int upperBound)
    {
        return f >= lowerBound && f <= upperBound;
    }
}

public static class InputX
{
    public static Vector2 InputRawVect2 => new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    public static Vector3 InputRawVect3 => new Vector3(InputRawVect2.x, 0, InputRawVect2.y);

    public static bool interactionButton => Input.GetKeyDown(KeyCode.Space);
}

public static class VizX
{
    public static Color ColorByRange(this float col, Dictionary<float, Color> dict)
    {
        float f = Mathf.Floor(col);
        if (f < dict.ElementAt(0).Key) { return dict.ElementAt(0).Value; }
        if (f > dict.Last().Key) { return dict.Last().Value; }
        if (dict.ContainsKey(f)) { return dict[f]; }
        float a = 0; float b = 0; Color a_c = new Color(); Color b_c = new Color();
        for (int i = 0; i < dict.Count - 1; i++)
        {
            if (f.SqueezeBetween(dict.ElementAt(i).Key, dict.ElementAt(i + 1).Key))
            {
                a = dict.ElementAt(i).Key; a_c = dict.ElementAt(i).Value;
                b = dict.ElementAt(i + 1).Key; b_c = dict.ElementAt(i + 1).Value;
            }
        }

        float RangeThreshold = Mathf.Abs(b - a);
        float col_2_mult = f - a; float col_1_mult = b - f;
        return (a_c * col_1_mult + b_c * col_2_mult) / RangeThreshold;
    }

    public static bool OverlappingWorldSpace(this RectTransform rectA, RectTransform rectB)
    {
        Vector2 sizeBuffer = rectA.rect.size / 2;
        Vector2 pos = rectA.position;

        Vector2 lowBound = pos - sizeBuffer;
        Vector2 highBound = pos + sizeBuffer;

        if (rectB.localPosition.x < lowBound.x || rectB.position.y < lowBound.y)
        {
            return false;
        }

        return rectB.position.x < highBound.x && rectB.position.y < highBound.y;
    }

    public static bool OverlappingLocalSpace(this RectTransform rectA, RectTransform rectB)
    {
        Vector2 sizeBuffer = rectA.rect.size/2;
        Vector2 pos = rectA.localPosition;

        Vector2 lowBound = pos - sizeBuffer;
        Vector2 highBound = pos + sizeBuffer;

        if(rectB.localPosition.x < lowBound.x || rectB.localPosition.y < lowBound.y)
        {
            return false;
        }

        return rectB.localPosition.x < highBound.x && rectB.localPosition.y < highBound.y;
    }

    public static Texture2D CreateTexture2D(this Sprite s)
    {
        int w = (int)s.rect.width;// Mathf.Max(1024, (int)s.rect.width);
        int h = (int)s.rect.height;// Mathf.Max(1024, (int)s.rect.height);
        Texture2D t = new Texture2D(w, h);
        var pixels = s.texture.GetPixels((int)s.rect.x,
                                         (int)s.rect.y,
                                         (int)s.rect.width,
                                         (int)s.rect.height);
        t.SetPixels(pixels);
        t.Apply();
        return t;
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
    public static void InstantiateItemInWorld(this string itemName, Vector3 spawnPosition, int amnt = 1)
    {
        Collectable collect = GameDataManager.InstantiateItem(spawnPosition).GetComponent<Collectable>();
        collect.SetItemName(itemName);
        collect.amount = amnt;
    }

   public static string ToID(this string display)
    {
        StringBuilder sb = new StringBuilder(display.ToLowerInvariant());
        sb.Replace(' ', '_');
        return sb.ToString();
    }

    public static string ToDisplay(this string Id)
    {
        if(Id.Length <= 0) { return Id; }

        if (Id.Contains("_"))
        {
            StringBuilder builder = new StringBuilder(Id.Length);
            string[] segments = Id.Split('_').Select(s => s.GetCapitalizedString()).ToArray();
            
            foreach(string s in segments)
            {
                builder.Append($"{s} ");
            }

            return builder.ToString();

        } else {
            return Id.GetCapitalizedString();
        }

    }

    public static bool Plantable(this Identifiable i)
    {
        return i.IdentifiableName.Contains("_seed");
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
        return string.Compare(i.itemName, other.itemName, StringComparison.OrdinalIgnoreCase);
    }

    public static int CompareItemValue(this Item i, Item other)
    {
        if(other == null) { return 1; }
        return Mathf.Abs(i.value - other.value);
    }

}


public static class PhysicsX
{
    const string axes = "xyzw";
    public static void CancelRigidBodyVelocity(this GameObject g)
    {
        try
        {
            g.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        catch (NullReferenceException)
        {

        }
    }

    static float GetAxis(this Vector3 vect, char a)
    {
        switch (a)
        {
            case 'x':
                return vect.x;
            case 'y':
                return vect.y;
            case 'z':
                return vect.z;
        }

        return 0;
    }

    static float GetAxis(this Vector4 vect, char a)
    {
        if(a == 'w')
        {
            return vect.w;
        }
        Vector3 v = vect;
        return v.GetAxis(a);
    }

    public static Vector3 GetAxesOfPosition(this Transform t, bool local, string axes)
    {
        Vector3 pos = local ? t.localPosition : t.position;
        return pos.GetAxesOfVector3(axes);
    }

    public static Vector3 GetAxesOfVector3(this Vector3 t, string a)
    {
        float[] getAxes = new float[3];
        int track = 0;
        for (int i = 0; i < getAxes.Length; i++)
        {
            if (track >= a.Length)
            {
                break;
            }
            //xyzw vs y
            char at = axes[i];

            if (a[track] == at)
            {
                getAxes[i] = t.GetAxis(at);
                track++;
            } else
            {
                getAxes[i] = 0;
            }
        }

        return new Vector3(getAxes[0], getAxes[1], getAxes[2]);

    }

    public static bool RaycastOnComponentOf<T>(this RaycastHit rh, out T raycastedComponent)
    {
        raycastedComponent = rh.transform.GetComponent<T>();
        return raycastedComponent != null;
    }

    public static void TurnToMainCamera(this Transform t)
    {
        Transform cam = Camera.main.transform;
        t.forward = cam.forward * -1;
    }
}

public static class CustomGravity
{
    public static Vector3 GetGravity(Vector3 position)
    {
        return Physics.gravity;
    }

    public static Vector3 GetUpAxis(Vector3 position)
    {
        return -Physics.gravity.normalized;
    }

    public static Vector3 GetGravity(Vector3 position, out Vector3 upAxis)
    {
        upAxis = -Physics.gravity.normalized;
        return Physics.gravity;
    }
}


