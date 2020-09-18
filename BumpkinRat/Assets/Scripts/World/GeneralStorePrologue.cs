using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralStorePrologue : MonoBehaviour
{
    RealTimeCounter prologueCounter;

    TimeSpan startTime;
    TimeSpan addOneSecond;

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
        PrologueHUD.SetTimerDisplayMessage(startTime.ToString());
    }

    IEnumerator AddToTimeSpan()
    {
        while (gameObject.activeSelf)
        {
            yield return new WaitForSeconds(1);
            startTime = startTime.Add(addOneSecond);
        }
    }
}
