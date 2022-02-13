using UnityEditor;
using UnityEngine;

namespace Items
{
    public class ItemObjectUiFactory 
    {
        private const string UiElementPrefabPath = "Assets/Prefabs/UI Prefabs/Item Object UI.prefab";
        private const string FocusAreaUiPath = "Assets/Prefabs/UI Prefabs/Focus Area.prefab";

        private GameObject uiElementPrefab;
        private GameObject focusAreaPrefab;

        private int spawnCount;

        public ItemObjectUiFactory()
        {
            uiElementPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(UiElementPrefabPath);
            focusAreaPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(FocusAreaUiPath);
        }

        public  ItemObjectUiElement CreateItemObjectUiElement(Transform parent, Item item)
        {
            ItemObjectUiElement element = Object.Instantiate(uiElementPrefab).GetOrAddComponent<ItemObjectUiElement>();
            element.name = $"{item.DisplayName}: Obj #{spawnCount}";
            spawnCount++;

            SetFromItem(element, item);

            return element;
        }

        private void SetFromItem(ItemObjectUiElement itemObj, Item item)
        {
            itemObj.SetItemFromId(item.itemId);

            Sprite itemSprite = ItemDataManager.GetDisplaySprite(item.itemName);

            itemObj.SetItemObjectSprite(itemSprite);

            int focusAreas = item.FocusAreaCount;

            Transform transform = itemObj.transform;

            if (focusAreas > 0)
            {
                for (int i = 0; i < focusAreas; i++)
                {
                    FocusAreaUI uiElement = Object.Instantiate(focusAreaPrefab).GetOrAddComponent<FocusAreaUI>();

                    uiElement.transform.SetParent(transform);

                    FocusAreaUiDetails details = item.GetFocusAreaUiDetailsAtIndex(i);

                    uiElement.SetDetails(details);
                }
            }

            itemObj.RegisterFocusHandlerAreas();
        }
    }
}