using UnityEngine;

public class S_PlayerCameraLook : MonoBehaviour
{
    //[Header("Settings")]

    //[Header("References")]

    //[Header("Input")]

    [Header("Output")]
    [SerializeField] RSE_OnPlayerCameraLook _onPlayerCameraLook;

    private void Start()
    {
        _onPlayerCameraLook.Call(transform);
    }
}