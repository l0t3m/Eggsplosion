using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner networkRunner;

    [SerializeField] private GameObject sessionPanel;
    [SerializeField] private Button startSessionButton;
    [SerializeField] private TextMeshProUGUI maxPlayersAllowed;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private HandleStartButtonBehavior startBehavior;
    [SerializeField] private Toggle publicToggleField;
    [SerializeField] private TextMeshProUGUI privacyText;
    [SerializeField] private HandleDisconnect disconnectionPanel;
    //[SerializeField] private DisableComponentIfNotSceneAuthority[] objectsToDisable;

    public event Action<List<SessionInfo>> SessionsListUpdated;

    void Start()
    {
        networkRunner = NetworkRunnerSpawner.SpawnNetworkRunner();
        networkRunner.AddCallbacks(this);
#if UNITY_SERVER
        StartSessionServer();
#endif
    }

    private void StartSessionServer()
    {
        System.Random rnd = new System.Random();
        networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Server,
            SessionName = rnd.Next().ToString(),
            OnGameStarted = OnGameStarted,
            CustomLobbyName = "EU",
            PlayerCount = 2,
            IsVisible = true
        });
    }

    public async void StartSession(TextMeshProUGUI text)
    {
        var res = await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Client,
            SessionName = text.text,
            OnGameStarted = OnGameStarted,
        });
        if (!res.Ok)
            PlayerDisconnected(res.ShutdownReason.ToString());
    }

    public async void StartSession(string text)
    {
        var res = await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Client,
            SessionName = text,
            OnGameStarted = OnGameStarted,
        });
        if (!res.Ok)
            PlayerDisconnected(res.ShutdownReason.ToString());
    }

    private void OnGameStarted(NetworkRunner runner)
    {
        Debug.Log($"Game started, session name: {runner.SessionInfo.Name}");

        uiManager.SwitchPanel(3);
    }

    public void EndSession()
    {
        if (networkRunner.IsRunning)
        {
            networkRunner.Shutdown();
        }

        uiManager.SwitchPanel(2);

        Destroy(networkRunner.gameObject);
        networkRunner = NetworkRunnerSpawner.SpawnNetworkRunner();

        startSessionButton.interactable = true;
    }

    public async void JoinLobbyAsHost(string lobbyName)
    {
        StartGameResult result = await networkRunner.JoinSessionLobby(SessionLobby.Custom, lobbyName);

        if (result.Ok)
        {
            uiManager.SwitchPanel(1);
            Debug.Log($"Joined Lobby: {networkRunner.LobbyInfo.Name}");
        }
    }

    public void JoinLobbyAsHost(TextMeshProUGUI text)
    {
        JoinLobbyAsHost(text.text);
    }

    public async void JoinLobbyAsGuest(string lobbyName)
    {
        StartGameResult result = await networkRunner.JoinSessionLobby(SessionLobby.Custom, lobbyName);

        if (result.Ok)
        {
            uiManager.SwitchPanel(2);
            Debug.Log($"Joined Lobby: {networkRunner.LobbyInfo.Name}");
        }
    }

    public void JoinLobbyAsGuest(TextMeshProUGUI text)
    {
        JoinLobbyAsGuest(text.text);
    }

    public void StartGame()
    {
        startBehavior.StartGame(networkRunner);
    }

    private void RefreshRoomUI()
    {
        uiManager.UpdatePlayersConnectedText(networkRunner.SessionInfo.PlayerCount-1);
        startBehavior.HandleChangeInSession(networkRunner);
        //if (networkRunner.IsRunning && !networkRunner.IsShutdown)
        //{
        //    sessionPanel.SetActive(true);
        //}
        //else
        //{
        //    sessionPanel.SetActive(false);
        //}
    }

    #region RunnerCallBacks
   

    

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
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

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        PlayerDisconnected(reason.ToString());
        Destroy(networkRunner.gameObject);
        networkRunner = NetworkRunnerSpawner.SpawnNetworkRunner();
    }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        PlayerDisconnected(reason.ToString());
        Destroy(networkRunner.gameObject);
        networkRunner = NetworkRunnerSpawner.SpawnNetworkRunner();
    }

    private void PlayerDisconnected(string reason)
    {
        disconnectionPanel.gameObject.SetActive(true);
        disconnectionPanel.OnDisconnect(reason);
        Destroy(networkRunner.gameObject);
        networkRunner = NetworkRunnerSpawner.SpawnNetworkRunner();
    }
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
#if UNITY_SERVER
        if (runner.SessionInfo.PlayerCount == runner.SessionInfo.MaxPlayers)
            StartGame();
#endif
        RefreshRoomUI();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {

        Debug.Log($"{player.PlayerId} left");
        RefreshRoomUI();
        if (player.PlayerId == networkRunner.LocalPlayer.PlayerId)
        {
            Destroy(networkRunner.gameObject);
            networkRunner = NetworkRunnerSpawner.SpawnNetworkRunner();
        }
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        RefreshRoomUI();
        Destroy(networkRunner.gameObject);
        networkRunner = NetworkRunnerSpawner.SpawnNetworkRunner();
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
            Debug.Log($"Session Name: {session.Name}, Player Count: {session.PlayerCount-1}");
        }

        SessionsListUpdated?.Invoke(sessionList);
    }
}
