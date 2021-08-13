using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour, IWarpTo
{
    private static PlayerBehavior player;
    private static MovementController PlayerMovementController { get; set; }

    public static Vector3 PlayerPosition => player.transform.position;
    public static Vector3 DeltaPosition { get; private set; }

    private static Vector3 previousPosition;

    public static GameObject PlayerGameObject => player.gameObject;

    void OnEnable()
    {
        if (player == null) 
        { 
            player = this; 
        } 
        else 
        { 
            Destroy(this); 
        }

        SetPlayerMovementController();

        DialogueRunner.DialogueEventIndicated += OnDialogueEvent;
        UiMenu.UiEvent += OnUiEvent;
    }

    private void Update()
    {
        previousPosition = PlayerPosition;
        if (Input.GetKeyDown(KeyCode.T))
        {
        //    this.SetValue("playerData.playerName", "Billy-Bob");
           // bool nestedEval = this.EvaluateValue("playerData.playerName", "Billy", true);
          //  if (nestedEval) { Debug.Log(playerData.playerName + ": " + nestedEval); }
            
            if (PlantingManager.NearestPlantingSpace != null)
            {
                PlantingSpace space = PlantingManager.NearestPlantingSpace;
                space.Plant(PlantingManager.GetRandomPlant());
            }
        }
    }

    void LateUpdate()
    {
        DeltaPosition = previousPosition - PlayerPosition;
    }

    void OnDialogueEvent(object source, IndicatorArgs args)
    {
        if (args.TargetObject(this)){
            Debug.Log("Valid Indicator Event");
        }
    }

    void OnUiEvent(object source, UiEventArgs args)
    {
        FreezePlayerMovementController(args.Load);
    }

    void OnDisable()
    {
        DialogueRunner.DialogueEventIndicated -= OnDialogueEvent;
    }

    public static void FreezePlayerMovementController(bool isFrozen)
    {
        PlayerMovementController.SetFreezePlayerMovement(isFrozen);
    }

    void SetPlayerMovementController()
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
        FreezePlayerMovementController(true);
    }

    public void OnWarpEnd()
    {
        FreezePlayerMovementController(false);
    }
}
