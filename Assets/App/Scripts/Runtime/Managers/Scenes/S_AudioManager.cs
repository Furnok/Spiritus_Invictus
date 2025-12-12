using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

public class S_AudioManager : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Audios")]
    [SerializeField] private EventReference music;

    [TabGroup("References")]
    [SerializeField] private EventReference wind;

    private FMOD.Studio.EventInstance musicInstance;
    private FMOD.Studio.EventInstance windInstance;

    private void Start()
    {
        musicInstance = RuntimeManager.CreateInstance(music);
        windInstance = RuntimeManager.CreateInstance(wind);

        musicInstance.start();
        windInstance.start();
    }

    private void OnDisable()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        windInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        musicInstance.release();
        windInstance.release();
    }
}