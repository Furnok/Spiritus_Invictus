using Sirenix.OdinInspector;
using UnityEngine;

public class S_Extract : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Filter")]
    [SerializeField] [S_TagName] private string tagPlayer;

    [TabGroup("Settings")]
    [Title("Extract")]
    [SerializeField] private int extractIndex;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerInteractInput rseOnPlayerInteract;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnOpenExtractWindow rseOnOpenExtractWindow;

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
        rseOnOpenExtractWindow.Call(extractIndex);
    }
}