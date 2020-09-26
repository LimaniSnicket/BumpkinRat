using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.Linq;

[Serializable]
public class IdentifiableNode
{
    public string windowTitle;
    [SerializeField] public string identifier;
    [SerializeField] public int value;

    [SerializeField] int row;
    [SerializeField] int col;
    [SerializeField] List<RecipeNode> recipeNode;
    [SerializeField]bool craftable;

    public IdentifiableNode()
    {
        identifier = "New Identifiable Node";
        windowTitle = identifier.ToID();
        recipeNode = new List<RecipeNode>();
        SubscribeToEvents();
    }

    public IdentifiableNode(Item i, GameData data)
    {
        identifier = i.DisplayName;
        windowTitle = i.itemName;
        value = i.value;
        craftable = data.HasRecipe(i);
        recipeNode = new List<RecipeNode>();
        if (craftable)
        {
            foreach(var r in data.GetRecipe(i.itemId).ingredients)
            {
                recipeNode.Add(new RecipeNode(r));
            }
        }
        SubscribeToEvents();
    }

    public (int, int) position => (row, col);

    public void SetNode((int, int) tuple)
    {
        row = tuple.Item1;
        col = tuple.Item2;
    }

    public void SetInfo(string id) { identifier = id; }

    public void DrawNode()
    {
        using (new GUILayout.VerticalScope("Box", GUILayout.MaxWidth(200)))
        {
            GUILayout.Label(identifier.ToID(), EditorStyles.boldLabel);
            identifier = EditorGUILayout.TextField(identifier);
        }
    }

    public void DrawNode(Action<IdentifiableNode> action, IdentifiableNode n, GameData data, NodeData nData)
    {
        GUI.color = Color.cyan;
        using (new GUILayout.VerticalScope("Box", GUILayout.MaxWidth(200)))
        {
            GUI.color = Color.black;
            using (new GUILayout.HorizontalScope("box", GUILayout.Height(25)))
            {
                GUI.color = Color.white;
                identifier = EditorGUILayout.TextField(identifier);
                GUI.color = Color.red;
                if (GUILayout.Button("X"))
                {
                    action(n);
                }
            }
            GUILayout.Label("Item ID: " + identifier.ToID(), EditorStyles.boldLabel);
            GUILayout.Label(string.Format("Row: {0}, Column: {1}", row, col));
            GUI.color = Color.white;
            using (new GUILayout.HorizontalScope("box"))
            {
                GUILayout.Label("Value: $");
                value = EditorGUILayout.IntField(value);
            }
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Craftable: ");
                craftable = EditorGUILayout.Toggle(craftable);
                using (new GUILayout.HorizontalScope())
                {
                    if (craftable)
                    {
                        if (GUILayout.Button("+")) { recipeNode.Add(new RecipeNode()); }
                        if (GUILayout.Button("-") && recipeNode.Count > 0) { recipeNode.RemoveAt(recipeNode.Count - 1); }
                    }
                }
            }
                if (craftable) { RunCraftable(data, nData); }
        }
    }

    void RunCraftable(GameData data, NodeData nData)
    {
        if (recipeNode.ValidList())
        {
                foreach (var node in recipeNode)
                {
                    node.DrawNode(data, nData);
                }
        }
    }

    void OnNodesCleared(object source, EventArgs args)
    {
        UnsubscribeFromEvents();
    }

    void OnAddToData(object source, CreateItemDataArgs args)
    {
        args.gameData.AddToGameData(ConvertToItem());
        if (craftable) { args.gameData.AddToGameData(ConvertToRecipe()); }
    }

    public void OnNodeDeleted(object source, NodeDeletedArgs args)
    {
        if (args.deletedNodePosition.Item1 == row)
        {
            if(args.deletedNodePosition.Item2 < col)
            {
                col--;
            }
        }
    }

    public Item ConvertToItem()
    {
        return new Item { itemName = identifier.ToID() };
    }

    public Recipe ConvertToRecipe()
    {
        List<RecipeIngredient> ings = recipeNode.Select(r => new RecipeIngredient(r.nodeData.Item1, r.nodeData.Item2)).ToList();
        return new Recipe { outputName = identifier.ToID(), ingredients = ings };
    }

    void SubscribeToEvents()
    {
        NodeData.NodeRemoved += OnNodeDeleted;
        NodeData.NodesCleared += OnNodesCleared;
        InventoryDatabaseWindow.AddToData += OnAddToData;
    }

    void UnsubscribeFromEvents()
    {
        NodeData.NodeRemoved -= OnNodeDeleted;
        NodeData.NodesCleared -= OnNodesCleared;
        InventoryDatabaseWindow.AddToData -= OnAddToData;
    }
}

[Serializable]
public class RecipeNode
{
    [SerializeField] string id;
    [SerializeField] int amount;
    public (string, int) nodeData => (id, amount);
    [SerializeField] int selectionIndex;

    public static event EventHandler<CreateItemArgs> CreateItem;

    public RecipeNode() { id = "New Ingredient"; amount = 0; }
    public RecipeNode(RecipeIngredient ingredient)
    {
        id = ingredient.ID;
        amount = ingredient.amount;
    }

    public void DrawNode(GameData data, NodeData nData)
    {
        bool valid = data.ValidItem(id.ToID()) || nData.ActiveNodeIDs.Contains(id);
        Color c = valid ? Color.black : Color.red;
        using (new GUILayout.VerticalScope())
        {
            using (new GUILayout.HorizontalScope(GUILayout.MaxWidth(175)))
            {
                if (GUILayout.Button("Fill"))
                {
                    id = nData.ActiveNodeIDs[selectionIndex];
                }
                selectionIndex = EditorGUILayout.Popup(selectionIndex, nData.ActiveNodeIDs.ToArray(), GUILayout.MinWidth(100));
                id = GUILayout.TextField(id, GUILayout.MinWidth(100));
                amount = EditorGUILayout.IntField(amount);
            }
            using (new GUILayout.HorizontalScope(GUILayout.MinWidth(100), GUILayout.MinHeight(25)))
            {
                if (!valid)
                {
                    GUIStyle error = new GUIStyle();
                    error.padding = new RectOffset(8, 5, 7, 10);
                    error.normal.textColor = c;
                    error.fontStyle = FontStyle.Bold;
                    GUILayout.Label("Item Not Valid!", error);
                   
                    if (GUILayout.Button("Create Item?"))
                    {
                        if(CreateItem != null)
                        {
                            CreateItem(this, new CreateItemArgs { displayName = id, ID = id.ToID()});
                        }
                    }
                }
            }
        }
    }
}


public class CreateItemArgs : EventArgs
{
    public string ID { get; set; }
    public string displayName { get; set; }
    
}

public class CreateItemDataArgs : EventArgs
{
   public GameData gameData { get; set; }
}
