using UnityEngine;

public class S_Extract : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] [S_TagName] private string tagPlayer;

    [Header("Input")]
    [SerializeField] private RSE_OnPlayerInteractInput rseOnPlayerInteract;

    [Header("Output")]
    [SerializeField] private SSO_ExtractText ssoExtractText;
    [SerializeField] private RSE_OnOpenExtractWindow rseOnOpenExtractWindow;
    [SerializeField] private RSE_OnDisplayExtract rseOnDisplayExtract;

    private void OnDisable()
    {
        rseOnPlayerInteract.action -= ExtractInteract;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagPlayer))
        {
            rseOnPlayerInteract.action += ExtractInteract;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagPlayer))
        {
            rseOnPlayerInteract.action -= ExtractInteract;
        }
    }

    private void ExtractInteract()
    {
        rseOnOpenExtractWindow.Call();
        StartCoroutine(S_Utils.DelayFrame(() => rseOnDisplayExtract.Call(ssoExtractText.Value)));
    }
}