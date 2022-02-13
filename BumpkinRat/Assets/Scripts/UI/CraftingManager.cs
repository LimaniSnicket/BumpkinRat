using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using BumpkinRat.Crafting;
using Items;

public class CraftingManager : MonoBehaviour
{
    private static CraftingManager craftingUI;

    private static CraftingMenu craftingUiMenu;

    private static ItemCrafter itemCrafter;

    private static ConversationUi craftingConversationBehavior;

    private static ToolkitMenu toolkitMenu;

    private static Dictionary<FocusAreaObject, GameObject> focusAreaIndicators;

    public static float DistractionJitter = 0.25f;

    public static bool FocusedOnCrafting { get; private set; }

    public static bool CraftingUiBusy { get; private set; }

    #region Inspector Values

    public GameObject craftingButton;

    public GameObject craftingActionButtonParent;

    public GameObject progressDisplay;

    public GameObject orderDisplay;

    public GameObject focusAreaIndicatorPrefab;

    public Vector2[] itemObjectPositions;

    public List<CraftingActionWidget> craftingActionButtons;

    #endregion

    private CraftingOrderSnippet orderSnippet;

    private static CraftingUiElementFactory uiElementFactory;

    void Awake()
    {
        if (craftingUI == null)
        {
            craftingUI = this;
            focusAreaIndicators = new Dictionary<FocusAreaObject, GameObject>();
        }
        else
        {
            Destroy(this);
        }

        if (itemCrafter == null)
        {
            itemCrafter = new ItemCrafter();
        }

        craftingConversationBehavior = GetComponent<ConversationUi>();
        toolkitMenu = GetComponentInChildren<ToolkitMenu>();

        uiElementFactory = new CraftingUiElementFactory(this, toolkitMenu);
    }

    void Start()
    {
        craftingUiMenu = uiElementFactory.CreateCraftingMenu();

        orderSnippet = GetComponentInChildren<CraftingOrderSnippet>();

        // Not in conversation by default
        craftingConversationBehavior.enabled = false; 

        craftingUiMenu.TailoredUiEvent += OnCraftingMenuStatusChange;

        CustomerDialogueTracker.IntroDialogueEnded += OnCustomerIntroEndMoveToCraftingView;
        CustomerDialogueTracker.OutroDialogueTriggered += OnDialogueOutroExitFocusedView;

        InventoryButton.InventoryButtonPressed += OnInventoryButtonPressed;

        RecipeProgressTracker.onRecipeCompleted.AddListener(OnRecipeCompleted);

        toolkitMenu.ToolRollout += OnToolRollout;
        toolkitMenu.gameObject.SetActive(false);

        uiElementFactory.CreateDefaultCraftingActionWidget(craftingButton, craftingUiMenu.CraftingButtonContainer);
    }

