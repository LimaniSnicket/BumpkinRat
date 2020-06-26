using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    private static PlayerBehavior player;
    public PlayerData playerData;

    void OnEnable()
    {
        if (player == null) { player = this; } else { Destroy(this); }
        if (playerData == null) { playerData = new PlayerData(); }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            playerData.SetValue("playerName", "Billy-Bob");
        }
    }

    void OnDisable()
    {

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
