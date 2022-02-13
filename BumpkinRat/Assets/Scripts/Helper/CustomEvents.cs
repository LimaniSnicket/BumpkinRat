using System;


public class EvaluateEventArgs<T>: EventArgs
{
    public T Evaluate { get; set; }
}
