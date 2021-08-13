using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Items
{
    public class ItemDataManager : MonoBehaviour
    {
        [SerializeField]
        private List<Item> itemData;

        private const string ItemDataPath = "Assets/Resources/Databases/ItemList.json";

        private const string PrefabFolder = "Assets/Prefabs/Item Prefabs/";

        private static ItemDataManager itemDataManager;

        private static Dictionary<int, Item> standardItems;

        private static Dictionary<int, Recipe> itemRecipes;

        private static Dictionary<int, GameObject> itemGameObjectCache;

        private static Dictionary<string, Sprite> itemSpriteSheet;

        private static ItemObjectUiFactory itemObjectUiFactory;

        [Serializable]
        public struct ItemListWrapper
        {
            public List<Item> items;
        }

        private void Awake()
        {
            InitializeItemDataManager();
        }

        private void InitializeItemDataManager()
        {
            if (itemDataManager == null)
            {
                itemDataManager = this;
                itemGameObjectCache = new Dictionary<int, GameObject>();
                itemObjectUiFactory = new ItemObjectUiFactory();
                InitializeItemData(ItemDataPath);
                SetSpriteSheet();
            }
            else
            {
                Destroy(this);
            }
        }

        private static void SetSpriteSheet()
        {
            try
            {
                itemSpriteSheet = Resources.LoadAll<Sprite>("Sprites/ItemSprites").ToDictionary(s => s.name);
            }
            catch (NullReferenceException)
            {
                Debug.Log("Problem finding Spritesheet");
            }
        }

        private void InitializeItemData(string path)
        {
            var itemDataWrapper = path.InitializeFromJSON<ItemListWrapper>();
            itemData = itemDataWrapper.items;

            standardItems = new Dictionary<int, Item>();
            itemRecipes = new Dictionary<int, Recipe>();

            foreach(var item in itemData)
            {
                standardItems.Add(item.itemId, item);

                if (item.IsCraftable)
                {
                    itemRecipes.Add(item.itemId, item.craftingRecipe[0]);
                }
            }
        }

        public static Item GetItemById(int id)
        {
            if (standardItems.ContainsKey(id))
            {
                return standardItems[id];
            }

            return new Item { itemId = id, itemName = $"invalid_item_{id}", value = -1 };
        }

        public static Recipe GetRecipeForItem(int itemId)
        {
            Item item = GetItemById(itemId);

            if (item.craftable)
            {
                return item.craftingRecipe[0];
            }

            return null;
        }

        public static IEnumerable<Recipe> GetRecipesCraftableWith(Dictionary<int, int> availableIngredients)
        {
            return itemRecipes.Values.Where(r => r.Craftable(availableIngredients));
        }

        public static Sprite GetDisplaySprite(string itemName)
        {
            return itemSpriteSheet[itemName];
        }

        private static bool ExistsInCache(int id)
        {
            return itemGameObjectCache.ContainsKey(id);
        }

        public static ItemObjectUiElement CreateItemObjectUiElement(Transform parent, int itemId)
        {
            var item = GetItemById(itemId);

            var uiElement = itemObjectUiFactory.CreateItemObjectUiElement(parent, item);

            uiElement.SetParent(parent);

            return uiElement;
        }

        public static ItemObjectWorldElement SpawnFromPrefabPath(Item item)
        {           
            try
            {
                int id = item.itemId;

                if (ExistsInCache(id))
                {
                    Debug.Log("Spawning from cache!");
                    ItemObjectWorldElement i = CreateItemObjectBehavior(id, (itemGameObjectCache[id]));
                    return i;

                }

                GameObject gameObject = (GameObject)AssetDatabase.LoadAssetAtPath(item.prefabName, typeof(GameObject));

                ItemObjectWorldElement itemObj = CreateItemObjectBehavior(id, gameObject);

                itemGameObjectCache.Add(id, gameObject);

                return itemObj;

            }
            catch (ArgumentException)
            {
                return new GameObject("Invalid_Item_Object", typeof(ItemObjectWorldElement)).GetComponent<ItemObjectWorldElement>();
            }
        }

        private static ItemObjectWorldElement CreateItemObjectBehavior(int id, GameObject toClone)
        {
            ItemObjectWorldElement itemObj = GameObject.Instantiate(toClone).GetOrAddComponent<ItemObjectWorldElement>();

            itemObj.itemObject = new ItemObject(id);

            return itemObj;
        }
    }
}