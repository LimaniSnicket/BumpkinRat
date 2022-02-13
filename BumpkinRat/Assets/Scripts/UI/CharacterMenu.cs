using UnityEngine;
using TMPro;

public class CharacterMenu : MonoBehaviour
{
    private NpcDatabaseEntry activeCharacter;

    private TextMeshProUGUI characterNameDisplay;

    public void SetActiveCharacter(NpcDatabaseEntry npc)
    {
        if (this.characterNameDisplay == null)
        {
            this.characterNameDisplay = GetComponentInChildren<TextMeshProUGUI>();
        }

        this.activeCharacter = npc;

        this.characterNameDisplay.text = this.activeCharacter.NpcName;
    }
}
