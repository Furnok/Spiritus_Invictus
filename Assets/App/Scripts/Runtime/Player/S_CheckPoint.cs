using UnityEngine;
using Sirenix.OdinInspector;

public class S_CheckPoint : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Filter")]
    [SerializeField][S_TagName] private string tagPlayer;

    //[TabGroup("Settings")]

    [TabGroup("References")]
    [Title("Content")]
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject _newSpawnPositionAndRotation;

    [Title("RSO")]
    [SerializeField] RSO_PlayerRespawnPosition _playerRespawnPosition;


    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerInteractInput rseOnPlayerInteract;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnOpenExtractWindow rseOnOpenExtractWindow;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnSendConsoleMessage rseOnSendConsoleMessage;


    bool isActivated = false;

    private void OnDisable()
    {
        rseOnPlayerInteract.action -= SaveInteract;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagPlayer) && isActivated == false)
        {
            content.SetActive(true);
            rseOnPlayerInteract.action += SaveInteract;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagPlayer))
        {
            content.SetActive(false);
            rseOnPlayerInteract.action -= SaveInteract;
        }
    }

    private void SaveInteract()
    {
        if (isActivated) return;
        isActivated = true;

        rseOnPlayerInteract.action -= SaveInteract;
        content.SetActive(false);
        _playerRespawnPosition.Value.position = _newSpawnPositionAndRotation.transform.position;
        _playerRespawnPosition.Value.rotation = _newSpawnPositionAndRotation.transform.rotation;
        rseOnSendConsoleMessage.Call("Player Interact with " + gameObject.name + "!");
        rseOnSendConsoleMessage.Call("Checkpoint activated, new pose respawn: " + _newSpawnPositionAndRotation.transform.position + "!");
    }
}