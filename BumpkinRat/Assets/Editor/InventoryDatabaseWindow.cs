using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
 

public class InventoryDatabaseWindow : EditorWindow
{
    [MenuItem("Tools/Item Database")]
    public static void OpenInventoryDatabaseWindow()
    {
        GetWindow<InventoryDatabaseWindow>("Item Database Editor");
        
    }

    //save node rows to a json file
    const string sessionDataPath = "Assets/Resources/Databases/itemEditorTest.json";

    [SerializeField] string jsonPath = "Assets/Resources/Databases/ItemData.json";
    [SerializeField] EditMode dataToEdit;
    [SerializeField]GameData SaveToJSON;
    public NodeData sessionData;


    bool showCurrentJSON, showPreviewJSON;
    [SerializeField] List<Item> itemsToAdd = new List<Item>();
    [Range(3, 10)]
    int rowCap;
    public int RowCapacity
    {
        get
        {
            return rowCap;
        }
        set
        {
            if (ColumnsChanged != null)
            {
                if (rowCap != value)
                {
                    ColumnsChanged(this, new ValueChangeArgs { changedValue = value, direction = (value - rowCap) < 0 });
                }
            }
            rowCap = value;
        }
    }


    public static event EventHandler<ValueChangeArgs> ColumnsChanged;
    public static event EventHandler<CreateItemDataArgs> AddToData;

    Vector2 scroll;
    SerializedObject so;

    private void OnEnable()
    {
        Selection.selectionChanged += Repaint;
        so = new SerializedObject(this);
        rowCap = Mathf.Clamp(rowCap, 3, 10);
        SaveToJSON = jsonPath.InitializeFromJSON<GameData>();
        SaveToJSON.InitializeLookupTables();
        sessionData = sessionDataPath.InitializeFromJSON<NodeData>();
        sessionData.rowCapacity = RowCapacity;
        sessionData.InitializeData();
        ColumnsChanged += sessionData.OnColumnChange;
        RecipeNode.CreateItem += OnCreateItem;
        if (itemsToAdd == null) { itemsToAdd = new List<Item>(); }
    }

    private void OnGUI()
    {
        so.Update();
        using (new GUILayout.VerticalScope())
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Item Database Path:");
                jsonPath = GUILayout.TextField("Assets/Resources/Databases/ItemData.json");
                GUILayout.Label("Editing:");
                dataToEdit = (EditMode)EditorGUILayout.EnumPopup(dataToEdit);
             

                
            }
            showCurrentJSON = EditorGUILayout.Foldout(showCurrentJSON, "Show Existing JSON");
            showPreviewJSON = EditorGUILayout.Foldout(showPreviewJSON, "Preview Node Data JSON");

