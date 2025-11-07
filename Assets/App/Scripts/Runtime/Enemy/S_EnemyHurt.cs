using UnityEngine;
using UnityEngine.Events;

public class S_EnemyHurt : MonoBehaviour, IDamageable
{
    [HideInInspector] public UnityEvent<float> onUpdateEnemyHealth;

    public void TakeDamage(float damage)
    {
        onUpdateEnemyHealth.Invoke(damage);
    }
}