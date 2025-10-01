using UnityEngine;

public class S_PlayerDamageSystem : MonoBehaviour
{
    //[Header("Settings")]

    //[Header("References")]

    [Header("Input")]
    [SerializeField] RSE_OnPlayerTakeDamage _onPlayerTakeDamage;

    [Header("Output")]
    [SerializeField] RSE_OnPlayerHealthReduced _onPlayerHealthReduced;

    private void OnEnable()
    {
        _onPlayerTakeDamage.action += TakeDamage;
    }
    
    private void OnDisable()
    {
        _onPlayerTakeDamage.action -= TakeDamage;
    }

    void TakeDamage(float damage)
    {
        _onPlayerHealthReduced.Call(damage);
        Debug.Log($"Player Took Damage: {damage}");
    }

}