using System;
using System.Collections;
using UnityEngine;

public class RealTimeCounter 
{
    private float timeToTrack;

    private float timeLeft;

    private TimeSpan time;

    public bool TimerComplete => timeLeft <= 0;

    public RealTimeCounter(TimeSpan timespan)
    {
        this.time = timespan;
    }

    public RealTimeCounter(float time, TimeUnit unit, TimeSpan timespan)
    {
        timeToTrack = ConvertToTimeUnit(time, unit);
        timeLeft = timeToTrack;
        this.time = timespan;
    }

    public RealTimeCounter WithOneMinuteIncrement()
    {
        timeToTrack = ConvertToTimeUnit(1, TimeUnit.Minute);
        timeLeft = timeToTrack;

        return this;
    }

    public void DecrementTimer()
    {
        timeLeft -= Time.deltaTime;
    }

    public override string ToString()
    {
        return time.ToString();
    }
    public IEnumerator IncrementTimeSpan(Func<bool> waitCondition, int secondsToIncrement)
    {
        TimeSpan incrementer = new TimeSpan(0, 0, secondsToIncrement);
        while (waitCondition())
        {
            yield return new WaitForSeconds(secondsToIncrement);
            time = time.Add(incrementer);
        }
    }

    private float ConvertToTimeUnit(float time, TimeUnit unit)
    {
        switch (unit)
        {
            case TimeUnit.Second:
                return time;
            case TimeUnit.Minute:
                return time * 60;
            case TimeUnit.Hour:
                return time * (60 * 60);
            default:
                return 0;
        }
    }
}

public enum TimeUnit
{
    None = 0,
    Second = 1,
    Minute = 2,
    Hour = 3
}

