using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorCellBehaviour : MonoBehaviour
{
    protected ColorCell colorCell;
    protected static PaintByNumbers pbn;

    public virtual void InitializeColorCell(PaintByNumbers reference, int num)
    {
        if(pbn == null)
        {
            pbn = reference;
        } 

        colorCell = new ColorCell(num);
    }

    protected virtual void SetColor(SpriteRenderer renderer)
    {
        Color col = pbn.GetColorForNumber(ColorCell.ActiveNumber);
        renderer.color = col;
        colorCell.Paint();
    }
}

public class ColorCell
{
    public static int ActiveNumber = -1;

    private readonly int associatedNumber;
    private int paintedColorNumber = -1;
    private bool painted;

    public ColorCell(int number)
    {
        associatedNumber = number;
    }

    public int CellNumber => associatedNumber;

    public bool IsPaintedCorrectly => painted && associatedNumber.Equals(paintedColorNumber);
    
    public void Paint()
    {
        if(ActiveNumber >= 0)
        {
            paintedColorNumber = ActiveNumber;
            painted = true;
        }
    }

    public void Reset()
    {
        paintedColorNumber = -1;
        painted = false;
    }
}
