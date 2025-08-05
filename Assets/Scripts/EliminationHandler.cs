
using Fusion;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EliminationHandler : NetworkBehaviour
{
    private Dictionary<PlayerRef, bool> playersAlive;
    public override void Spawned()
    {
        base.Spawned();
        if (Runner.IsServer)
        {
            playersAlive = new Dictionary<PlayerRef, bool>();
            foreach (var player in Runner.ActivePlayers)
                playersAlive[player] = true;

            playersAlive[Runner.LocalPlayer] = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Runner.IsServer) 
        {
            if (collision.gameObject.tag == "Player")
            {
                NetworkObject playerNO = collision.gameObject.GetComponent<NetworkObject>();
                playersAlive[playerNO.InputAuthority] = false;
                Runner.Despawn(playerNO);
                if (playersAlive.Count(a => a.Value == true) == 1)
                {
                    GameManager.Instance.PlayerWon(playersAlive.FirstOrDefault(a => a.Value == true).Key);
                }
            }
        }
    }
}
