using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralStorePrologue : MonoBehaviour, IDistributeItems<ItemProvisioner>, ILevel, IDialogueCommandReceiver
{
    RealTimeCounter prologueCounter;

    TimeSpan startTime;

    bool timer;
    public bool OnBreak
    {
        get
        {
            if (timer != prologueCounter.TimerComplete)
            {
                OnBreakChange(!timer);
            }

            timer = prologueCounter.TimerComplete;
            return !timer;
        }
    }
    public bool atWork;
    public ItemProvisioner Distributor { get; private set; }
    public string LevelDataPath => "Assets/Resources/Databases/GeneralStoreLevelData.json";

    public LevelData LevelData => LevelData.GetFromPath(LevelDataPath);

    public static CustomerOrder[] prologueCraftingOrders;

    public Transform queueHead;

    public bool craftingMenuOpened;

    public string setCustomerName;

    private void Start()
    {
        LevelManager.SetActiveLevel(this);

        prologueCounter = new RealTimeCounter(1f, TimeUnitToTrack.Minute);
        startTime = new TimeSpan(12, 22, 27);
        StartCoroutine(IncrementTimeSpan(1));

        Distributor = new ItemProvisioner(this);
        Distributor.SetItemDropData((4, 2));

        Distributor.Distribute();

        prologueCraftingOrders = CustomerOrder.CreateCustomerOrders((0, OrderType.CRAFTING, 0), (1, OrderType.CRAFTING, 0));
        CustomerOrder.QueueAndSpawnCustomers(this, "WorkbenchQueueHead", prologueCraftingOrders);

        //CraftingUI.SetDisableCraftingMenuEntry(true);

        UiMenu.UiEvent += OnUiEvent;
        DialogueX.DialogueCommand += OnDialogueCommand;

    }

    private void Update()
    {
        prologueCounter.DecrementTimerOverTime();
        PrologueHUD.SetTimerDisplayMessage(startTime.ToString());//+ $"\n{BreakMessage}");

        if (atWork && GlobalFader.IsClear)
        {
            Debug.Log("Actively taking customers");
            CraftingManager.LockPlayerInCrafting(true);
            //ui display of customer at counter --> crafting ui
        }
    }

    private IEnumerator IncrementTimeSpan(int secondsToIncrement)
    {
        TimeSpan incrementer = new TimeSpan(0, 0, secondsToIncrement);
        while (gameObject.activeSelf)
        {
            yield return new WaitForSeconds(secondsToIncrement);
            startTime = startTime.Add(incrementer);
        }
    }

    void OnBreakChange(bool value)
    {
        atWork = value;
        PlayerBehavior.FreezePlayerMovementController(value);
        GlobalFader.TransitionBetweenFade(this, RunOnBreakChange(), 2, 0.5f, 2.2f);
    }

    public IEnumerator RunOnBreakChange()
    {
        yield return new WaitForSeconds(1);
        //WarpBehavior.ForceWarpToLocation(PlayerBehavior.PlayerGameObject, "Workbench");
        Distributor.Distribute();
    }

    void OnUiEvent(object source, UiEventArgs args)
    {
        if (!craftingMenuOpened && args.MenuTypeLoaded.Equals(MenuType.Crafting) && args.Load)
        {
            craftingMenuOpened = true;
           // StartCoroutine(IntroductionToCraftingAndCustomers());
        }
    }

    IEnumerator IntroductionToCraftingAndCustomers()
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
