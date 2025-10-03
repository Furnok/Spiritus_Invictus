using UnityEngine;
using UnityEngine.Events;

public class S_EnemyHealth : MonoBehaviour
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TakeDamage(50);
        }
    }

    private void TakeDamage(float damage)
    {
        enemyHealth -= damage;
        onUpdateEnemyHealth.Invoke(enemyHealth);
    }
}