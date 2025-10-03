using UnityEngine;

public class S_CinematicFinish : MonoBehaviour
{
    [Header("Output")]
    [SerializeField] private RSE_OnCinematicFinish rseOnCinematicFinish;

    public void FinishCinematic()
    {
        rseOnCinematicFinish.Call();
    }
}