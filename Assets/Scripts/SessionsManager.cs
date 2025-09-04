using Fusion;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SessionsManager : MonoBehaviour
{
    [SerializeField] InitializeSession sessionPrefab;
    [SerializeField] LobbyManager lobbyManager;
    [SerializeField] GameObject sessionsParent;

    const string JOIN_STR = "Joining...";
    private void Start()
    {
        lobbyManager.SessionsListUpdated += InitializeSessions;
    }

    public void InitializeSessions(List<SessionInfo> sessions)
    {
        for (int i = 0; i < sessionsParent.transform.childCount; i++)
            Destroy(sessionsParent.transform.GetChild(i).gameObject);

        foreach (SessionInfo session in sessions)
        {
            if (session.IsValid && session.IsOpen && sessions.Where(si => si.Name == session.Name).Count() <= 1)
            {
                InitializeSession sessionobj = Instantiate(sessionPrefab);
                sessionobj.transform.parent = sessionsParent.transform;
                sessionobj.Initialize(session);

                sessionobj.OnJoinPressed += JoinSessionPressed;
                sessionobj.OnJoinPressed += (btn) => { sessionobj.joinText.text = JOIN_STR; };
            }
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
