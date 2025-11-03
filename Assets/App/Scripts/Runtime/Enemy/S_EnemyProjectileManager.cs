using Unity.VisualScripting;
using UnityEngine;

public class S_EnemyProjectileManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int initialPoolSize;
    S_ObjectPool<S_EnemyProjectile> projectilePool = null;
    Transform target = null;

    [Header("References")]
    [SerializeField] Transform spawnProjectile;
    [SerializeField] Transform poolProjectileParent;
    [SerializeField] S_EnemyProjectile projectilePrefab;
    [SerializeField] private S_EnemyDetectionRange S_EnemyRangeDetection;

    [Header("Inputs")]
    [SerializeField] private RSE_OnDespawnEnemyProjectile RSE_OnDespawnEnemyProjectile;
    [SerializeField] private RSE_OnSpawnEnemyProjectile RSE_OnSpawnEnemyProjectile;
    //[Header("Outputs")]

    private void Awake()
    {
        projectilePool = new S_ObjectPool<S_EnemyProjectile>(projectilePrefab, initialPoolSize, poolProjectileParent);
    }
    private void OnEnable()
    {
        S_EnemyRangeDetection.onTargetDetected.AddListener(GetTarget);
        RSE_OnDespawnEnemyProjectile.action += ReturnProjectileToPool;
        RSE_OnSpawnEnemyProjectile.action += GetProjectileFromPool;
    }
    private void OnDisable()
    {
        S_EnemyRangeDetection.onTargetDetected.RemoveListener(GetTarget);
    }
    private void GetTarget(GameObject newTarget)
    {
        if(newTarget != null)
        {
            target = newTarget.transform;
        }
    }
    private void GetProjectileFromPool(float damage)
    {
        var projectile = projectilePool.Get();
        projectile.transform.position = spawnProjectile.position;
        projectile.transform.rotation = spawnProjectile.rotation;
        projectile.Initialize(damage,target);
    }

    private void ReturnProjectileToPool(S_EnemyProjectile projectile)
    {
        projectilePool.ReturnToPool(projectile);
    }
}