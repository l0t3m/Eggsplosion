using TMPro;
using UnityEngine;

public class HandleDisconnect : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI disconnectReasonText;

    public void OnDisconnect(string reason)
    {
        disconnectReasonText.text = "Disconnected: " + reason;

    }

    public void PressedOk()
    {
        gameObject.SetActive(false);
    }
}
