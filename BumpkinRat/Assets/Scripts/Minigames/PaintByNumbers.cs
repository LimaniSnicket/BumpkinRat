using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PaintByNumbers : MonoBehaviour
{
    public Sprite interactionMap;

    public Color[] pbnColors;

    public Transform outline;

    public PBNMap testPbnMap;

    public bool PbnSequenceActive { get; private set; }

    public GameObject numberPrefab;

    public GraphicRaycaster GraphicsRaycast { get; private set; }
    PointerEventData pointerEventData;
    EventSystem eventSystem;

    public Sprite referenceSprite;

    public Color debug;

    private PBNValidator validator;

    [Range(0, 10)]
    public float meter;
    public Slider meterDisplay;

    private void Start()
    {
        GenerateCanvasColorCells(false);
        GraphicsRaycast = GetComponent<GraphicRaycaster>();
        eventSystem = FindObjectOfType<EventSystem>();
        meter = 10;
        validator = new PBNValidator();
        InitializePaintableColorCells(outline);
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            PbnSequenceActive = false;
        }


        if (!PBNColorCell.ReferenceInFocus && PbnSequenceActive)
        {
            meter -= Time.deltaTime;
        }

        debug = GetColorForNumber(ColorCell.ActiveNumber);
        meterDisplay.value = meter;
    }

    public void ActivateSequence()
    {
        PbnSequenceActive = true;
        RefillMeter();
    }

    public void RefillMeter()
    {
        meter = 10f;
    }

    public Color GetColorForNumber(int num)
    {
        try
        {
            return pbnColors[num];

        } catch (IndexOutOfRangeException)
        {
            return Color.white;
        }
    }

    void InitializePaintableColorCells(Transform outline)
    {
        int childCount = outline.childCount;
        for(int i = 0; i < childCount; i++)
        {
            GameObject child = outline.GetChild(i).gameObject;
            string last = child.name.Last().ToString();
            int num = int.Parse(last);
            PBNPaintableCell paintable = child.AddComponent<PBNPaintableCell>();
            paintable.InitializeColorCell(this, num);
        }
    }

    void GenerateCanvasColorCells(bool paintableUI)
    {
        if (pbnColors.CollectionIsNotNullOrEmpty())
        {
            int startX = -400;
            for (int i = 0; i < pbnColors.Length; i++)
            {
                PBNColorCell newCell = PBNColorCell.InstantiatePBNColorCell(i, this);

                Color set = pbnColors[i];
                newCell.SetColor(new Color(set.r, set.g, set.b, 1));

                newCell.transform.SetParent(transform);
                newCell.GetComponent<RectTransform>().localPosition = new Vector3(startX + 200 * i, 400);
            }
        }
        if (paintableUI)
        {
            PBNColorCell.GenerateGenericPBNColorCellRow(10, this, Vector2.up * 300);

        }
    }

    public void CheckGraphicsRaycastFromMouse()
    {
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();

        GraphicsRaycast.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            Debug.Log(result.gameObject.name);
        }
    }
}

[Serializable]
public struct PBNMap
{
    public string[] colorHexCodes;
    public string prefabPath;
    public int pbnId;
}

public class PBNValidator
{
    private List<ColorCell> cells;

    public PBNValidator()
    {
        cells = new List<ColorCell>();
    }

    public void Add(ColorCell cell)
    {
        cells.Add(cell);
    }

    public float GetError()
    {
        if (cells.CollectionIsNotNullOrEmpty())
        {
            float totalCount = cells.Count;
            IEnumerable<ColorCell> incorrect = cells.Where(c => c.IsPaintedCorrectly);
            int error = incorrect.Count();
            return error == 0 ? 0 : error / totalCount;
        }

        return -1;
    }
} 