using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class S_EnemyProjectileManager : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Pool")]
    [SerializeField] private int initialPoolSize;

    [TabGroup("References")]
    [Title("Transforms")]
    [SerializeField] private Transform spawnProjectile;

    [TabGroup("References")]
    [SerializeField] private Transform poolProjectileParent;

    [TabGroup("References")]
    [Title("Script Prefab")]
    [SerializeField] private S_EnemyProjectile projectilePrefab;

    [TabGroup("References")]
    [Title("Script")]
    [SerializeField] private S_EnemyDetectionRange S_EnemyRangeDetection;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnDespawnEnemyProjectile RSE_OnDespawnEnemyProjectile;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnSpawnEnemyProjectile RSE_OnSpawnEnemyProjectile;

    private S_ObjectPool<S_EnemyProjectile> projectilePool = null;
    private Transform target = null;
    [HideInInspector] public UnityEvent<Transform> onSpawnProjectile;
    private void Awake()
    {
        projectilePool = new S_ObjectPool<S_EnemyProjectile>(projectilePrefab, initialPoolSize, poolProjectileParent);
    }

    private void OnEnable()
    {
        S_EnemyRangeDetection.onTargetDetected.AddListener(GetTarget);

        RSE_OnDespawnEnemyProjectile.action += ReturnProjectileToPool;
        RSE_OnSpawnEnemyProjectile.action += GetProjectileFromPool;

        onSpawnProjectile.AddListener(GetProjectileFromPool);
    }

    private void OnDisable()
    {
        S_EnemyRangeDetection.onTargetDetected.RemoveListener(GetTarget);

        RSE_OnDespawnEnemyProjectile.action -= ReturnProjectileToPool;
        RSE_OnSpawnEnemyProjectile.action -= GetProjectileFromPool;

        onSpawnProjectile.RemoveListener(GetProjectileFromPool);
    }

    private void GetTarget(GameObject newTarget)
    {
        if (newTarget != null && newTarget.TryGetComponent<IAimPointProvider>(out IAimPointProvider aimPoitProvider))
        {
            target = aimPoitProvider != null ? aimPoitProvider.GetAimPoint() : newTarget.transform;
        }
    }

    private void GetProjectileFromPool(Transform owner)
    {
        var projectile = projectilePool.Get();
        projectile.transform.position = spawnProjectile.position;
        projectile.transform.rotation = spawnProjectile.rotation;
        projectile.Initialize(owner, target);
    }

    private void ReturnProjectileToPool(S_EnemyProjectile projectile)
    {
        projectilePool.ReturnToPool(projectile);
    }
}