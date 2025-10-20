using UnityEngine;
using UnityEngine.Events;

public class S_EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private SSO_EnemyHealth ssoEnemyHealthMax;

    [Header("Input")]
    [HideInInspector] public UnityEvent<float> onUpdateEnemyHealth;
    [HideInInspector] public UnityEvent<float> onInitializeEnemyHealth;

    private float enemyHealth = 0;

    private void Start()
    {
        enemyHealth = ssoEnemyHealthMax.Value;
        onInitializeEnemyHealth.Invoke(enemyHealth);
    }

    public void TakeDamage(float damage)
    {
        enemyHealth -= damage;
        onUpdateEnemyHealth.Invoke(enemyHealth);
    }
}