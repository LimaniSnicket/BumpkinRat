using System;
using UnityEngine;
public class ConversationBubbleOrientationManager
{
    private static Sprite leftSprite, centeredSprite, rightSprite;

    private static readonly float tmpZRotation = 18f;

    private readonly static Vector2 leftAlignedTmpPosition = new Vector2(-55, 0);

    private readonly static Vector2 centerAlignedTmpPosition = new Vector2(-23, -6);

    private readonly static Vector2 rightAlignedTmpPosition = new Vector2(15, -14);

    private readonly static Vector2 rightAlignedPivot = new Vector2(0.78f, 0.22f);

    private readonly static Vector2 centerAlignedPivot = new Vector2(0.53f, 0);

    private readonly static Vector2 leftAlignedPivot = new Vector2(0.17f, 0.22f);

    public static void InitializeSprites(Sprite left, Sprite center, Sprite right)
    {
        if (leftSprite == null)
        {
            leftSprite = left;
            centeredSprite = center;
            rightSprite = right;
        }
    }

    public static ConversationBubbleOrientation[] MoveToRight(BubbleOrientation start)
    {
        return MoveTowardsOrientation(start, BubbleOrientation.RIGHT);
    }

    public static ConversationBubbleOrientation[] MoveToLeft(BubbleOrientation start)
    {
        return MoveTowardsOrientation(start, BubbleOrientation.LEFT);
    }

    private static ConversationBubbleOrientation[] MoveTowardsOrientation(BubbleOrientation start, BubbleOrientation desired)
    {
        if (start.Equals(desired))
        {
            return Array.Empty<ConversationBubbleOrientation>();
        }
        else
        {
            ConversationBubbleOrientation opposite = GetOpposite(start);
            return start.Equals(BubbleOrientation.CENTER) 
                ? new ConversationBubbleOrientation[] 
                { 
                    opposite 
                } 
                : new ConversationBubbleOrientation[] 
                { 
                    ConversationBubbleOrientation.Center, 
                    opposite 
                };
        }
    }

    private static ConversationBubbleOrientation GetOpposite(BubbleOrientation orientation)
    {
        if (orientation.Equals(BubbleOrientation.LEFT))
        {
            return ConversationBubbleOrientation.Right;
        } 
        else if (orientation.Equals(BubbleOrientation.RIGHT))
        {
            return ConversationBubbleOrientation.Left;
        }

        return ConversationBubbleOrientation.Center;
    }

    internal static Sprite GetSpriteForOrientation(BubbleOrientation orientation)
    {
        switch (orientation)
        {
            case BubbleOrientation.LEFT:
                return leftSprite;
            case BubbleOrientation.CENTER:
                return centeredSprite;
            case BubbleOrientation.RIGHT:
                return rightSprite;
            case BubbleOrientation.NONE:
            default:
                return null;
        }
    }

    internal static Vector3 GetTmpEulersForOrientation(BubbleOrientation orientation)
    {
        float zRotationFactor = 0;

        if(!(orientation == BubbleOrientation.CENTER || orientation == BubbleOrientation.NONE))
        {
            zRotationFactor = orientation == BubbleOrientation.LEFT ? -1 : 1;
        }

        return tmpZRotation * zRotationFactor * Vector3.forward;
    }

    internal static Vector3 GetTmpPositionForOrientation(BubbleOrientation orientation)
    {
        if (!(orientation == BubbleOrientation.CENTER || orientation == BubbleOrientation.NONE))
        {
            return orientation == BubbleOrientation.LEFT ? leftAlignedTmpPosition : rightAlignedTmpPosition;
        }

        return centerAlignedTmpPosition;
    }

    internal static Vector2 GetPivotForOrientation(BubbleOrientation orientation)
    {
        if (!(orientation == BubbleOrientation.CENTER || orientation == BubbleOrientation.NONE))
        {
            return orientation == BubbleOrientation.LEFT ? leftAlignedPivot : rightAlignedPivot;
        }

        return centerAlignedPivot;
    }
}
public class ConversationBubbleOrientation
{
    public BubbleOrientation BubbleOrientation { get; private set; }
    public Sprite SpriteForOrientation { get; private set; }

    public Vector3 TmpEulers { get; private set; }

    public Vector3 TmpLocalPosition { get; private set; }

    private Vector2 pivotForOrientation;

    private static ConversationBubbleOrientation left;
    private static ConversationBubbleOrientation center;
    private static ConversationBubbleOrientation right;

    private readonly static Vector2 defaultPivot = new Vector2(0.5f, 0.5f);

    private ConversationBubbleOrientation(BubbleOrientation orientation)
    {
        BubbleOrientation = orientation;
        SpriteForOrientation = ConversationBubbleOrientationManager.GetSpriteForOrientation(orientation);
        TmpEulers = ConversationBubbleOrientationManager.GetTmpEulersForOrientation(orientation);
        TmpLocalPosition = ConversationBubbleOrientationManager.GetTmpPositionForOrientation(orientation);
        pivotForOrientation = ConversationBubbleOrientationManager.GetPivotForOrientation(orientation);
    }

    public static ConversationBubbleOrientation Left
    {
        get
        {
            if(left == null)
            {
                left = new ConversationBubbleOrientation(BubbleOrientation.LEFT);
            }

            return left;
        }
    }

    public static ConversationBubbleOrientation Center
    {
        get
        {
            if (center == null)
            {
                center = new ConversationBubbleOrientation(BubbleOrientation.CENTER);
            }

            return center;
        }
    }

    public static ConversationBubbleOrientation Right
    {
        get
        {
            if (right == null)
            {
                right = new ConversationBubbleOrientation(BubbleOrientation.RIGHT);
            }

            return right;
        }
    }

    public void AlignPivotWithOrientation(RectTransform rect)
    {
        rect.pivot = pivotForOrientation;
    }

    public void ResetPivotToDefault(RectTransform rect)
    {
        rect.pivot = defaultPivot;
    }
}

public enum BubbleOrientation
{
    NONE = 0,
    LEFT = -1,
    RIGHT = 1,
    CENTER = 2
}