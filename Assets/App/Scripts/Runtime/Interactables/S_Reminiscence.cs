using UnityEngine;

public class S_Reminiscence : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int cameraIndex;
    [S_TagName] [SerializeField] private string tagPlayer;

    [Header("Input")]
    [SerializeField] private RSE_OnPlayerInteract rseOnPlayerInteract;

    [Header("Output")]
    [SerializeField] private RSE_CameraCinematic rseCameraCinematic;

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
        rseCameraCinematic.Call(cameraIndex);
    }
}