            if (showCurrentJSON || showPreviewJSON)
            {
                scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Width(800), GUILayout.Height(500));
                string data = File.ReadAllText(jsonPath);
                if (showCurrentJSON) { GUILayout.TextArea(data); }
                else
                {
                    if (showPreviewJSON)
                    {
                        string preview = EditorJsonUtility.ToJson(SaveToJSON, true);
                        GUILayout.TextArea(preview);
                    }
                }
                EditorGUILayout.EndScrollView();
            }
           

            DrawHelpPanel();

            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Width(800), GUILayout.Height(500));
            Paint();
            EditorGUILayout.EndScrollView();
        }
        so.ApplyModifiedProperties();
    }

    void Paint()
    {
        if (sessionData != null)
        {
            sessionData.PaintSessionData(SaveToJSON);
        }
    }

    void DrawHelpPanel()
    {
        using (new GUILayout.VerticalScope("helpbox", GUILayout.MaxWidth(700)))
        {
            if (!sessionData.initialized) { GUILayout.Label("Session Data not Initialized"); }
            GUILayout.Label(dataToEdit.ToString()  + " Mode Node Tools", EditorStyles.boldLabel);
            if (dataToEdit.Equals(EditMode.Append))
            {
                AppendModeToolbar();
            }
           
        }
    }

    void AppendModeToolbar()
    {
        using (new GUILayout.HorizontalScope("helpbox", GUILayout.MaxWidth(685)))
        {
            using (new GUILayout.VerticalScope())
            {
                GUILayout.Label("Manage Nodes:", GUILayout.MaxWidth(100));
                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("+", GUILayout.MaxWidth(50)))
                    {
                        IdentifiableNode id = new IdentifiableNode();
                        sessionData.AddNode(id);
                    }
                    if (GUILayout.Button("-", GUILayout.MaxWidth(50)))
                    {
                        sessionData.RemoveAtLast();
                    }
                }
                if (GUILayout.Button("Clear Nodes"))
                {
                    foreach (NodeRow r in sessionData.Rows)
                    {
                        r.Clear();
                    }
                    sessionData.Clear();
                }
            }

            using (new GUILayout.VerticalScope())
            {
                GUILayout.Label("Columns per Row:");
                RowCapacity = EditorGUILayout.IntSlider(rowCap, 3, 10);
            }
            using (new GUILayout.VerticalScope())
            {
                using (new GUILayout.HorizontalScope())
                {

                    if (GUILayout.Button("Write To JSON"))
                    {
                        BroadcastAddData(SaveToJSON);
                        string saveItems = EditorJsonUtility.ToJson(SaveToJSON, true);
                        Debug.Log(saveItems);
                        File.WriteAllText("Assets/Resources/Databases/SampleItemDatabase.json", saveItems);
                    }
                }
                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Append From JSON"))
                    {
                        if (SaveToJSON == null) { SaveToJSON = jsonPath.InitializeFromJSON<GameData>(); }
                        if (SaveToJSON.GetItemData().ValidList())
                        {
                            foreach (Item i in SaveToJSON.GetItemData())
                            {
                                sessionData.AddNode(new IdentifiableNode(i, SaveToJSON));
                            }
                        }
                    }

                    if (GUILayout.Button("Update Game Data"))
                    {
                        BroadcastAddData(SaveToJSON);
                    }
                }
            }
        }
    }

    private void OnCreateItem(object sender, CreateItemArgs e)
    {
        IdentifiableNode created = new IdentifiableNode();
        created.SetInfo(e.displayName);
        sessionData.AddNode(created);
        sessionData.PrintAllNodes();
    }

    void BroadcastAddData(GameData data)
    {
        if (AddToData != null)
        {
            AddToData(this, new CreateItemDataArgs { gameData = data });
        }
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= Repaint;
        ColumnsChanged -= sessionData.OnColumnChange;
        string saveSession = EditorJsonUtility.ToJson(sessionData, true);
        File.WriteAllText(sessionDataPath, saveSession);
    }

}

public enum EditMode
{
   Append, Remove, EditExisting
}

[Serializable]
public class NodeData
{
    public int rowCapacity;
    public List<NodeRow> Rows;
    int? rowIndexNullable => Rows.Count - 1;
    public int rowIndex => rowIndexNullable.GetValueOrDefault(0);
    public bool initialized => Rows != null || Rows[0].nodesInRow != null;

    public static event EventHandler<NodeDeletedArgs> NodeRemoved;
    public static event EventHandler NodesCleared;

    public void InitializeData()
    {
        if (Rows.ValidList())
        {
            foreach(var r in Rows)
            {
                r.Initialize();
            }
        }
    }

    public IdentifiableNode GetNode(int row, int col)
    {
        try
        {
            return Rows[row].nodesInRow[col];
        }
        catch (IndexOutOfRangeException) { Debug.Log("Node you're looking for is out of range"); return new IdentifiableNode(); }
    }

    public void OnColumnChange(object source, ValueChangeArgs args)
    {
        AdjustRows(args);
    }

