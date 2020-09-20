using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralStorePrologue : MonoBehaviour, IDistributeItems<ItemProvisioner>
{
    RealTimeCounter prologueCounter;

    TimeSpan startTime;
    TimeSpan addOneSecond;

    bool timer;
    public bool OnBreak
    {
        get
        {
            if(timer != prologueCounter.TimerComplete)
            {
                OnBreakChange(!timer);
            }

            timer = prologueCounter.TimerComplete;
            return !timer;
        }
    }
    public bool atWork;

    string breakMessage => OnBreak ? "Lunch Break!" : "Back to Work!";

    public ItemProvisioner ItemDistributor { get; set; }

    public List<ItemDrop> ItemDropData { get; set; }

    private void Start()
    {
        prologueCounter = new RealTimeCounter(0.1f, TimeUnitToTrack.Minute);
        startTime = new TimeSpan(12, 14, 27);
        addOneSecond = new TimeSpan(0, 0, 1);
        StartCoroutine(AddToTimeSpan());

        ItemDistributor = new ItemProvisioner(this);
        ItemDropData = ItemDrop.GetListOfItemsToDrop(("item_a", 1), ("item_b", 2));
    }

    private void Update()
    {
        prologueCounter.DecrementTimerOverTime();
        PrologueHUD.SetTimerDisplayMessage(startTime.ToString() + $"\n{breakMessage}");
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
        WarpBehavior.ForceWarpToLocation(PlayerBehavior.PlayerGameObject, "Workbench");
        ItemDistributor.Distribute();
    }
}
