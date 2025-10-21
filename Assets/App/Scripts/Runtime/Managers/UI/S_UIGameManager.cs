using Sirenix.OdinInspector;
using UnityEngine;

public class S_UIGameManager : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Extract")]
    [SerializeField] private GameObject extractCanvas;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnOpenExtractWindow rseOnOpenExtractWindow;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnUIInputEnabled rseOnUIInputEnabled;

    private void OnEnable()
    {
        rseOnOpenExtractWindow.action += DiplayExtract;
    }

    private void OnDisable()
    {
        rseOnOpenExtractWindow.action -= DiplayExtract;
    }

    private void DiplayExtract()
    {
        rseOnUIInputEnabled.Call();

        extractCanvas.SetActive(true);
    }
}