     void AdjustRows(ValueChangeArgs args)
    {
        rowCapacity = args.changedValue;
        if (!args.direction)
        {
            ShiftRows();
        } else
        {
            for(int i = 0; i < Rows.Count-1; i++)
            {
                if (!Rows[i].Overflowed(rowCapacity))
                {
                    break;
                } else
                {
                    Rows[i + 1].AppendToFront(Rows[i].GetOverflow(rowCapacity));
                }
            }
            if (Rows[rowIndex].Overflowed(rowCapacity))
            {
                AddNodeRow(Rows[rowIndex].GetOverflow(rowCapacity));
            }
        }
    }

    public void Clear()
    {
        if(NodesCleared != null) { NodesCleared(this, EventArgs.Empty); }
        Rows.Clear();
        Rows.Add(new NodeRow(0));
    }

    public void RemoveNode(IdentifiableNode r)
    {
        NodeRemoved -= r.OnNodeDeleted;
        Rows[r.position.Item1].Remove(r.position.Item2);
        if(NodeRemoved != null) { NodeRemoved(this, new NodeDeletedArgs {deletedNodePosition = r.position }); }
        ShiftRows();
    }

    public void RemoveNode(IdentifiableNode r, bool verify)
    {
        if (!verify)
        {
            RemoveNode(r);
        }
    }

    public void PaintSessionData(GameData data)
    {
        if (Rows.ValidList())
        {
            foreach(NodeRow r in Rows)
            {
                r.DrawRow(RemoveNode, data, this);
            }
        }
    }

    public void AddNode(IdentifiableNode id)
    {
        if (Rows.ValidList())
        {
            bool capped = Rows[rowIndex].AtCapacity(rowCapacity);
            AddNode(id, capped);
        }
    }

    void AddNodeRow(List<IdentifiableNode> nodes)
    {
        NodeRow nr = new NodeRow(rowIndex + 1);
        nr.nodesInRow = new List<IdentifiableNode>(nodes);
        nr.Initialize();
        Rows.Add(nr);
    }

     void AddNode(IdentifiableNode id, bool withRow)
    {
        if (Rows.ValidList())
        {
            if (withRow)
            {
                int n = rowIndex + 1; 
                NodeRow nr = new NodeRow(n);
                Rows.Add(nr);
            }
            Rows[rowIndex].Add(id);
        }
    }

    public void RemoveAtLast()
    {
        if (Rows[rowIndex].nodesInRow.ValidList())
        {
            Rows[rowIndex].Remove(Rows[rowIndex].nodesInRow.Count - 1);
        }
        ClearEmptyRows();
    }

    void ShiftRows()
    {
        if(Rows.Count > 0)
        {
            for(int i =1; i< Rows.Count; i++)
            {
                if (!Rows[i - 1].AtCapacity(rowCapacity) && Rows[i].nodesInRow.Count > 0)
                {
                    Rows[i - 1].Add(Rows[i].nodesInRow[0]);
                    Rows[i].Remove(0);
                }
            }
            ClearEmptyRows();
        }
    }

    void ClearEmptyRows()
    {
        if(rowIndex > 0)
        {
            if (!Rows[rowIndex].nodesInRow.ValidList())
            {
                Rows.RemoveAt(rowIndex);
                ClearEmptyRows();
            }
        } else
        {
            return;
        }
    }

    public List<Item> ConvertNodesToItem() 
    {
        List<Item> c = new List<Item>();
        if (Rows.Count > 0)
        {
           foreach(NodeRow r in Rows)
            {
                c.AddRange(r.GetItemsFromNodes());
            }
        }
        return c;
    }

    public List<IdentifiableNode> GetAllNodes()
    {
        List<IdentifiableNode> idNodes = new List<IdentifiableNode>();
        if(Rows.Count > 0)
        {
            foreach(NodeRow r in Rows)
            {
                if(r.nodesInRow.Count > 0) { idNodes.AddRange(r.nodesInRow); }
            }
        }
        return idNodes;
    }

