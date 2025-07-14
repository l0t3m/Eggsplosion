using Fusion;
using TMPro;
using UnityEngine;

public class ChatHandler : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    [Rpc]
    public void RPC_SendChatMessage(int sender, string messsage)
    {
        text.text += $"[{(sender == -1 ? "SERVER" : $"Player {sender + 1}")}: {messsage}\n";
    }
}
