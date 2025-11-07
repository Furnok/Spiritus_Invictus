using Sirenix.OdinInspector;
using UnityEngine;

public class S_Reminiscence : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Filter")]
    [SerializeField] [S_TagName] private string tagName;

    [TabGroup("Settings")]
    [Title("Camera Cinematic")]
    [SerializeField] private int cameraIndex;

    [TabGroup("References")]
    [Title("Content")]
    [SerializeField] private GameObject content;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerInteractInput rseOnPlayerInteract;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnCameraCinematic rseOnCameraCinematic;

    private void OnValidate()
    {
        if (cameraIndex < 0)
        {
            cameraIndex = 0;
        }
    }

    private void OnDisable()
    {
        rseOnPlayerInteract.action -= Interract;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagName))
        {
            content.SetActive(true);
            rseOnPlayerInteract.action += Interract;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagName))
        {
            content.SetActive(false);
            rseOnPlayerInteract.action -= Interract;
        }
    }

    private void Interract()
    {
        rseOnCameraCinematic.Call(cameraIndex);
    }
}