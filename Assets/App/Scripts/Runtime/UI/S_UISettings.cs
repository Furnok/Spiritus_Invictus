using UnityEngine;

public class S_UISettings : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject defaultPanelSet;

    private GameObject currentPanelSet;

    private void OnEnable()
    {
        if (defaultPanelSet != null)
        {
            defaultPanelSet.SetActive(true);
            currentPanelSet = defaultPanelSet;
        }
    }

    private void OnDisable()
    {
        if (currentPanelSet != null)
        {
            currentPanelSet.SetActive(false);
            currentPanelSet = null;
        }
    }

    private void ClosePanel()
    {
        if (currentPanelSet != null)
        {
            currentPanelSet.SetActive(false);
        }
    }

    public void OpenPanel(GameObject panelSet)
    {
        if (currentPanelSet != panelSet)
        {
            ClosePanel();

            panelSet.SetActive(true);
            currentPanelSet = panelSet;
        }
    }
}