using UnityEngine;

[CreateAssetMenu(fileName = "RSO_PlayerRespawnPosition", menuName = "Data/RSO/Player/RSO_PlayerRespawnPosition")]
public class RSO_PlayerRespawnPosition : BT.ScriptablesObject.RuntimeScriptableObject<PositionAndRotation> {}

[System.Serializable]
public class PositionAndRotation
{
    public Vector3 position;
    public Quaternion rotation;
}