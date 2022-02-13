using System;
using UnityEngine;
using UnityEngine.UI;

public class ToolSelectorButton : Button
{
    private ToolkitMenu toolkitMenu;

    [SerializeField]
    private bool isSelected;

    private ActivityColorSet activityColors;

    private Text buttonText;

    public bool IsSelected => this.isSelected;

    public Text ButtonText => this.buttonText ?? (this.buttonText = GetComponentInChildren<Text>());

    public string ToolName;

    public CraftingAction action;

    protected override void Start()
    {
        base.Start();

        this.buttonText = GetComponentInChildren<Text>();

        this.toolkitMenu = GetComponentInParent<ToolkitMenu>();

        this.transition = Transition.None;

        this.activityColors = ActivityColorSet.PlainWhite;

        this.onClick.AddListener(() => OnClickToggleSelection());

        this.SetSelectedStatus(false);
    }

    public void SetToolData(CraftingAction tool)
    {
        this.ToolName = tool.ToString();
        ButtonText.text = this.ToolName;
        this.action = tool;
    }

    private void OnClickToggleSelection()
    {
        bool setTo = !isSelected;
        this.SetSelectedStatus(setTo);
        this.toolkitMenu.ToggleToolSelectorButton(this);
    }

    private void SetSelectedStatus(bool setTo)
    {
        isSelected = setTo;
        activityColors.ApplyColorSetToImage(this.image, setTo);
    }
}
