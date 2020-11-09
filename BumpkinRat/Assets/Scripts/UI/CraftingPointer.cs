using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class CraftingPointer : MonoBehaviour
{

    private static CraftingPointer craftingPointer;

    private Image thisImage;
    private static Image faPointerBase;
    private static TextMeshProUGUI faPointerText;

    public static bool Grabbing => ItemCrafter.CraftingSequenceActive;


    private void Awake()
    {
        this.InitializeStaticInstance(craftingPointer);
        Cursor.visible = false;
    }

    void Start()
    {
        faPointerBase = transform.GetChild(0).GetComponent<Image>();
        faPointerText = faPointerBase.GetComponentInChildren<TextMeshProUGUI>();
        thisImage = GetComponent<Image>();

        faPointerBase.gameObject.SetActive(false);
    }

    private void Update()
    {
        transform.position = Input.mousePosition;

        thisImage.color = Grabbing ? Color.blue : Color.white;
    }

    public static void OnFocusAreaHover(FocusArea fa)
    {
     //   faPointerBase.gameObject.SetActive(true);
        faPointerText.text = "Grab @: " + fa.focusAreaId.ToString();
    }

    public static void OnHoverEnd()
    {
        faPointerBase.gameObject.SetActive(false);
    }
}
