using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Data.OleDb;
using System.Collections;

public class GlobalFader : MonoBehaviour
{
    private static GlobalFader globalFader;

    private static Image FaderImage;

    private static Color colorAtPreviousFrame;
    private static bool fading;
    public static bool Fading
    {
        get
        {
            if (colorAtPreviousFrame.Equals(FaderImage.color)){

                if (fading)
                {
                    fading = false;
                    //event or something
                }
                colorAtPreviousFrame = FaderImage.color;

                return false;
            }  else
            {
                fading = true;
                colorAtPreviousFrame = FaderImage.color;

                return true;
            }
        }
    }

    public static bool IsClear => IsColor(Color.clear);

    private void Awake()
    {
        if (globalFader == null)
        {
            globalFader = this;
        } else
        {
            Destroy(this);
        }
        SetFaderImageReference();
    }


    void SetFaderImageReference()
    {
        try
        {
            FaderImage = GetComponentInChildren<Image>();
        }
        catch (NullReferenceException)
        {
            GameObject image = new GameObject("FaderImage", typeof(Image));
            FaderImage = image.GetComponent<Image>();
        }
        colorAtPreviousFrame = FaderImage.color;
    }

    public static bool IsColor(Color checkingForColor)
    {
        return FaderImage.color.Equals(checkingForColor);
    }

    public static void FadeToColorAtSpeed(Color color, float speed)
    {
        FaderImage.DOColor(color, speed);
    }

    public static void FadeToBlack(float duration)
    {
        FadeToColorAtSpeed(Color.black, duration);
    }

    public static void FadeToClear(float duration)
    {
        FadeToColorAtSpeed(Color.clear, duration);
    }

    public static void TransitionBetweenFade(string runInBetweenFades, float inFade, float outFade)
    {
        globalFader.StartCoroutine(FadeTransition(globalFader, runInBetweenFades, inFade, outFade));
    }

    public static void TransitionBetweenFade(MonoBehaviour host, IEnumerator runInBetweenFades, float inFade, float outFade, float wait)
    {
        globalFader.StartCoroutine(FadeTransition(host, runInBetweenFades, inFade, outFade, wait));
    }

    static IEnumerator FadeTransition(MonoBehaviour host, string runInBetween, float inFadeDuration, float outFadeDuration)
    {
        FadeToBlack(inFadeDuration);
        yield return new WaitUntil(() => IsColor(Color.black));

        yield return host.StartCoroutine(runInBetween);

        FadeToClear(outFadeDuration);
        yield return new WaitUntil(() => IsColor(Color.clear));
    }

    static IEnumerator FadeTransition(MonoBehaviour host, IEnumerator runInBetween, float inFadeDuration, float outFadeDuration, float wait)
    {
        FadeToBlack(inFadeDuration);
        yield return new WaitUntil(() => IsColor(Color.black));

        Coroutine between = host.StartCoroutine(runInBetween);

        yield return new WaitForSeconds(wait);

        FadeToClear(outFadeDuration);
        yield return new WaitUntil(() => IsColor(Color.clear));
    }
}
