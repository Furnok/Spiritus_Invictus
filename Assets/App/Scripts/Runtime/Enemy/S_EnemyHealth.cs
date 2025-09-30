using UnityEngine;
using UnityEngine.Events;

public class S_EnemyHealth : MonoBehaviour
{
    //[Header("Settings")]
    private float enemyHealth;

    [Header("References")]
    [SerializeField] SSO_EnemyHealth enemyHealthMax;

    [Header("Input")]
    [HideInInspector] public UnityEvent<float> onUpdateEnemyHealth;
    [HideInInspector] public UnityEvent<float> onInitializeEnemyHealth;

    //[Header("Output")]
    private void Start()
    {
        enemyHealth = enemyHealthMax.Value;
        onInitializeEnemyHealth.Invoke(enemyHealth);
    }
    void TakeDamage(float damage)
    {
        enemyHealth -= damage;
        onUpdateEnemyHealth.Invoke(enemyHealth);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TakeDamage(50);
        }
    }
}