using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class CustomerNpc : OverworldNpc
{
    const string prefabPath = "Assets/Prefabs/npc prefabs/Customer.prefab";
    static GameObject cachedPrefab;
    static bool CacheExists => cachedPrefab != null;

    private void Update()
    {
        transform.eulerAngles = Camera.main.transform.eulerAngles.GetAxesOfVector3("y");
    }

    public static CustomerNpc GetCustomerNpc(int npc)
    {
        GameObject toInstantiate = GetPrefabToInstantiate();

        CustomerNpc instantiatedCustomer = Instantiate(toInstantiate).GetOrAddComponent<CustomerNpc>();

        Texture2D toSet;
        bool inCache = NpcTextureCache.TryGetTexture(npc, out toSet);

        if (!inCache)
        {
            toSet = GetTextureForNpc(npc);
            NpcTextureCache.CacheTexture(npc, toSet);
        }

        instantiatedCustomer.SetNpcAppearence(toSet);
        return instantiatedCustomer;
    }

    private static GameObject GetPrefabToInstantiate()
    {
        if (!CacheExists)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            cachedPrefab = prefab;
        }

        return cachedPrefab;
    }

    private static Texture2D GetTextureForNpc(int npc)
    {
        string texturePath = NpcData.GetTexturePath(npc);
        return AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
    }
}

