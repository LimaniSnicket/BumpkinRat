using DG.Tweening;
using UnityEngine;
using TMPro;
using System.Text;
using System.Collections;

public class CraftingOrderSnippet : MonoBehaviour
{
    private RectTransform rectTransform;
    private TextMeshProUGUI orderDisplay;
    private StringBuilder builder;

    private Vector2 hiddenPosition = new Vector2(-1500, -25);

    private Coroutine movingThroughScreenRoutine;
    public bool IsMoving => movingThroughScreenRoutine != null;

    private void Awake()
    {
        InitializeSnippet();
    }

    public void MoveOrderSnippetThroughScreen(string message, float moveInTime, float hangTime, float moveOutTime)
    {
        if (movingThroughScreenRoutine == null)
        {
            movingThroughScreenRoutine = StartCoroutine(MoveThroughScreenView(message, moveInTime, hangTime, moveOutTime));
        }
    }

    private IEnumerator MoveThroughScreenView(string message, float moveInTime, float hangTime, float moveOutTime)
    {
        builder = new StringBuilder(message);

        this.SetOrderDisplayToBuilder();

        rectTransform.DOLocalMoveX(0, moveInTime);

        yield return new WaitForSeconds(hangTime + moveInTime);

        rectTransform.DOLocalMoveX(1600, moveOutTime).OnComplete(() => KillMovingRoutine());
    }

    private void KillMovingRoutine()
    {
        rectTransform.localPosition = hiddenPosition;

        this.ClearText();

        movingThroughScreenRoutine = null;
    }

    private void SetOrderDisplayToBuilder()
    {
        orderDisplay.text = builder.ToString();
    }

    private void InitializeSnippet()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
        orderDisplay = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        builder = new StringBuilder();
        ClearText();
    }

    private void ClearText()
    {
        orderDisplay.text = string.Empty;
    }
}
