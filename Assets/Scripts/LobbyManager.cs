using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] NetworkRunner networkRunner;

    [SerializeField] private GameObject sessionPanel;
    [SerializeField] private Button startSessionButton;
    [SerializeField] private Button endSessionButton;

    void Start()
    {
        networkRunner.AddCallbacks(this);
        endSessionButton.interactable = false;
    }

    public void StartSession()
    {
        networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = "GameID",
            OnGameStarted = OnGameStarted
        });
    }

    private void OnGameStarted(NetworkRunner runner)
    {
        Debug.Log("Game started");

        startSessionButton.interactable = false;
        endSessionButton.interactable = true;
    }

    public void EndSession()
    {
        if (networkRunner.IsRunning)
        {
            networkRunner.Shutdown();
        }

        startSessionButton.interactable = true;
        endSessionButton.interactable = false;
    }

    public async void JoinLobby()
    {
        StartGameResult result = await networkRunner.JoinSessionLobby(SessionLobby.Custom, "MainLobby");

        if (result.Ok)
        {
            Debug.Log($"Joined Lobby {networkRunner.LobbyInfo.Name}");
        }
    }

    public async void JoinLobby(string lobbyName)
    {
        StartGameResult result = await networkRunner.JoinSessionLobby(SessionLobby.Custom, lobbyName);

        if (result.Ok)
        {
            Debug.Log($"Joined Lobby {networkRunner.LobbyInfo.Name}");
        }
    }

    public async void JoinLobby(TextMeshProUGUI text)
    {
        StartGameResult result = await networkRunner.JoinSessionLobby(SessionLobby.Custom, text.text);

        if (result.Ok)
        {
            Debug.Log($"Joined Lobby {networkRunner.LobbyInfo.Name}");
        }
    }

    private void RefreshRoomUI()
    {
        if (networkRunner.IsRunning && !networkRunner.IsShutdown)
        {
            sessionPanel.SetActive(true);
        }
        else
        {
            sessionPanel.SetActive(false);
        }
    }

    #region RunnerCallBacks
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }
    #endregion

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        bool isLocalPlayer = false;

        if (networkRunner.LocalPlayer == player) 
            isLocalPlayer = true;

        Debug.Log($"Player {player.PlayerId} joined, localPlayer: {isLocalPlayer}");
        RefreshRoomUI();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {

        Debug.Log($"{player.PlayerId} left");
        RefreshRoomUI();

    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        RefreshRoomUI();
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Connected to server and lobby successfully!");
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log($"Session list updated. Found {sessionList.Count} sessions.");

        foreach (var session in sessionList)
        {
            Debug.Log($"Session Name: {session.Name}, Player Count: {session.PlayerCount}");
        }
    }
}
