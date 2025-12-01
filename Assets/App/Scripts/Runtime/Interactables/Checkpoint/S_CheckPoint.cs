using UnityEngine;
using Sirenix.OdinInspector;

public class S_CheckPoint : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Filter")]
    [SerializeField, S_TagName] private string tagPlayer;

    [TabGroup("Settings")]
    [Title("Saves")]
    [SerializeField, S_SaveName] private string saveName;

    [TabGroup("References")]
    [Title("Content")]
    [SerializeField] private GameObject content;

    [TabGroup("References")]
    [Title("Spawn")]
    [SerializeField] private GameObject newSpawnPositionAndRotation;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerInteractInput rseOnPlayerInteract;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnOpenExtractWindow rseOnOpenExtractWindow;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnSendConsoleMessage rseOnSendConsoleMessage;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnSaveData rseOnSaveData;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_PlayerRespawnPosition rsoplayerRespawnPosition;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_PlayerCurrentHealth rsoPlayerCurrentHealth;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_PlayerCurrentConviction rsoPlayerCurrentConviction;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_DataSaved rsoDataSaved;

    private void OnDisable()
    {
        rseOnPlayerInteract.action -= Checkpoint;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagPlayer))
        {
            content.SetActive(true);
            rseOnPlayerInteract.action += Checkpoint;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagPlayer))
        {
            content.SetActive(false);
            rseOnPlayerInteract.action -= Checkpoint;
        }
    }

    private void Checkpoint()
    {
        rsoplayerRespawnPosition.Value.position = newSpawnPositionAndRotation.transform.position;
        rsoplayerRespawnPosition.Value.rotation = newSpawnPositionAndRotation.transform.rotation;
        rseOnSendConsoleMessage.Call("Player Interact with " + gameObject.name + "!");
        rseOnSendConsoleMessage.Call("Checkpoint activated, new pose respawn: " + newSpawnPositionAndRotation.transform.position + "!");

        Save();
    }

    private void Save()
    {
        rsoDataSaved.Value.dateSaved = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        rsoDataSaved.Value.position = newSpawnPositionAndRotation.transform.position;
        rsoDataSaved.Value.rotation = newSpawnPositionAndRotation.transform.rotation;
        rsoDataSaved.Value.health = rsoPlayerCurrentHealth.Value;
        rsoDataSaved.Value.conviction = rsoPlayerCurrentConviction.Value;

        rseOnSaveData.Call(saveName, false);
    }
}