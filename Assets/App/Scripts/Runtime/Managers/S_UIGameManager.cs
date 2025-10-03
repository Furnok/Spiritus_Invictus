using UnityEngine;

public class S_UIGameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject extractCanvas;

    [Header("Input")]
    [SerializeField] private RSE_OnOpenExtractWindow rseOnOpenExtractWindow;

    [Header("Output")]
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