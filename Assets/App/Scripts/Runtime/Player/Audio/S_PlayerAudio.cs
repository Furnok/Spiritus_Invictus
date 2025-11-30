using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

public class S_PlayerAudio : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Audio")]
    [SerializeField] private EventReference footstepsSound;

    public void PlayFootsteps()
    {
        RuntimeManager.PlayOneShot(footstepsSound);
    }
}