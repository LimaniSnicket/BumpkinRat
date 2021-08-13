using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class CraftingPointer : MonoBehaviour
{

    private static CraftingPointer craftingPointer;

    private Image thisImage;
    private static Image faPointerBase;
    private static TextMeshProUGUI faPointerText;

    public Sprite grabbedSprite, openSprite;

    public static bool Grabbing => ItemCrafter.CraftingSequenceActive;

    LineRenderer line;


    private void Awake()
    {
        this.InitializeStaticInstance(craftingPointer);
    }

    private void OnEnable()
    {
        Cursor.visible = false;
    }

    void Start()
    {
        faPointerBase = transform.GetChild(0).GetComponent<Image>();
        faPointerText = faPointerBase.GetComponentInChildren<TextMeshProUGUI>();
        thisImage = GetComponent<Image>();

        faPointerBase.gameObject.SetActive(false);
        line = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        transform.position = Input.mousePosition;

        thisImage.sprite = Grabbing ? grabbedSprite : openSprite;
    }

    public static void OnFocusAreaHover(FocusAreaObject fa)
    {
     //   faPointerBase.gameObject.SetActive(true);
        faPointerText.text = "Grab @: " + fa.focusAreaId.ToString();
    }

    public static void OnHoverEnd()
    {
        faPointerBase.gameObject.SetActive(false);
    }
}
