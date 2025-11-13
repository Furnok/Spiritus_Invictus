using UnityEngine;
using UnityEngine.Events;

public class S_EnemyHurt : MonoBehaviour, I_Damageable
{
    [HideInInspector] public UnityEvent<float> onUpdateEnemyHealth;

    public void TakeDamage(float damage)
    {
        onUpdateEnemyHealth.Invoke(damage);
    }
}