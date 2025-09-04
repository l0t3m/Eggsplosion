using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitHandler : NetworkBehaviour
{
    public void ExitGame()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
