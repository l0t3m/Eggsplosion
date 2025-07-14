using Fusion;
using TMPro;
using UnityEngine;

public class ChatHandler : NetworkBehaviour
{
    [Rpc]
    public void RPC_SendChatMessage(int sender, string messsage)
    {
        GetComponent<TextMeshProUGUI>().text += $"[{(sender == -1 ? "SERVER" : $"Player {sender + 1}")}]: {messsage}\n";
    }
}
