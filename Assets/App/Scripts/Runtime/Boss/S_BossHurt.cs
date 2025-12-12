using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class S_BossHurt : MonoBehaviour, I_Damageable
{
    [TabGroup("References")]
    [Title("Scripts")]
    [SerializeField] private S_Boss boss;

    public void TakeDamage(float damage)
    {
        boss.TakeDamage(damage);
    }
}