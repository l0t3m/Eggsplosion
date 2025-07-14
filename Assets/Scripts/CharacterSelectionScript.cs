using Fusion;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class CharacterSelectionScript : NetworkBehaviour
{
    [SerializeField] Material[] playerColors;
    [SerializeField] CharacterSelectionIndividualHandler playerUIPrefab;

    private NetworkRunner runner;
    private CharacterSelectionIndividualHandler playersOnUI;

    [Networked, Capacity(4)]
    private NetworkArray<int> playerSelectedColors => default;

    [Networked, Capacity(4)]
    private NetworkArray<NetworkId> objectIDs => default;

    private int playerColorIndex;
    private int playerNum = 0;

    public override void Spawned()
    {
        base.Spawned();
        playersOnUI.PlayerNumber = playerNum;
        playerSelectedColors.Set(playerNum, 0);
        RPC_ChangeCharacterColor(playerNum, playerNum);
        playersOnUI.NextPressed += PressNext;
        playersOnUI.PreviousPressed += PressPrev;
        objectIDs.Set(playerNum, playersOnUI.Id);
        foreach (NetworkId id in objectIDs)
        {
            if (id != playersOnUI.Id)
            {
                runner.Spawn(runner.FindObject(id)).transform.SetParent(transform);
            }
            else
                runner.FindObject(id).transform.SetParent(transform);
        }
    }

    public async void PlayerJoined(NetworkRunner nr, int player)
    {
        Debug.Log("PLAYER IS " + player);   
        
        playerColorIndex = player;
        var playersOnUI = await nr.SpawnAsync(playerUIPrefab) as CharacterSelectionIndividualHandler;       
        runner = nr;
        playersOnUI.gameObject.transform.SetParent(transform);
        Debug.Log(transform.position.ToString());
        playerNum = player;
    }
   
    public void PressNext(int player)
    {
        int i = playerColorIndex;
        do
        {
            i++;
            if (i == playerColors.Length)
                i = 0;
        }
        while (playerSelectedColors.Contains(i));
        RPC_ChangeCharacterColor(player, i);
    }

    public void PressPrev(int player)
    {
        int i = playerColorIndex;
        do
        {
            i--;
            if (i == -1)
                i = playerColors.Length - 1;
        }
        while (playerSelectedColors.Contains(i));
        RPC_ChangeCharacterColor(player, i);
    }

    [Rpc]
    private void RPC_ChangeCharacterColor(int player, int color)
    {
        playerColorIndex = color;
        playerSelectedColors.Set(player, color);
        runner.FindObject(objectIDs.Get(player)).GetComponent<Image>().material = playerColors[playerSelectedColors.Get(player)];
        playersOnUI.PlayerText.text = playerColors[playerSelectedColors.Get(player)].name;
    }
}
