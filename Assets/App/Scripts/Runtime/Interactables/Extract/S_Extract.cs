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

    [TabGroup("References")]
    [Title("Content")]
    [SerializeField] private GameObject content;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerInteractInput rseOnPlayerInteract;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnOpenExtractWindow rseOnOpenExtractWindow;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnSendConsoleMessage rseOnSendConsoleMessage;

    private void OnDisable()
    {
        rseOnPlayerInteract.action -= ExtractInteract;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagPlayer))
        {
            content.SetActive(true);
            rseOnPlayerInteract.action += ExtractInteract;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagPlayer))
        {
            content.SetActive(false);
            rseOnPlayerInteract.action -= ExtractInteract;
        }
    }

    private void ExtractInteract()
    {
        rseOnSendConsoleMessage.Call("Player Interact with " + gameObject.name + "!");
        rseOnOpenExtractWindow.Call(extractIndex);
    }
}