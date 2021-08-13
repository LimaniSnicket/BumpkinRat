using DG.Tweening;
using System.Collections;
using System;
using TMPro;
using UnityEngine;

public class DraggableFoldout<T> : MonoBehaviour where T : Transform
{
    public T foldoutTransform, draggerTransform;
    public FoldoutStatus foldout;
    public Vector3 originalPosition;
    public string pullDirection;

    public bool InView { get; private set; }

    protected Coroutine fidgetRoutine;
    protected TMP_Text textDisplay;
    protected Vector3 pullDirectionalVector;

    const string TabPullDirection = "rlud";
    public static bool Interactable { get; protected set; } = true;

    public virtual FoldoutStatus FoldoutStatus
    {
        get
        {
            FoldoutStatus status = GetFoldingStatus();
            if (foldout != status)
            {
                OnFoldoutChange.BroadcastEvent(this, new EvaluateEventArgs<FoldoutStatus> { Evaluate = status });
            }
            foldout = status;
            return foldout;
        }
    }

    public event EventHandler<EvaluateEventArgs<FoldoutStatus>> OnFoldoutChange;
    public void SetLocalPosition(Vector2 position)
    {
        draggerTransform.localPosition = position;
    }

    public void SetOriginalPosition()
    {
        originalPosition = draggerTransform.localPosition;
    }

    public void SetOriginalPosition(Vector2 pos)
    {
        originalPosition = pos;
    }
    public void MarkInView(bool inView)
    {
        InView = inView;
        SetFidgeting();
    }

    protected bool CanDragOut()
    {
        if (TabPullDirection.Contains(pullDirection))
        {
            if(FoldoutStatus.Equals(FoldoutStatus.FOLDED_IN) && Interactable)
            {
                return true;
            }
        }
        return false;
    }

    protected bool ValidMouseDirectionDrag(string direction, out Vector3 directionVector)
    {
        directionVector = Vector3.zero;
        bool valid = true;

        if (TabPullDirection.Contains(direction))
        {
            foreach(char c in direction)
            {
                switch (c)
                {
                    case 'r':
                        valid &= MouseManager.delta.x > 0;
                        directionVector += Vector3.right;
                        break;
                    case 'l':
                        valid &= MouseManager.delta.x < 0;
                        directionVector += Vector3.right * -1;
                        break;
                    case 'u':
                        valid &= MouseManager.delta.y > 0;
                        directionVector += Vector3.up;
                        break;
                    case 'd':
                        valid &= MouseManager.delta.y < 0;
                        directionVector += Vector3.up * -1;
                        break;
                }

                if (!valid)
                {
                    break;
                }
            }
        }

        return valid;
    }

    void SetFidgeting()
    {
        if (fidgetRoutine == null)
        {
            fidgetRoutine = StartCoroutine(Fidget(3f));
        }
        else
        {
            StopCoroutine(fidgetRoutine);
            fidgetRoutine = null;
        }
    }

    public void SetMessage(string message)
    {
        textDisplay.text = message;
    }

    public virtual IEnumerator DragOut()
    {
        Interactable = false;
        yield return null;
    }

    public virtual IEnumerator CollapseBack()
    {
        Interactable = true;
        yield return null;
    }

    protected IEnumerator MovingOut(Vector2 pos, Vector2 scale, float time, float y = 20)
    {
        Vector3 original = draggerTransform.localEulerAngles;
        float yPos = draggerTransform.localPosition.y;
        draggerTransform.DOLocalMove(pos, time);

        float timeChunkOne = time * 0.66f;
        float timeChunkTwo = time - timeChunkOne;

        draggerTransform.DOLocalRotate(original + Vector3.forward * 10, timeChunkOne);
        draggerTransform.DOLocalMoveY(yPos + y, timeChunkOne);
        foldoutTransform.DOScale(scale, time);

        yield return new WaitForSeconds(timeChunkOne);
        draggerTransform.DOLocalRotate(original, timeChunkTwo);
        draggerTransform.DOLocalMoveY(yPos, timeChunkTwo);


        yield return new WaitForSeconds(timeChunkTwo);

    }
    protected IEnumerator Fidget(float waitTime)
    {
        while (InView)
        {
            if (foldout.Equals(FoldoutStatus.FOLDED_IN))
            {
                draggerTransform.DOShakeRotation(0.6f, Vector3.forward * 100, 10, 10);
                yield return new WaitForSeconds(0.6f);

            }

            yield return new WaitForSeconds(waitTime);
        }
    }

    public FoldoutStatus GetFoldingStatus()
    {
        if (Mathf.Approximately(foldoutTransform.localScale.x, 0))
        {
            return FoldoutStatus.FOLDED_IN;
        }
        else if (Mathf.Approximately(foldoutTransform.localScale.x, 1))
        {
            return FoldoutStatus.FOLDED_OUT;
        }

        return FoldoutStatus.FOLDING;
    }
}
