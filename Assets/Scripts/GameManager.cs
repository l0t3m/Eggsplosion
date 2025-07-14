using Fusion;
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

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        networkRunner = FindFirstObjectByType<NetworkRunner>();
        SpawnReadyManagers();
        ReadyText.text = $"0/{networkRunner.SessionInfo.PlayerCount} ready";


    }

    private async void SpawnReadyManagers()
    {
        if (networkRunner.IsSharedModeMasterClient)
            await networkRunner.SpawnAsync(readyManager);

    }

    public void SendReady()
    {
        ReadyManagerInstance.RPC_SetReady();
        readyButton.interactable = false;
        
        //SendChatMessage(-1, $"Player {networkRunner.LocalPlayer.AsIndex} has readied up!"); 
    }

    public void SendChatMessage(int sender, string message)
    {
        chat.RPC_SendChatMessage(sender, message);
    }
}
