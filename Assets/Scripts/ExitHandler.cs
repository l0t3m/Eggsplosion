using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitHandler : NetworkBehaviour
{
    
    public void ExitGame()
    {
        NetworkRunner runner = FindFirstObjectByType<NetworkRunner>();
        if (runner.IsSharedModeMasterClient)
        {
            runner.LoadScene("MainMenuScene");
        }
        else
            SceneManager.LoadScene("MainMenuScene");
    }
}
