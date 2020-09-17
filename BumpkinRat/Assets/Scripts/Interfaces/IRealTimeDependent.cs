using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class RealTimeCounter : IRealTimeDependent
{
    public float AmountOfTimeToTrack { get; set; }

    public float AmountOfTimeLeft { get; set; }

    public bool TimerComplete => AmountOfTimeLeft <= 0;

    public RealTimeCounter(float time, TimeUnitToTrack unit)
    {
        AmountOfTimeToTrack = TimeUnitConverter.Converted(time, unit);
        AmountOfTimeLeft = AmountOfTimeToTrack;

    }

    public void DecrementTimerOverTime()
    {
        AmountOfTimeLeft -= Time.deltaTime;
    }
    public void DecrementTimerOverTimeWithModifier(float modifier)
    {
        AmountOfTimeLeft -= Time.deltaTime * modifier;
    }

    public void ResetTimer()
    {
        AmountOfTimeLeft = AmountOfTimeToTrack;
    }
}

public interface IRealTimeDependent
{
    float AmountOfTimeToTrack { get; set; } 
    float AmountOfTimeLeft { get; set; }
    bool TimerComplete { get; }

}

public enum TimeUnitToTrack
{
    None = 0,
    Second = 1,
    Minute = 2,
    Hour = 3
}

public struct TimeUnitConverter
{
    public static float Converted(float time, TimeUnitToTrack unit)
    {
        switch (unit)
        {
            case TimeUnitToTrack.Second:
                return time;
            case TimeUnitToTrack.Minute:
                return time * 60;
            case TimeUnitToTrack.Hour:
                return time * (60 * 60);
            default:
                return 0;
        }
    }
}