    void Update()
    {

        if (craftingUiMenu.entryDisabled)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.C) && !craftingUiMenu.exitDisabled)
        {
            craftingUiMenu.ToggleMenu();
        }

        if (!craftingUiMenu.Active)
        {
            FocusedOnCrafting = false;
            return;
        }

        CraftingUiBusy = !orderSnippet.IsMoving || toolkitMenu.ToolkitOpen;

        craftingUiMenu.UpdateDisplayWithSequenceProgress();

        if (Input.GetMouseButtonUp(0) && !toolkitMenu.ToolkitOpen)
        {
            EndCraftingSequence();
        }

        // craftingUiMenu.occupiablePositionContainer.Print();
    }

    public void SubmitCraftingActionToItemCrafter(CraftingAction action)
    {
        itemCrafter.ExecuteCraftingAction(this, action);
    }

    private void PlaceObjects(ItemObjectUiElement element)
    {
        bool placedSuccessfully = craftingUiMenu.occupiablePositionContainer.TryPlaceObjectInOccupiablePosition(element);

        element.OccupierTransform.ToUnitScaleLocal();

        if (!placedSuccessfully)
        {
            element.SetPositionAndUnitScale(Vector3.zero);
        } 
    }

    private void OnInventoryButtonPressed(object source, ItemEventArgs args)
    {
        ItemObjectUiElement newItemUi = ItemDataManager.CreateItemObjectUiElement(transform, args.ItemToPass.itemId);
        newItemUi.transform.SetAsFirstSibling();
        this.PlaceObjects(newItemUi);
    }

    private void EndCraftingSequence()
    {
        ItemCrafter.EndCraftingSequence();

        itemCrafter.StoreCompletedCraftingSequence();
    }

    private void OnCraftingMenuStatusChange(object source, UiEventArgs args)
    {
        craftingConversationBehavior.enabled = args.Load;
        try
        {
            craftingConversationBehavior.SetActiveConversation(CustomerOrderManager.GetActiveOrder());
        }
        catch (InvalidOperationException)
        {
            Debug.Log("No Active Orders!");
        }
    }

    private void OnRecipeCompleted(Recipe recipe)
    {
        ItemObjectUiElement completed = ItemDataManager.CreateItemObjectUiElement(transform, recipe.outputId);

        this.PlaceObjects(completed);

        float outroDelay = 1f;

        craftingConversationBehavior.InvokeOutroDialogue(outroDelay * 0.75f);

        craftingUiMenu.occupiablePositionContainer.ReleaseAll(this, outroDelay);
    }

    private void OnDialogueOutroExitFocusedView(object source, EventArgs args)
    {
        ConversationSnippet.DestroyAllCustomerResponseSnippets(this);

        StartCoroutine(ExitFocusedCraftingModeAfterTimeDelay(1f));

        CameraManager.ChangeViewTo();
    }

    private void OnCustomerIntroEndMoveToCraftingView(object source, EventArgs args)
    {
        string orderMessage = CustomerOrderManager.ActiveOrderDetails;

        CameraManager.CraftingFocusView();

        StartCoroutine(MoveOrderSnippetIntoFocusedOnCraftingView(orderMessage));
    }

    private void OnToolRollout(object source, EventArgs args)
    {
        if (!FocusedOnCrafting)
        {
            Debug.Log("Not ready to focus on crafting.");
            return;
        }

        uiElementFactory.CreateCraftingActionWidgetsForToolkit(craftingButton, craftingUiMenu.CraftingButtonContainer, withDefaultAction: true, clearAllChildren: true);
    }

    private IEnumerator MoveOrderSnippetIntoFocusedOnCraftingView(string orderMessage)
    {
        float moveTime = 3f;
        float hangTime = 2f;

        orderSnippet.MoveOrderSnippetThroughScreen(orderMessage, moveTime, hangTime, moveTime);

        yield return new WaitForSeconds(2);

        FocusedOnCrafting = true;
    }

    private IEnumerator ExitFocusedCraftingModeAfterTimeDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        FocusedOnCrafting = false;
    }

    public static void LockPlayerInCrafting(bool openMenu = false)
    {

        ToggleCraftingMenuEntry(lockExit: true);

        if (openMenu)
        {
            craftingUiMenu.LoadMenu();
        }
    }

    public static void IncreaseDistraction(float amount)
    {
        DistractionJitter *= amount;
    }

    public static void AddFocusAreaScreenIndicator(FocusAreaObject focus)
    {
        if (focusAreaIndicators.ContainsKey(focus))
        {
            focusAreaIndicators[focus].SetActive(true);
        } 
        else
        {
            GameObject indicatorPrefab = Instantiate(craftingUI.focusAreaIndicatorPrefab);

            FocusAreaIndicator indicator = uiElementFactory.CreateFocusAreaIndicator(indicatorPrefab, focus);

            focusAreaIndicators.Add(focus, indicator.gameObject);
        }
    }

    public static void TryRemoveFocusAreaScreenIndicator(FocusAreaObject focus, bool destroy)
    {
        if (focusAreaIndicators.ContainsKey(focus))
        {
            GameObject o = focusAreaIndicators[focus];

            if (destroy)
            {
                focusAreaIndicators.Remove(focus);
                Destroy(o);
            } else
            {
                o.SetActive(false);
            }
        }
    }

    private static void ToggleCraftingMenuEntry(bool lockExit)
    {
        DisableCraftingMenuEntry(!lockExit);
        DisableCraftingMenuExit(lockExit);
    }

    private static void DisableCraftingMenuEntry(bool value)
    {
        craftingUI.StartCoroutine(EditMenuAvailability(true, value));
    }

    private static void DisableCraftingMenuExit(bool value)
    {
        craftingUI.StartCoroutine(EditMenuAvailability(false, value));
    }

    private static IEnumerator EditMenuAvailability(bool entry, bool disable)
    {
        yield return new WaitUntil(() => craftingUiMenu != null);

        if (entry)
        {
            craftingUiMenu.entryDisabled = disable;
        } else
        {
            craftingUiMenu.exitDisabled = disable;
        }
    }

    private void OnDestroy()
    {
        craftingUiMenu.TailoredUiEvent -= OnCraftingMenuStatusChange;

        CustomerDialogueTracker.IntroDialogueEnded -= OnCustomerIntroEndMoveToCraftingView;
        CustomerDialogueTracker.OutroDialogueTriggered -= OnDialogueOutroExitFocusedView;

        InventoryButton.InventoryButtonPressed -= OnInventoryButtonPressed;
        RecipeProgressTracker.onRecipeCompleted.RemoveListener(OnRecipeCompleted);

        toolkitMenu.ToolRollout -= OnToolRollout;
    }
}