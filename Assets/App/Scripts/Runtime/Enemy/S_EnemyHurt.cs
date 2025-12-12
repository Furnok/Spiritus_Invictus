using Sirenix.OdinInspector;
using UnityEngine;

public class S_EnemyHurt : MonoBehaviour, I_Damageable
{
    [TabGroup("References")]
    [Title("Scripts")]
    [SerializeField] private S_Enemy enemy;

    public void TakeDamage(float damage)
    {
        enemy.TakeDamage(damage);
    }
}