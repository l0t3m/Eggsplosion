
using Fusion;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class EliminationHandler : NetworkBehaviour
{
    [SerializeField] GameObject losePanel;
    [SerializeField] GameObject winPanel;

    private Dictionary<PlayerRef, bool> playersAlive;
    public override void Spawned()
    {
        base.Spawned();
        if (Runner.IsServer)
        {
            playersAlive = new Dictionary<PlayerRef, bool>();
            foreach (var player in Runner.ActivePlayers)
            {
                if (Runner.LocalPlayer != player)
                    playersAlive[player] = true;

            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Runner.IsServer) 
        {
            if (collision.gameObject.tag == "Player")
            {
                NetworkObject playerNO = collision.gameObject.GetComponent<NetworkObject>();
                RPC_PlayerLost(playerNO.InputAuthority);
                playersAlive[playerNO.InputAuthority] = false;
                Runner.Despawn(playerNO);
                Debug.Log(playersAlive.Count(a => a.Value == true));
                if (playersAlive.Count(a => a.Value == true) <= 1)
                {
                    PlayerRef winner = playersAlive.FirstOrDefault(a => a.Value && a.Key != PlayerRef.None && a.Key != Runner.LocalPlayer).Key;
                    Debug.Log(winner.PlayerId);
                    RPC_ShowWinToPlayer(winner);
                }
            }
        }
     
    }

    [Rpc]
    private void RPC_PlayerLost([RpcTarget] PlayerRef player)
    {
        losePanel.SetActive(true);
    }

    [Rpc]
    public void RPC_ShowWinToPlayer([RpcTarget] PlayerRef player)
    {
        Debug.Log(player.PlayerId);
        winPanel.SetActive(true);
    }

    [Rpc]
    public void RPC_EndGame()
    {
        if (Runner.IsServer)
            Runner.LoadScene("MainMenuScene");
    }
}
