using System;
using UnityEngine;

[Serializable]
public abstract class UiMenu
{
    public MenuType MenuType { get; protected set; }

    protected GameObject gameObject;

    public bool exitDisabled;

    public bool entryDisabled;

    public bool Active { get; private set; }
    public abstract KeyCode ActivateKeyCode { get; }
    public abstract void LoadMenu();
    public abstract void CloseMenu();

    public static event EventHandler<UiEventArgs> UiEvent;
    public event EventHandler<UiEventArgs> TailoredUiEvent;

    public void ToggleMenu()
    {
        if (Active && !exitDisabled)
        {
            CloseMenu();
        } 
        else if(!Active && !entryDisabled)
        {
            LoadMenu();
        }
    }

    protected void SendLoadingMenuEvent()
    {
        this.BroadcastUiEvent(true);
    }

    protected void SendClosingMenuEvent()
    {
        this.BroadcastUiEvent(false);
    }

    private void BroadcastUiEvent(bool load)
    {
        UiEventArgs eventToSend = new UiEventArgs { 
            Load = load, 
            MenuTypeLoaded = MenuType,
            UiMenuName = this.GetType().ToString()
        };

        if (UiEvent != null)
        {
            Debug.LogFormat("{0} a menu of type: " + MenuType, load ? "Loading": "Closing");
            UiEvent(this, eventToSend) ;
        }

        TailoredUiEvent.BroadcastEvent(this, eventToSend);

        Active = load;
    }

}
