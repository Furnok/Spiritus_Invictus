using UnityEngine;

public class S_PlayerParry : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private RSE_OnPlayerParryInput rseOnPlayerParry;

    private void OnEnable()
    {
        rseOnPlayerParry.action += Parry;
    }

    private void OnDisable()
    {
        rseOnPlayerParry.action -= Parry;
    }

    private void Parry()
    {

    }
}