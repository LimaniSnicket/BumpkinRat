using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class PbnMapperWindow : EditorWindow
{
    [MenuItem("Tools/Pbn Mapper")]
    public static void OpenPbnMapperWindow()
    {
        GetWindow<PbnMapperWindow>("Pbn Mapper Window");
    }

    SerializedObject so;
    Sprite colorMapper;
    List<ColorMap> mappedColors;
    int segments;
    string path;
    int id;
    const string jsonPath = "Assets/Resources/PaintByNumbers/PaintData.json";

    private void OnEnable()
    {
        so = new SerializedObject(this);
        mappedColors = new List<ColorMap>();
    }

    private void OnGUI()
    {
        so.Update();
        using(new GUILayout.VerticalScope())
        {
            colorMapper = (Sprite)EditorGUILayout.ObjectField(colorMapper, typeof(Sprite), true);
            EditorGUILayout.LabelField("Segment Count");

            segments = Math.Max(EditorGUILayout.IntField(segments), 1);

            if (GUILayout.Button("Map Colors"))
            {
                SpriteToColorString(colorMapper, segments);
            }

            if (GUILayout.Button("Filter JSON for Duplicates"))
            {
                FilterJSON();
            }

            EditorGUILayout.LabelField("Prefab Path");
            path = GUILayout.TextField(path);
            EditorGUILayout.LabelField("PBN Id");
            id = EditorGUILayout.IntField(id);

            if (mappedColors.CollectionIsNotNullOrEmpty())
            {
                foreach(var color in mappedColors)
                {
                    color.Paint();
                }

                if(GUILayout.Button("Append to JSON"))
                {
                    AppendEntry();
                }
            }
        }
    }

    void SpriteToColorString(Sprite s, int colorCount)
    {
        mappedColors.Clear();
        Texture2D t = s.CreateTexture2D();
        List<string> colors = ColorX.GetColorsHexesFromStrip(t, colorCount);
        mappedColors = colors.Select(c => new ColorMap() { color = c.ToColor(), colorString = c}).ToList();
    }

    void AppendEntry()
    {
        PBNMap appending = GetAsMap();

        string data = File.ReadAllText(jsonPath);
        List<PBNMap> existing = JsonConvert.DeserializeObject<List<PBNMap>>(data);

        existing.Add(appending);

        string json = JsonConvert.SerializeObject(existing);
        File.WriteAllText(jsonPath, json);
    }

    void FilterJSON()
    {
        string data = File.ReadAllText(jsonPath);
        List<PBNMap> existing = JsonConvert.DeserializeObject<List<PBNMap>>(data);
        List<int> cacheIds = new List<int>();

        List<PBNMap> filtered = new List<PBNMap>();

        foreach(var pbn in existing)
        {
            if (!cacheIds.Contains(pbn.pbnId))
            {
                filtered.Add(pbn);
            }
        }

        string json = JsonConvert.SerializeObject(filtered);
        File.WriteAllText(jsonPath, json);
        Debug.Log(json);
    }

    PBNMap GetAsMap()
    {
        return new PBNMap
        {
            prefabPath = path,
            pbnId = id,
            colorHexCodes = mappedColors.Where(c => c.includeInMap).Select(c => c.colorString).ToArray()
        };
    }

    [Serializable]
    class ColorMap
    {
        public Color color;
        public string colorString;
        public bool includeInMap = true;

        internal void Paint()
        {
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.ColorField(color);
                includeInMap = EditorGUILayout.Toggle(includeInMap);
            }
        }
    }
}


