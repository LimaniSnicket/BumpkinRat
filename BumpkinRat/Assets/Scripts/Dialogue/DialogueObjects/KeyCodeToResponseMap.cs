using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct KeyCodeToResponseMap
{
    private readonly Dictionary<KeyCode, ResponseTier> keycodeMappedToResponseTier;

    public IEnumerable<KeyCode> KeyCodes => keycodeMappedToResponseTier.Keys;

    public KeyCodeToResponseMap(KeyCode low, KeyCode mid, KeyCode best)
    {
        keycodeMappedToResponseTier = new Dictionary<KeyCode, ResponseTier>() 
        {
            { low, ResponseTier.LOW },
            { mid, ResponseTier.MID },
            { best, ResponseTier.BEST }
        };
    }

    public ResponseTier GetResponseTierForKey(KeyCode key)
    {
        if (!keycodeMappedToResponseTier.ContainsKey(key))
        {
            throw new ArgumentException("Bad KeyCode provided!");
        }

        return keycodeMappedToResponseTier[key];
    }

    public float CalculateDistractionAmount(KeyCode pressed)
    {
        if (keycodeMappedToResponseTier.ContainsKey(pressed))
        {
            return (int)keycodeMappedToResponseTier[pressed] * 0.5f + 1;
        }

        return 0;
    }
}
