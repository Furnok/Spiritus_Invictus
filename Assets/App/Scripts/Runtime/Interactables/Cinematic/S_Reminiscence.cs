using UnityEngine;

public class S_Reminiscence : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] [S_TagName] private string tagPlayer;
    [SerializeField] private int cameraIndex;

    [Header("Input")]
    [SerializeField] private RSE_OnPlayerInteractInput rseOnPlayerInteract;

    [Header("Output")]
    [SerializeField] private RSE_OnCameraCinematic rseOnCameraCinematic;

    private void OnDisable()
    {
        rseOnPlayerInteract.action -= Interract;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagPlayer))
        {
            rseOnPlayerInteract.action += Interract;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagPlayer))
        {
            rseOnPlayerInteract.action -= Interract;
        }
    }

    private void Interract()
    {
        rseOnCameraCinematic.Call(cameraIndex);
    }
}