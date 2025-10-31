using UnityEngine;
using UnityEngine.Events;

public class S_EnemyHealth : MonoBehaviour, IDamageable
{

    [SerializeField] GameObject enemyBody;
    [Header("Input")]
    [HideInInspector] public UnityEvent<float> onUpdateEnemyHealth;
    [HideInInspector] public UnityEvent<float> onInitializeEnemyHealth;
    [Header("Output")]
    [SerializeField] RSE_OnEnemyTargetDied RSE_OnEnemyTargetDied;
    private float enemyHealth = 0;

    [Header("References")]
    [SerializeField] private SSO_EnemyHealth ssoEnemyHealthMax;

    private void Start()
    {
        enemyHealth = ssoEnemyHealthMax.Value;
        onInitializeEnemyHealth.Invoke(enemyHealth);
    }

    public void TakeDamage(float damage)
    {
        enemyHealth -= damage;
        onUpdateEnemyHealth.Invoke(enemyHealth);
        if(enemyHealth <= 0)
        {
            RSE_OnEnemyTargetDied.Call(enemyBody);
        }
    }
}