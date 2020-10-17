using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour, IWarpTo
{
    private static PlayerBehavior player;
    public PlayerData playerData { get; set; }
    static MovementController PlayerMovementController { get; set; }

    public static Vector3 playerPosition => player.transform.position;

    public static GameObject PlayerGameObject => player.gameObject;

    void OnEnable()
    {
        if (player == null) { player = this; } else { Destroy(this); }
        if (playerData == null) { playerData = new PlayerData(); }

        GetOrAddMovementController();

        DialogueRunner.DialogueEventIndicated += OnDialogueEvent;
        UiMenu.UiEvent += OnUiEvent;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
        //    this.SetValue("playerData.playerName", "Billy-Bob");
            bool nestedEval = this.EvaluateValue("playerData.playerName", "Billy", true);
            if (nestedEval) { Debug.Log(playerData.playerName + ": " + nestedEval); }
            
            if (PlantingManager.NearestPlantingSpace != null)
            {
                PlantingSpace space = PlantingManager.NearestPlantingSpace;
                space.Plant(PlantingManager.GetRandomPlant());
            }
        }
    }

    void OnDialogueEvent(object source, IndicatorArgs args)
    {
        if (args.TargetObject(this)){
            Debug.Log("Valid Indicator Event");
        }
    }

    void OnUiEvent(object source, UiEventArgs args)
    {
        SetFreezePlayerMovementController(args.load);
    }

    void OnDisable()
    {
        DialogueRunner.DialogueEventIndicated -= OnDialogueEvent;
    }

    public static void SetFreezePlayerMovementController(bool freeze)
    {
        PlayerMovementController.SetFreezePlayerMovement(freeze);
    }

    void GetOrAddMovementController()
    {
        if (PlayerMovementController != null)
        {
            return;
        }
        try
        {
            PlayerMovementController = GetComponent<MovementController>();
        }
        catch (NullReferenceException)
        {
            PlayerMovementController = gameObject.AddComponent<MovementController>();
        }
    }

    public void OnWarpBegin()
    {
        SetFreezePlayerMovementController(true);
    }

    public void OnWarpEnd()
    {
        SetFreezePlayerMovementController(false);
    }
}

[Serializable]
public class PlayerData
{
    [SerializeField] string player_name;
    public PlayerData() { }

    public void SetPlayerName(string name) { player_name = name; }
    public string playerName {
        get => player_name;
        set => player_name = value;
    }
}
