using Sirenix.OdinInspector;
using UnityEngine;

public class S_CheckpointManager : MonoBehaviour
{
    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnDataLoad rseOnDataLoad;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_DataSaved rsoDataSaved;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_PlayerRespawnPosition rsoPlayerRespawnPosition;

    private void OnEnable()
    {
        rseOnDataLoad.action += LoadCheckpoint;
    }

    private void OnDisable()
    {
        rseOnDataLoad.action -= LoadCheckpoint;
    }

    private void LoadCheckpoint()
    {
        rsoPlayerRespawnPosition.Value.position = rsoDataSaved.Value.position;
        rsoPlayerRespawnPosition.Value.rotation = rsoDataSaved.Value.rotation;
    }
}