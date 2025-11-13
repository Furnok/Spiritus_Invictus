using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

public class S_PlayerAudio : MonoBehaviour
{
    //[Header("Settings")]

    [TabGroup("References")]
    [SerializeField] private EventReference footstepsSound;

    //[Header("Inputs")]

    //[Header("Outputs")]

    public void PlayFootsteps()
    {
        RuntimeManager.PlayOneShot(footstepsSound);
    }
}