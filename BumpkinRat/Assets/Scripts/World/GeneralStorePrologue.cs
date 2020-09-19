using System;
using System.Collections;
using UnityEngine;

public class GeneralStorePrologue : MonoBehaviour
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

    private void Start()
    {
        prologueCounter = new RealTimeCounter(0.5f, TimeUnitToTrack.Minute);
        startTime = new TimeSpan(12, 14, 27);
        addOneSecond = new TimeSpan(0, 0, 1);
        StartCoroutine(AddToTimeSpan());
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
    }
}
