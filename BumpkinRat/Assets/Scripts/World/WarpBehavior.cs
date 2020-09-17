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
    }

    public WarpTo warpToInfo;

    void Awake()
    {
        AddToWarpingLocations();
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
        WarpingActive = tagOfWarping.Equals("null") ? false : true;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag(warpToInfo.tag) && !WarpingActive)
        {
            Warp(collider.gameObject, warpToInfo.destination);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag(tagOfWarping) && WarpingActive && IsWarpingTarget(this, warpedTo))
        {
            SetWarpingStatus("null");
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