    public List<string> ActiveNodeIDs
    {
        get
        {
            if (!Rows.ValidList()) { return new List<string>(); }
            return Rows.SelectMany(row => row.getNodeIDs).ToList();
        }
    }

    public void PrintAllNodes()
    {
        if (ActiveNodeIDs.ValidList())
        {
            foreach(string s in ActiveNodeIDs)
            {
                Debug.Log(s);
            }
        }
    }
}

[Serializable]
public class NodeRow
{
    public List<IdentifiableNode> nodesInRow;
    public int rowIndex;
    public NodeRow() { }
    public NodeRow(int r) { nodesInRow = new List<IdentifiableNode>(); rowIndex = r; }

    public List<string> getNodeIDs { get
        {
            if (!nodesInRow.ValidList()) { return new List<string>(); }
            var s = nodesInRow.Select(x => x.identifier).ToList();
                return s;
        } }

    public int rowTotal => nodesInRow.ValidList() ? nodesInRow.Count : -1;

    public bool AtCapacity(int cap)
    {
        return nodesInRow.ValidList() && nodesInRow.Count == cap;
    }

    public bool Overflowed(int cap)
    {
        return nodesInRow.ValidList() && nodesInRow.Count > cap;
    }

    public void Initialize()
    {
        for (int i =0; i< nodesInRow.Count; i++)
        {
            nodesInRow[i].SetNode((rowIndex, i));
        }
    }

    public void AppendToFront(List<IdentifiableNode> nodes)
    {
        for(int i =nodes.Count-1; i >-1; i--)
        {
            nodesInRow.Insert(0, nodes[i]);
            nodesInRow[0].SetNode((rowIndex, i));
        }
    }

    public List<IdentifiableNode> GetOverflow(int cap)
    {
        if (Overflowed(cap))
        {
            List<IdentifiableNode> overflow = new List<IdentifiableNode>();
            for (int i = cap; i < nodesInRow.Count; i++)
            {
                overflow.Add(nodesInRow[i]);
                nodesInRow.Remove(nodesInRow[i]);
            }
            return overflow;
        }
        else
        {
            return new List<IdentifiableNode>();
        }
    }

    public int GetColumn(IdentifiableNode id)
    {
        if (nodesInRow.Contains(id)) { return nodesInRow.IndexOf(id); }
        return -1;
    }

    public void SetNodeRow(int r) { rowIndex = r; }

    public List<Item> GetItemsFromNodes()
    {
        List<Item> i = new List<Item>();
        if (nodesInRow.ValidList())
        {
            foreach(var n in nodesInRow)
            {
                i.Add(n.ConvertToItem());
            }
        }
        return i;
    }

    public void DrawRow()
    {
        using (new GUILayout.HorizontalScope())
        {
            foreach (var n in nodesInRow)
            {
                n.DrawNode();
            }
        }
    }

    public void DrawRow(Action<IdentifiableNode> Method, GameData data, NodeData nData)
    {
        using (new GUILayout.HorizontalScope())
        {
            foreach (var n in nodesInRow)
            {
                n.DrawNode(Method, n, data, nData);
            }
        }
    }

    public void Add(IdentifiableNode id)
    {
        if (nodesInRow == null) { nodesInRow = new List<IdentifiableNode>(); }
        id.SetNode((rowIndex, nodesInRow.Count));
        nodesInRow.Add(id);
    }

    public void Clear()
    {
        nodesInRow.Clear();
    }

    public void Remove(int col)
    {
        if (nodesInRow != null) {
            if (col < nodesInRow.Count)
            {
                nodesInRow.RemoveAt(col);
            }
        }
    }
}


public class ValueChangeArgs : EventArgs
{
    public int changedValue { get; set; }
    public bool direction { get; set; }
}


public class NodeDeletedArgs: EventArgs
{
    public (int, int) deletedNodePosition { get; set; }
}