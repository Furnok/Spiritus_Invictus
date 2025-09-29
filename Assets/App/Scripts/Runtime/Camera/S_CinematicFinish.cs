using UnityEngine;

public class S_CinematicFinish : MonoBehaviour
{
    [Header("Output")]
    [SerializeField] private RSE_CinematicFinish rseCinematicFinish;

    public void FinishCinematic()
    {
        rseCinematicFinish.Call();
    }
}