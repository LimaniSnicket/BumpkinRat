using System.Collections.Generic;
using UnityEngine;

public class NpcTextureCache 
{
    private readonly Dictionary<int, Texture2D> cachedNpcTextures;

    public NpcTextureCache()
    {
        cachedNpcTextures = new Dictionary<int, Texture2D>();
    }

    public void CacheTexture(int key, Texture2D value)
    {
        cachedNpcTextures.AddOrReplaceKeyValue(key, value);
    }

    public bool TryGetTexture(int entry, out Texture2D tex)
    {
        return cachedNpcTextures.TryGetValue(entry, out tex);
    }
}
