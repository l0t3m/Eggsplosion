using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class DisableComponentIfNotSceneAuthority : MonoBehaviour
{
    [SerializeField] Button buttonToDisable;

    public void Disable(NetworkRunner nr)
    {
        if (!nr.IsSceneAuthority)
            buttonToDisable.interactable = false;
    }
}
