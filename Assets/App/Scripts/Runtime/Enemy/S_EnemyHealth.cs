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

    //[Header("Output")]
    private void Awake()
    {
        enemyHealth = enemyHealthMax.Value;
        onUpdateEnemyHealth.Invoke(enemyHealth);
    }
    void TakeDamage(float damage)
    {
        enemyHealth -= damage;
        onUpdateEnemyHealth.Invoke(enemyHealth);
    }
}