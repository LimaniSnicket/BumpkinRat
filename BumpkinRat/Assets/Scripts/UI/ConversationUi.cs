using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConversationUi : MonoBehaviour
{
    Stack<string> storedMessages;

    bool responding;

    public bool CanRespond => responding;
    public string joined;

    StringBuilder stringBuilder;
    LoremIpsum loremIpsum;

    public TextMeshProUGUI displayConversationMessages;

    private void Start()
    {
        storedMessages = new Stack<string>();
        stringBuilder = new StringBuilder();
        loremIpsum = new LoremIpsum();
    }

    private void Update()
    {
        UpdateConversationDisplay();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            RespondMessage(KeyCode.Alpha1);
        }  

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            RespondMessage(KeyCode.Alpha2);
        }
    }

    public void RespondMessage(KeyCode pressed)
    {
        if (!responding)
        {
            StartCoroutine(TakeResponse(pressed));
        }
    }

    IEnumerator TakeResponse(KeyCode pressed)
    {
        responding = true;
        string response = $"Responding with {pressed}";
        storedMessages.Push(response);
        stringBuilder.AppendLine(storedMessages.Peek());
        joined = stringBuilder.ToString();
        yield return new WaitForSeconds(1);
        SetConversationNodeToRespondTo();
        responding = false;
    }

    void SetConversationNodeToRespondTo()
    {
        string lorem = loremIpsum.GetLoremIpsum();
        storedMessages.Push(lorem + ".");
        stringBuilder.AppendLine(storedMessages.Peek());
        joined = stringBuilder.ToString();
    }

    void UpdateConversationDisplay()
    {
        if(displayConversationMessages != null)
        {
            displayConversationMessages.text = joined;
        }
    }
}
