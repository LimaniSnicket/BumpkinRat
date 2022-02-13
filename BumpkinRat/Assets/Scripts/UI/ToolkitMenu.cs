using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ToolkitMenu : MonoBehaviour
{
    [SerializeField]
    private List<ToolSelectorButton> activeToolSelectorButtons;

    [SerializeField]
    private List<CraftingAction> cacheRollout;

    [SerializeField]
    private GameObject toolSelectorButtonPrefab;

    private ToolSelectorButtonFactory toolSelectorButtonFactory;

    private RectTransform rectTransform;

    public event EventHandler ToolRollout;

    public bool ToolkitOpen => gameObject.activeSelf;

    public CraftingAction[] ActiveToolCraftingActions { get; private set; }

    private void Awake()
    {
        activeToolSelectorButtons = new List<ToolSelectorButton>();
        cacheRollout = new List<CraftingAction>();
        rectTransform = GetComponent<RectTransform>();

        toolSelectorButtonFactory = new ToolSelectorButtonFactory(toolSelectorButtonPrefab, rectTransform);
    }

    private void Start()
    {
        this.CreateToolSelectorButtons();
    }

    public void ToggleToolkit(GameObject obj)
    {
        bool isActive = obj.activeSelf;

        obj.SetActive(!isActive);

        if (!obj.activeSelf)
        {
            this.RolloutSelectedTools();
        }
    }

    public void ToggleToolSelectorButton(ToolSelectorButton btn)
    {
        activeToolSelectorButtons.HandleInstanceObjectInList(btn, btn.IsSelected);
    }

    private void SetActiveCraftingActionsFromButtons()
    {
        int length = activeToolSelectorButtons.Count;
        this.ActiveToolCraftingActions = new CraftingAction[length];

        for(int i = 0; i < activeToolSelectorButtons.Count; i++)
        {
            this.ActiveToolCraftingActions[i] = activeToolSelectorButtons[i].action;
        }
    }

    private void CacheActive()
    {
        if (this.ActiveToolCraftingActions.ValidArray())
        {
            this.cacheRollout = new List<CraftingAction>(this.ActiveToolCraftingActions);
        }
    }

    private void CreateToolSelectorButtons()
    {
        var actions = Enum.GetValues(typeof(CraftingAction));


        foreach (CraftingAction n in actions)
        {
            if (this.CraftingActionNeedsItem(n))
            {
                var btn = toolSelectorButtonFactory.CreateToolSelectorButton(n);
            }
        }
    }

    private bool CraftingActionNeedsItem(CraftingAction action)
    {
        if (action == CraftingAction.NONE || action == CraftingAction.ATTACH)
        {
            return false;
        }

        return true;
    }

    private void RolloutSelectedTools()
    {
        this.CacheActive();
        this.SetActiveCraftingActionsFromButtons();
        this.ToolRollout.BroadcastEvent(this);
    }


    private struct ToolSelectorButtonFactory
    {
        private readonly GameObject buttonPrefab;

        private readonly RectTransform rectTransform;

        public ToolSelectorButtonFactory(GameObject prefab, RectTransform rect)
        {
            this.buttonPrefab = prefab;
            this.rectTransform = rect;
        }

        public ToolSelectorButton CreateToolSelectorButton(CraftingAction tool)
        {
            var instantiate = Instantiate(buttonPrefab, this.rectTransform);

            var selector = instantiate.GetOrAddComponent<ToolSelectorButton>();
            selector.SetToolData(tool);

            return selector;
        }
    }
}

