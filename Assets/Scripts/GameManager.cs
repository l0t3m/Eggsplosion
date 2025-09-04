using Fusion;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private GameObject readyManagerGeneric;
    [SerializeField] private ReadyManager readyManager;
    private NetworkRunner networkRunner;

    public ReadyManager ReadyManagerInstance;
    [SerializeField] Button readyButton;
    public TextMeshProUGUI ReadyText;
    [SerializeField] ChatHandler chat;
    [SerializeField] CharacterSelection characterSelection;
    [SerializeField] GameObject WinPanel;

    [SerializeField] Transform[] spawnPointsLocations;
    [SerializeField] GameObject playerPrefab;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        networkRunner = FindFirstObjectByType<NetworkRunner>();
        StartCoroutine(SpawnReadyManagers());
        ReadyText.text = $"0/{networkRunner.SessionInfo.PlayerCount-1} ready";
    }

    private IEnumerator SpawnReadyManagers()
    {
        SpawnManager();
        while (ReadyManagerInstance == null)
            yield return null;
        ReadyManagerInstance.OnReadyCounterReachedMax += MaxPlayersReady;
    }

    private async void SpawnManager()
    {
        if (networkRunner.IsServer)
            await networkRunner.SpawnAsync(readyManager);
    }

    private async void MaxPlayersReady()
    {
        foreach (var pref in networkRunner.ActivePlayers)
        {
            var player = await networkRunner.SpawnAsync(playerPrefab, spawnPointsLocations[pref.PlayerId].position, spawnPointsLocations[pref.PlayerId].rotation, inputAuthority: pref);
            PlayerLogic pl = player.GetComponent<PlayerLogic>();
            Color clr = characterSelection.UIColors[characterSelection.selectedColors[pref.PlayerId - 1]].color;
            Debug.Log(clr);
            pl.RPC_ColorPlayer(clr);
        }
        if (networkRunner.IsServer)
        {
            SendChatMessage(-1, "ALL PLAYERS ARE READY");
            
            networkRunner.Despawn(ReadyText.GetComponent<NetworkObject>());
            networkRunner.Despawn(readyButton.GetComponent<NetworkObject>());
            networkRunner.Despawn(characterSelection.transform.parent.GetComponent<NetworkObject>());
        }
        
            
       
    }

    public void SendReady()
    {
        ReadyManagerInstance.RPC_SetReady();
        readyButton.interactable = false;
        
        SendChatMessage(-1, $"Player {networkRunner.LocalPlayer.AsIndex} has readied up! [{characterSelection.UIColors[characterSelection.selectedColors[networkRunner.LocalPlayer.AsIndex-1]].name}]"); 
    }

    public void SendChatMessage(int sender, string message)
    {
        chat.RPC_SendChatMessageAll(sender, message);
    }

    public void PlayerWon(PlayerRef pRef)
    {
        SendChatMessage(-1, $"{pRef.PlayerId} has won!");
        WinPanel.SetActive(true);
    }

    public void EndGame()
    {
        if (networkRunner.IsServer)
            networkRunner.LoadScene("MainMenuScene");
    }

    public void PlayerLost()
    {
        SceneManager.LoadScene(0);
    }

}
