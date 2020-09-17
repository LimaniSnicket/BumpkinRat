using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralStorePrologue : MonoBehaviour
{
    RealTimeCounter prologueCounter;

    private void Start()
    {
        prologueCounter = new RealTimeCounter(0.5f, TimeUnitToTrack.Minute);
    }

    private void Update()
    {
        prologueCounter.DecrementTimerOverTime();
    }
}
