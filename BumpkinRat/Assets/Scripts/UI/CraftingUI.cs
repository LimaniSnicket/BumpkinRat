using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using TMPro;

public class CraftingUI : MonoBehaviour, IUiFunctionality<CraftingMenu> //driver class
{
    static CraftingUI craftingUI;

    public GameObject craftingButton;

    public GameObject craftingActionButtonParent;

    public GameObject progressDisplay;

    public List<CraftingActionButton> craftingActionButtons;

    static Dictionary<FocusArea, GameObject> focusAreaIndicators;

    public GameObject focusAreaIndicatorPrefab;

    public static float distraction = 0.25f;

    public bool focusOnCrafting;

    public CraftingMenu MenuFunctionObject { get; set; }

    public ConversationUi CraftingConversationBehavior => GetComponent<ConversationUi>();

    int numberOfTimesOpened;


    void Awake()
    {
        if (craftingUI == null)
        {
            craftingUI = this;
            focusAreaIndicators = new Dictionary<FocusArea, GameObject>();
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        MenuFunctionObject = new CraftingMenu(gameObject);
        MenuFunctionObject.SetCraftingActionButtons(craftingActionButtonParent, craftingButton, this);
        MenuFunctionObject.SetCraftingSequenceDisplay(progressDisplay);

        CraftingConversationBehavior.enabled = false; //not in conversation by default

        MenuFunctionObject.TailoredUiEvent += OnCraftingMenuStatusChange;
        CustomerDialogueTracker.IntroDialogueEnded += EnterCraftAfterConversationInfoView;
    }

    void Update()
    {

        if (MenuFunctionObject.entryDisabled)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            MenuFunctionObject.ChangeMenuStatus();
        }

        if (!MenuFunctionObject.Active)
        {
            return;
        }

        MenuFunctionObject.UpdateDisplay(MenuFunctionObject.itemCrafter.activeSequence.GetSequenceProgressDisplay());

        if (Input.GetMouseButtonUp(0))
        {
            OnMouseButtonUpEndCraftingSequence();
        }
    }

    void EnterCraftAfterConversationInfoView(object source, EventArgs args)
    {
        CameraManager.CraftingFocusView();
    }

    public static void HandleFocusAreaInstances(FocusArea area, bool add, bool destroy = false)
    {
        if (add)
        {
            TryAddFocusAreaScreenIndicator(area);
        } else
        {
            TryRemoveFocusAreaScreenIndicator(area, destroy);
        }
    }

    static void TryAddFocusAreaScreenIndicator(FocusArea focus)
    {
        if (focusAreaIndicators.ContainsKey(focus))
        {
            focusAreaIndicators[focus].SetActive(true);
        } else
        {
            GameObject indicatorPrefab = Instantiate(craftingUI.focusAreaIndicatorPrefab);
            FocusAreaIndicator indicator = new FocusAreaIndicator(indicatorPrefab);
            indicator.gameObject.transform.SetParent(craftingUI.transform);
            indicator.gameObject.GetComponent<RectTransform>().position = 
                RectTransformUtility.WorldToScreenPoint(Camera.main, focus.transform.position);

            indicator.SetTMPMessage(focus.ToString());

            focusAreaIndicators.Add(focus, indicator.gameObject);
        }
    }

    static void TryRemoveFocusAreaScreenIndicator(FocusArea focus, bool destroy)
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

    public static void SetDisableCraftingMenuEntry(bool value)
    {
        try
        {
            craftingUI.MenuFunctionObject.entryDisabled = value;
        }
        catch (NullReferenceException)
        {
            craftingUI.StartCoroutine(WaitToEditMenuAvailability(true, value));
        }
    }

    public static void SetDisableCraftingMenuExit(bool value)
    {
        craftingUI.StartCoroutine(WaitToEditMenuAvailability(false, value));
    }

    static IEnumerator WaitToEditMenuAvailability(bool entry, bool disable)
    {
        while(craftingUI.MenuFunctionObject == null)
        {
            yield return new WaitForEndOfFrame();
        }
        if (entry)
        {
            craftingUI.MenuFunctionObject.entryDisabled = disable;
        } else
        {
            craftingUI.MenuFunctionObject.exitDisabled = disable;
        }
    }

    public static void LockPlayerInCrafting(bool openMenu = false)
    {
        SetDisableCraftingMenuEntry(false);
        SetDisableCraftingMenuExit(true);

        if (openMenu)
        {
            craftingUI.MenuFunctionObject.LoadMenu();
        }
    }

    public static void Distract(float amount)
    {
        distraction *= amount;
    }

    void OnCraftingMenuStatusChange(object source, UiEventArgs args)
    {
        CraftingConversationBehavior.enabled = args.load;
        numberOfTimesOpened.IncrementIfTrue(args.load);
    }

    public void TakeCraftingActionViaCraftingUI(CraftingAction action)
    {
        MenuFunctionObject.itemCrafter.TakeCraftingAction(this, action);
    }

    private void OnMouseButtonUpEndCraftingSequence()
    {
        ItemCrafter.EndCraftingSequence();
        MenuFunctionObject.itemCrafter.EndLocalCraftingSequence();
    }

    private void OnDestroy()
    {
        MenuFunctionObject.TailoredUiEvent -= OnCraftingMenuStatusChange;
        CustomerDialogueTracker.IntroDialogueEnded -= EnterCraftAfterConversationInfoView;

    }

}

public struct FocusAreaIndicator
{
    public GameObject gameObject;
    public TextMeshProUGUI tmproDisplay;

    public FocusAreaIndicator(GameObject obj)
    {
        gameObject = obj;
        tmproDisplay = gameObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetTMPMessage(string message)
    {
        tmproDisplay.text = message;
    }
}
