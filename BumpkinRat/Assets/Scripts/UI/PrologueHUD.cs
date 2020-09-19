using UnityEngine;
using TMPro;

public class PrologueHUD : MonoBehaviour
{
    private static PrologueHUD prologueHud;

    private static string timerDisplayMessage;

    public TextMeshProUGUI timerDisplayTMP;
    private static TextMeshProUGUI TimerDisplayTMP { get; set; }
    private void Awake()
    {
        if(prologueHud == null)
        {
            prologueHud = this;
            SetTimerDisplayTextMeshPro();
            timerDisplayMessage = "";

        } else
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        UpdateTimerDisplayTMP();
    }

    void SetTimerDisplayTextMeshPro()
    {
        if(timerDisplayTMP != null)
        {
            TimerDisplayTMP = timerDisplayTMP;
        }
    }

    public static void SetTimerDisplayMessage(string message)
    {
        if (!timerDisplayMessage.Equals(message))
        {
            timerDisplayMessage = message;
        }
    }

    static void UpdateTimerDisplayTMP()
    {
        if (TimerDisplayTMP != null)
        {
            TimerDisplayTMP.text = timerDisplayMessage;
        }
    }
}
