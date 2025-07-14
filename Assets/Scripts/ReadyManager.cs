using Fusion;
using System;
using UnityEngine;

public class ReadyManager : NetworkBehaviour
{
    public event Action OnReadyCounterReachedMax;
    public int readyCounter = 0;

    [Rpc]
    public void RPC_SetReady(RpcInfo info = default)
    {
        readyCounter++;
        int playerCount = FindFirstObjectByType<NetworkRunner>().SessionInfo.PlayerCount;
        GameManager.Instance.ReadyText.text = $"{readyCounter}/{playerCount} Ready";
        if (readyCounter >= playerCount)
            OnReadyCounterReachedMax?.Invoke();
        Debug.Log($"{info.Source.AsIndex} has readied up!");
    }

    public override void Spawned()
    {
        base.Spawned();
        StartCoroutine(WaitForGameManager());
    }

    private System.Collections.IEnumerator WaitForGameManager()
    {
        while (GameManager.Instance == null)
        {
            yield return null;
        }

        GameManager.Instance.ReadyManagerInstance = this;
    }
}
