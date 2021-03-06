﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralStorePrologue : MonoBehaviour, IDistributeItems<ItemProvisioner>, ILevel
{
    RealTimeCounter prologueCounter;

    TimeSpan startTime;
    TimeSpan addOneSecond;

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
    string BreakMessage => OnBreak ? "Lunch Break!" : "Back to Work!";

    public ItemProvisioner ItemDistributor { get; set; }

    public List<ItemDrop> ItemDropData { get; set; }
    public string LevelName => "General Store...";
    public int LevelId => 0;

    public static CustomerOrder CraftingOrderTest;

    public static CustomerOrder[] prologueCraftingOrders;

    public bool craftingMenuOpened;

    private void Start()
    {
        prologueCounter = new RealTimeCounter(1f, TimeUnitToTrack.Minute);
        startTime = new TimeSpan(12, 22, 27);
        addOneSecond = new TimeSpan(0, 0, 1);
        StartCoroutine(AddToTimeSpan());

        ItemDistributor = new ItemProvisioner(this);
        ItemDropData = ItemDrop.GetListOfItemsToDrop((4, 2));

        ItemDistributor.Distribute();

        CraftingOrderTest = new CustomerOrder { 
            npcId = 0,
            orderDetails = new OrderDetails { 
                orderType = OrderType.CRAFTING,
                orderLookupId = 0
            } ,
        };
        CraftingOrderTest.Initialize(this);

        prologueCraftingOrders = CustomerOrder.CreateCustomerOrders((0, OrderType.CRAFTING, 0), (1, OrderType.CRAFTING, 0));
        CustomerOrder.QueueCustomersIntoFreshQueue(prologueCraftingOrders);

        //CraftingUI.SetDisableCraftingMenuEntry(true);

        UiMenu.UiEvent += OnUiEvent;
    }

    private void Update()
    {
        prologueCounter.DecrementTimerOverTime();
        PrologueHUD.SetTimerDisplayMessage(startTime.ToString());//+ $"\n{BreakMessage}");

        if (atWork && GlobalFader.IsClear)
        {
            Debug.Log("Actively taking customers");
            CraftingUI.LockPlayerInCrafting(true);
            //ui display of customer at counter --> crafting ui
        }
    }

    IEnumerator AddToTimeSpan()
    {
        while (gameObject.activeSelf)
        {
            yield return new WaitForSeconds(1);
            startTime = startTime.Add(addOneSecond);
        }
    }

    void OnBreakChange(bool value)
    {
        atWork = value;
        PlayerBehavior.SetFreezePlayerMovementController(value);
        GlobalFader.TransitionBetweenFade(this, RunOnBreakChange(), 2, 0.5f, 2.2f);
    }

    public IEnumerator RunOnBreakChange()
    {
        yield return new WaitForSeconds(1);
        //WarpBehavior.ForceWarpToLocation(PlayerBehavior.PlayerGameObject, "Workbench");
        ItemDistributor.Distribute();
    }

    void OnUiEvent(object source, UiEventArgs args)
    {
        if (!craftingMenuOpened && args.menuLoaded.Equals(MenuType.Crafting) && args.load)
        {
            craftingMenuOpened = true;
            StartCoroutine(IntroductionToCraftingAndCustomers());
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
    }
}
