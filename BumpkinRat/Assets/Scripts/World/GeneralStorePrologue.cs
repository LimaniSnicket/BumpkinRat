using System;
using System.Collections;
using BumpkinRat.Crafting;
using UnityEngine;

public class GeneralStorePrologue : LevelBase, IDialogueCommandReceiver
{
    private bool timerComplete;

    private bool atWork;

    private RealTimeCounter prologueCounter;

    public Transform queueHead;

    public bool craftingMenuOpened;

    public string setCustomerName;

    public bool OnBreak
    {
        get
        {
            if (timerComplete != prologueCounter.TimerComplete)
            {
                OnBreakChange(!timerComplete);
            }

            timerComplete = prologueCounter.TimerComplete;
            return !timerComplete;
        }
    }

    private void Start()
    {
        this.ActivateLevel();

        TimeSpan startTime = new TimeSpan(12, 22, 27);

        prologueCounter = new RealTimeCounter(startTime).WithOneMinuteIncrement();
        StartCoroutine(prologueCounter.IncrementTimeSpan(() => gameObject.activeSelf, 1));

        itemDistributer = new ItemProvisioner();
        this.DistributeDropOnStartItems();

        this.customerOrderManager = new CustomerOrderManager();

        customerOrderManager.SpawnCustomersWithOrders(CustomerQueueHead.WorkbenchQueueHead, levelData);

        // CraftingUI.SetDisableCraftingMenuEntry(true);

        UiMenu.UiEvent += OnUiEvent;
        DialogueX.DialogueCommand += OnDialogueCommand;

    }

    private void Update()
    {
        prologueCounter.DecrementTimer();
        PrologueHUD.SetTimerDisplayMessage(prologueCounter.ToString());

        if (atWork && GlobalFader.IsClear)
        {
            Debug.Log("Actively taking customers");
            CraftingManager.LockPlayerInCrafting(true);
            //ui display of customer at counter --> crafting ui
        }
    }

    private void OnBreakChange(bool value)
    {
        atWork = value;
        PlayerBehavior.FreezePlayerMovementController(value);
        GlobalFader.TransitionBetweenFade(this, RunOnBreakChange(), 2, 0.5f, 2.2f);
    }

    public IEnumerator RunOnBreakChange()
    {
        yield return new WaitForSeconds(1);
        //WarpBehavior.ForceWarpToLocation(PlayerBehavior.PlayerGameObject, "Workbench");
        itemDistributer.Distribute();
    }

    void OnUiEvent(object source, UiEventArgs args)
    {
        if (!craftingMenuOpened && args.MenuTypeLoaded.Equals(MenuType.Crafting) && args.Load)
        {
            craftingMenuOpened = true;
           // StartCoroutine(IntroductionToCraftingAndCustomers());
        }
    }

    private IEnumerator IntroductionToCraftingAndCustomers()
    {
        print("Crafting introduction!");
        yield return null;
    }

    void OnDestroy()
    {
        UiMenu.UiEvent -= OnUiEvent;
        DialogueX.DialogueCommand -= OnDialogueCommand;
    }

    public void OnDialogueCommand(object source, DialogueCommandArgs args)
    {
        if (args.ValidateTargetObject(this))
        {
            this.SetValue(args.setting, args.value);
            DialogueX.StackValue(this.GetPropertyOrField(args.setting));
        }
    }
}
