using UnityEngine;

public class ConversationUiElementFactory
{
    private readonly ConversationUi conversationUi;

    private readonly ConversationResponseDisplayManager responseDisplayManager;
    public ConversationUiElementFactory(ConversationUi ui, ConversationResponseDisplayManager displayManager)
    {
        conversationUi = ui;
        responseDisplayManager = displayManager;
    }

    public ConversationSnippet CreatePlayerSnippetFromResponseDisplay(ConversationResponseDisplay response)
    {
        Transform parent = response.gameObject.transform.parent;

        GameObject duplicated = GameObject.Instantiate(response.gameObject, parent);
        duplicated.name = "New Snippet";

        ConversationSnippet convoSnippet = duplicated.AddComponent<ConversationSnippet>();

        convoSnippet.InitializeSnippetFromBubbleDisplay(response);

        convoSnippet.transform.SetParent(conversationUi.transform);

        ApplyValuesToConvoSnippet(convoSnippet, true);

        return convoSnippet;
    }

    public ConversationSnippet CreateNpcSnippet(string message, Vector2 position)
    {
        return this.GetSnippetAtPosition(message, false, position);
    }

    public ConversationResponseDisplay[] GetResponseDisplays(GameObject[] uiElements, ConversationAesthetic aesthetics)
    {
        ConversationResponseDisplay[] arr = new ConversationResponseDisplay[3];

        for (int i = 0; i < arr.Length; i++)
        {
            ResponseTier tier = (ResponseTier)i;
            arr[i] = this.CreateConversationResponseDisplay(uiElements[i], aesthetics, tier);
        }

        return arr;
    }

    private ConversationSnippet GetSnippetAtPosition(string message, bool response, Vector2 position)
    {
        ConversationSnippet convoSnippet = this.GenerateConversationSnippet(response);

        bool scaleDown = CraftingManager.FocusedOnCrafting && !response;

        convoSnippet.InitializeSnippetTransform(position, scaleDown);
        convoSnippet.ReadLine(message, 0.07f);

        return convoSnippet;
    }

    private ConversationSnippet GenerateConversationSnippet(bool response)
    {
        GameObject snippet = GameObject.Instantiate(conversationUi.conversationSnippetPrefab, conversationUi.transform);
        ConversationSnippet convoSnippet = snippet.GetComponent<ConversationSnippet>();

        ApplyValuesToConvoSnippet(convoSnippet, response);

        return convoSnippet;
    }

    private void ApplyValuesToConvoSnippet(ConversationSnippet convoSnippet, bool response)
    {
        convoSnippet.UpdateChildIndex();
        convoSnippet.SetIsResponse(response);
        convoSnippet.SetConversationUi(conversationUi);
        convoSnippet.ApplyConversationAesthetic(conversationUi.currentConversationAesthetic);
    }

    public ConversationResponseDisplay CreateConversationResponseDisplay(GameObject obj, ConversationAesthetic aesthetic, ResponseTier priorityIndex)
    {
        ConversationResponseDisplay responseDisplay = new ConversationResponseDisplay(obj);
        responseDisplay.InitializeConversationResponseDisplayBubble(aesthetic);

        responseDisplayManager.AddToActiveInConversation(responseDisplay, priorityIndex);
        responseDisplay.SetActiveState(false);

        return responseDisplay;
    }
}