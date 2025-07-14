using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class HandleStartButtonBehavior : MonoBehaviour
{
    [SerializeField] private Button startButton;

    public void HandleChangeInSession(NetworkRunner nr)
    {
        if (nr.IsSceneAuthority && nr.SessionInfo.PlayerCount > 1)
            startButton.interactable = true;
        else
            startButton.interactable = false;
    }

    public async void StartGame(NetworkRunner nr)
    {
        if (nr.IsSceneAuthority && nr.SessionInfo.PlayerCount > 1)
        {
            await nr.LoadScene("GameScene");
        }
    }
}
