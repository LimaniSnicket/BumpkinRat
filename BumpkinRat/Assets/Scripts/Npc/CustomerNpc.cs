using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class CustomerNpc : MonoBehaviour
{

    const string prefabPath = "Assets/Prefabs/npc prefabs/Customer.prefab";
    static GameObject cachedPrefab;
    static bool validCache => cachedPrefab != null;

    MeshRenderer meshFilter;

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
        meshFilter = GetComponent<MeshRenderer>();
    }


    public static CustomerNpc GenerateCustomerNpc(NpcDatabaseEntry npc)
    {
        GameObject toInstantiate = validCache ? cachedPrefab : AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        CustomerNpc instantiatedCustomer = Instantiate(toInstantiate).GetOrAddComponent<CustomerNpc>();
        instantiatedCustomer.SetCustomerNpcAppearence(npc.TexturePath);
        return instantiatedCustomer;
    }

    void SetCustomerNpcAppearence(string texturePath)
    {
        if (!string.IsNullOrWhiteSpace(texturePath))
        {
            GetPropertyBlock.SetTexture(npcTexture, AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath));
            meshFilter.SetPropertyBlock(GetPropertyBlock);
        }
    }

}
