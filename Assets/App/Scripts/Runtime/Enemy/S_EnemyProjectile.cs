using UnityEngine;

public class S_EnemyProjectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float projectileLifeTime;
    float projectileDamage;
    [SerializeField] float projectileSpeed;

    Transform enemyTarget = null;
    float timeAlive = 0f;
    Vector3 direction = Vector3.zero;
    bool isInitialized = false;

    //[Header("References")]

    [Header("Inputs")]
    [SerializeField] RSE_OnPlayerDeath RSE_OnPlayerDeath;
    [SerializeField] RSE_OnDespawnEnemyProjectile RSE_OnDespawnEnemyProjectile;
    //[Header("Outputs")]
    public void Initialize(float damage, Transform target = null)
    {
        enemyTarget = target;
        direction = transform.forward;
        projectileDamage = damage;
        isInitialized = true;

    }
    private void OnEnable()
    {
        RSE_OnPlayerDeath.action += OnTargetDie;
    }

    private void OnDisable()
    {
        isInitialized = false;
        timeAlive = 0f;
        enemyTarget = null;
        direction = Vector3.zero;

        RSE_OnPlayerDeath.action -= OnTargetDie;
    }
    void OnTargetDie()
    {
        if (enemyTarget != null)
        {
            enemyTarget = null;
        }
    }

    private void Update()
    {
        if (!isInitialized) return;

        timeAlive += Time.deltaTime;
        if (timeAlive >= projectileLifeTime)
        {
            RSE_OnDespawnEnemyProjectile.Call(this);
            return;
        }

        Vector3 dir;
        if (enemyTarget != null)
        {
            dir = (enemyTarget.position - transform.position).normalized;
        }
        else
        {
            dir = direction;
        }

        transform.position += dir * projectileSpeed * Time.deltaTime;
        transform.forward = dir;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hurtbox" && other.TryGetComponent(out IDamageable damageable))
        {
            if (damageable != null)
            {

                damageable.TakeDamage(projectileDamage);
                RSE_OnDespawnEnemyProjectile.Call(this);

                Debug.Log($"Hit enemy for {projectileDamage} damage.");

            }
        }
        else
        {
            RSE_OnDespawnEnemyProjectile.Call(this);
        }

    }
}