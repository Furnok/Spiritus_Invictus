using Sirenix.OdinInspector;
using UnityEngine;

public class S_CinematicFinish : MonoBehaviour
{
    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnCinematicFinish rseOnCinematicFinish;

    public void FinishCinematic()
    {
        rseOnCinematicFinish.Call();
    }
}