using Fusion;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SessionsManager : MonoBehaviour
{
    [SerializeField] InitializeSession sessionPrefab;
    [SerializeField] LobbyManager lobbyManager;
    [SerializeField] NetworkRunner networkRunner;
    [SerializeField] GameObject sessionsParent;

    private void Start()
    {
        lobbyManager.SessionsListUpdated += InitializeSessions;
    }

    public void InitializeSessions(List<SessionInfo> sessions)
    {
        foreach (SessionInfo session in sessions)
        {
            InitializeSession sessionobj = Instantiate<InitializeSession>(sessionPrefab);
            sessionobj.transform.parent = sessionsParent.transform;
            sessionobj.Initialize(session);

            sessionobj.OnJoinPressed += JoinSessionPressed;
        }
    }

    public void RefreshSessionsList()
    {

    }

    public void JoinSessionPressed(string sessionName)
    {
        lobbyManager.StartSession(sessionName);
    }
}
