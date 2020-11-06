using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class CustomerNpc : MonoBehaviour
{

    const string prefabPath = "Assets/Prefabs/npc prefabs/Customer.prefab";
    private static Dictionary<int, Texture2D> cacheNpcTextures;
    static GameObject cachedPrefab;
    static bool validCache => cachedPrefab != null;

    MeshRenderer Renderer => GetComponent<MeshRenderer>();

    MaterialPropertyBlock propertyBlock;

    MaterialPropertyBlock GetPropertyBlock
    {
        get
        {
            if(propertyBlock == null)
            {
                propertyBlock = new MaterialPropertyBlock();
            }

            return propertyBlock;
        }
    }

    static readonly int npcTexture = Shader.PropertyToID("_MainTex");


    private void Start()
    {
        if(cacheNpcTextures == null)
        {
            cacheNpcTextures = new Dictionary<int, Texture2D>();
        }
    }


    public static CustomerNpc GenerateCustomerNpc(NpcDatabaseEntry npc)
    {
        GameObject toInstantiate = validCache ? cachedPrefab : AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        CustomerNpc instantiatedCustomer = Instantiate(toInstantiate).GetOrAddComponent<CustomerNpc>();

        Texture2D toSet;

        bool inCache = ExistsInCache(npc, out toSet);

        if (!inCache)
        {
            toSet = AssetDatabase.LoadAssetAtPath<Texture2D>(npc.TexturePath);
            CacheTexture(npc.NpcId, toSet);
        }

        instantiatedCustomer.SetCustomerNpcAppearence(toSet);
        return instantiatedCustomer;
    }

    void SetCustomerNpcAppearence(Texture2D texture)
    {
        GetPropertyBlock.SetTexture(npcTexture, texture);
        Renderer.SetPropertyBlock(GetPropertyBlock);
    }

    static void CacheTexture(int? entry, Texture2D tex)
    {
        if (cacheNpcTextures == null)
        {
            cacheNpcTextures = new Dictionary<int, Texture2D>();
        }

        cacheNpcTextures.AddOrReplaceKeyValue(entry.Value, tex);
    }

    static bool ExistsInCache(NpcDatabaseEntry entry, out Texture2D texture)
    {
        texture = null;
        if (!cacheNpcTextures.CollectionIsNotNullOrEmpty()) return false;
        return cacheNpcTextures.TryGetValue(entry.NpcId.Value, out texture);
    }

}
