using Fusion;
using TMPro;
using UnityEngine;

public class ChatHandler : NetworkBehaviour
{
    [Rpc]
    public void RPC_SendChatMessageAll(int sender, string messsage, RpcInfo info = default)
    {
        GetComponent<TextMeshProUGUI>().text += $"[{(sender == -1 ? "SERVER" : $"Player {sender + 1}")}]: {messsage}\n";
    }

    [Rpc]
    public void RPC_SendChatMessage(int sender, int target, string messsage, RpcInfo info = default)
    {
        if (Object.Runner.LocalPlayer.AsIndex == target+1)
            GetComponent<TextMeshProUGUI>().text += $"[{(sender == -1 ? "SERVER" : $"Player {sender + 1}")}]: {messsage}\n";
    }
}
