using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpBehavior : MonoBehaviour
{
    private static Dictionary<string, WarpBehavior> WarpingLocations { get; set; }

    public string warpTag;

    public static bool WarpingActive { get; private set; }
    private static string tagOfWarping;
    public static string warpedTo;

    [Serializable]
    public struct WarpTo
    {
        public string tag;
        public string destination;

        bool Invalid(string s) => s == "null" || s == string.Empty || s == null;
        public bool Valid => !Invalid(tag) && !Invalid(destination);

        public void FillEmptyValues()
        {
            if(tag == null || tag == string.Empty)
            {
                tag = "null";
            }

            if (destination == null || destination == string.Empty)
            {
                destination = "null";
            }
        }
    }

    public WarpTo warpToInfo;

    void Awake()
    {
        AddToWarpingLocations();
        warpToInfo.FillEmptyValues();
    }

    public static bool IsWarping(string checking)
    {
        return WarpingActive && tagOfWarping.Equals(checking);
    }

    public static bool IsWarpingTarget(WarpBehavior warpBehavior, string targ)
    {
        return targ.Equals(warpBehavior.warpTag); 
    }

    void SetWarpingStatus(string warpTo, string warping = "")
    {
        tagOfWarping = warping.Equals("") ? "null" : warping;
        warpedTo = warpTo;
        WarpingActive = !tagOfWarping.Equals("null");
    }

    void OnTriggerEnter(Collider collider)
    {
        if (warpToInfo.Valid)
        {
            if (collider.CompareTag(warpToInfo.tag) && !WarpingActive)
            {
                Warp(collider.gameObject, warpToInfo.destination);
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (!warpToInfo.Valid)
        {
            return;
        }

        if (collider.CompareTag(tagOfWarping) && WarpingActive && IsWarpingTarget(this, warpedTo))
        {
            SetWarpingStatus("null");
        }
    }

    public static WarpBehavior GetWarpingLocation(string location)
    {
        if (!WarpingLocations.ContainsKey(location))
        {
            throw new WarpingLocationException(location);
        } else
        {
            return WarpingLocations[location];
        }
    }

    public static void ForceWarpToLocation(GameObject toWarp, string destination)
    {
        try
        {
            WarpBehavior warpingTo = GetWarpingLocation(destination);

            warpingTo.Warp(toWarp, destination);

        } catch (WarpingLocationException warpError)
        {
            Debug.Log(warpError.Message);
        }
    }

    public void Warp(GameObject toWarp, string warpingTo)
    {
        StartCoroutine(WarpToPosition(toWarp, warpingTo));
    }

    IEnumerator WarpToPosition(GameObject toWarp, string warpingTo)
    {
        Debug.Log("Warping to new Warp Location...");
        SetWarpingStatus(warpingTo, toWarp.tag);
        toWarp.CancelRigidBodyVelocity();
        yield return new WaitForSeconds(1);
        if (WarpingLocations.ContainsKey(warpingTo))
        {
            toWarp.transform.position = WarpingLocations[warpingTo].transform.position;
        }
    }

    void AddToWarpingLocations()
    {
        if (WarpingLocations == null)
        {
            WarpingLocations = new Dictionary<string, WarpBehavior>();
        }
        if (!WarpingLocations.ContainsKey(warpTag) && warpTag != "")
        {
            WarpingLocations.Add(warpTag, this);
        }else
        {
            if (!WarpingLocations.ContainsKey(name))
            {
                WarpingLocations.Add(name, this);
                warpTag = name;
            }
        }
    }
    
}

public interface IWarpTo
{
    void OnWarpBegin();
    void OnWarpEnd();
}

public class WarpingLocationException: Exception
{
    public WarpingLocationException() { }

    public WarpingLocationException(string location) : 
        base($"Location Tag {location} does not exist in Warping Location Lookup.")
    {

    } 
}
