using Fusion;
using System.Collections;
using TMPro;
using UnityEngine;
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

    [SerializeField] Transform[] spawnPoints;
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
        ReadyText.text = $"0/{networkRunner.SessionInfo.PlayerCount} ready";


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
        if (networkRunner.IsSharedModeMasterClient)
            await networkRunner.SpawnAsync(readyManager);
    }

    private async void MaxPlayersReady()
    {
        if (networkRunner.IsSharedModeMasterClient)
        {
            SendChatMessage(-1, "ALL PLAYERS ARE READY");
            for (int i = 0; i < networkRunner.SessionInfo.PlayerCount; i++)
            {
                var temp = await networkRunner.SpawnAsync(playerPrefab, spawnPoints[i].position, spawnPoints[i].rotation);
                temp.GetComponent<Renderer>().material = characterSelection.UIColors[characterSelection.selectedColors[i]];
            }
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
}
