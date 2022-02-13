using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IContainFocusArea 
{
    FocusAreaHandler FocusAreaHandler { get; set; }
    int ItemObjectId { get; }
}

public class FocusAreaHandler {
    Dictionary<int, IFocusArea> FocusAreaLookup { get; set; }

    public FocusAreaHandler() { }

    void SetFocusAreaDictionary()
    {
        if (FocusAreaLookup == null)
        {
            FocusAreaLookup = new Dictionary<int, IFocusArea>();
        }
    }

    internal void RegisterFocusAreasInChildren(Transform obj)
    {
        if(obj.childCount > 0)
        {
            for(int i = 0; i < obj.childCount; i++)
            {
                Transform child = obj.GetChild(i);
                try
                {
                    RegisterFocusArea(child.GetComponent<FocusAreaObject>());
                }
                catch (NullReferenceException) {
                    GameObject.Destroy(child.gameObject);
                }
            }
        }
    }

    internal void RegisterFocusAreasInChildrenWithExceptions<T>(Transform obj) where T: MonoBehaviour
    {
        if (obj.childCount > 0)
        {
            for (int i = 0; i < obj.childCount; i++)
            {
                Transform child = obj.GetChild(i);
                try
                {
                    RegisterFocusArea(child.GetComponent<FocusAreaObject>());
                }
                catch (NullReferenceException)
                {
                    if (!obj.GetComponent<T>())
                    {
                        GameObject.Destroy(child.gameObject);
                    }
                }
            }
        }
    }

    internal void RegisterFocusAreaUiInChildren(Transform obj, IContainFocusArea container)
    {
        if (obj.childCount > 0)
        {
            for (int i = 0; i < obj.childCount; i++)
            {
                Transform child = obj.GetChild(i);
                FocusAreaUI focus = child.GetComponent<FocusAreaUI>();
                focus.FocusArea.InitializeParentAndId(container, i);
                RegisterFocusArea(focus);
            }
        }
    }

    internal void RegisterFocusArea(IFocusArea area)
    {
        SetFocusAreaDictionary();

       /* int id = area.FocusArea.focusAreaId;

        while (FocusAreaLookup.ContainsKey(id))
        {
            id++;
        }

        area.FocusArea.focusAreaId = id;*/

        FocusAreaLookup.Add(area.FocusArea.focusAreaId, area);

    }

    internal int NumberOfInFocusAreas()
    {
        if (FocusAreaLookup == null) { return -1; }

        return FocusAreaLookup.Where(k => k.Value.FocusArea.IsFocus).Count();
    }
}

