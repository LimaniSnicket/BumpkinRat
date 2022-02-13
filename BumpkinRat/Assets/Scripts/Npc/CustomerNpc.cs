using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class CustomerNpc : OverworldNpc
{
    private void Update()
    {
        transform.eulerAngles = CameraManager.GetEulersOfAxes("y");
    }

    public void SetCustomerAppearence(int npcId)
    {
        bool inCache = npcTextureCache.TryGetTexture(npcId, out Texture2D toSet);

        if (!inCache)
        {
            toSet = GetTextureForNpc(npcId);
            npcTextureCache.CacheTexture(npcId, toSet);
        }

        this.SetNpcTexture(toSet);
    }

    private Texture2D GetTextureForNpc(int npc)
    {
        string texturePath = NpcData.GetTexturePath(npc);
        return AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
    }
}

public struct CustomerNpcSpawner
{
    const string prefabPath = "Assets/Prefabs/npc prefabs/Customer.prefab";

    private static GameObject prefab;

    public static bool PrefabValid => prefab != null;

    public static CustomerNpc GetCustomerNpcPrefab()
    {
        if (!PrefabValid)
        {
            prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        }

        CustomerNpc instantiatedCustomer = GameObject.Instantiate(prefab).GetOrAddComponent<CustomerNpc>();
        return instantiatedCustomer;
    }
}

