using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] protected GameObject mainPanel;
    [SerializeField] protected GameObject hostPanel;
    [SerializeField] protected GameObject joinPanel;
    [SerializeField] protected GameObject sessionsPanel;

    private GameObject visiblePanel;

    private void Start()
    {
        mainPanel.SetActive(true);
        hostPanel.SetActive(false);
        joinPanel.SetActive(false);
        sessionsPanel.SetActive(false);

        visiblePanel = mainPanel;
    }

    public void SwitchPanel(GameObject targetPanel)
    {
        visiblePanel.gameObject.SetActive(false);

        targetPanel.SetActive(true);
        visiblePanel = targetPanel;
    }

    public void SwitchPanel(int targetPanelIndex)
    {
        if (targetPanelIndex == 0) 
            SwitchPanel(mainPanel);
        else if (targetPanelIndex == 1)
            SwitchPanel(hostPanel);
        else if (targetPanelIndex == 2)
            SwitchPanel(joinPanel);
        else if (targetPanelIndex == 3)
            SwitchPanel(sessionsPanel);
    }
}
