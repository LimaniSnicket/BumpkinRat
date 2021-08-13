using System.Collections;
using UnityEngine;
using CoroutineHelper;

namespace CoroutineHelper
{
    public class StartCoroutineHelper : MonoBehaviour
    {
        private static StartCoroutineHelper startCoroutineHelper;
        public static StartCoroutineHelper CoroutineHelper => startCoroutineHelper;

        private void Awake()
        {
            this.InitializeStaticInstance(startCoroutineHelper);
        }
    }
}

public static class CoroutineX
{
    public static IEnumerator RunWithStartDelay(this IEnumerator routine, float delay)
    {
        if(delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }

        yield return StartCoroutineHelper.CoroutineHelper.StartCoroutine(routine);
    }

    public static IEnumerator RunWithEndDelay(this IEnumerator routine, float delay)
    {
        yield return StartCoroutineHelper.CoroutineHelper.StartCoroutine(routine);

        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }
    }

    public static IEnumerator RunWithDelays(this IEnumerator routine, float startDelay, float endDelay)
    {
        if (startDelay > 0)
        {
            yield return new WaitForSeconds(startDelay);
        }

        yield return StartCoroutineHelper.CoroutineHelper.StartCoroutine(routine);

        if (endDelay > 0)
        {
            yield return new WaitForSeconds(endDelay);
        }
    }
}
