using UnityEngine;
using UnityEngine.Events;

public class S_BossHurt : MonoBehaviour, I_Damageable
{
    [HideInInspector] public UnityEvent<float> onUpdateEnemyHealth = null;

    public void TakeDamage(float damage)
    {
        onUpdateEnemyHealth.Invoke(damage);
    }
}