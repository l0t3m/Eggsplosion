using Fusion;
using System;
using TMPro;
using UnityEngine;

public class InitializeSession : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI sessionName;
    [SerializeField] TextMeshProUGUI sessionAmount;

    public event Action<string> OnJoinPressed;

    public void Initialize(SessionInfo sessionInfo)
    {
        sessionName.text = sessionInfo.Name;
        sessionAmount.text = $"{sessionInfo.PlayerCount-1}/{sessionInfo.MaxPlayers-1}";
    }

    public void Join()
    {
        OnJoinPressed?.Invoke(sessionName.text);
    }
}
