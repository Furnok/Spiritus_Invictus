using UnityEngine;

public class S_PlayerProjectileManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int initialPoolSize;

    [Header("References")]
    [SerializeField] private Transform spawnProjectileParent;
    [SerializeField] private S_PlayerProjectile projectilePrefab;

    [Header("Input")]
    [SerializeField] private RSE_OnNewTargeting rseOnNewTargeting;
    [SerializeField] private RSE_OnPlayerCancelTargeting rseOnPlayerCancelTargeting;
    [SerializeField] private RSE_OnDespawnProjectile rseOnDespawnProjectile;
    [SerializeField] private RSE_OnSpawnProjectile rseOnSpawnProjectile;

    private S_ObjectPool<S_PlayerProjectile> projectilePool = null;
    private Transform target = null;

    private void Awake()
    {
        projectilePool = new S_ObjectPool<S_PlayerProjectile>(projectilePrefab, initialPoolSize, spawnProjectileParent);
    }

    private void OnEnable()
    {
        rseOnNewTargeting.action += ChangeNewTargt;
        rseOnPlayerCancelTargeting.action += CancelTarget;
        rseOnDespawnProjectile.action += ReturnProjectileToPool;
        rseOnSpawnProjectile.action += GetProjectileFromPool;
    }

    private void OnDisable()
    {
        rseOnNewTargeting.action -= ChangeNewTargt;
        rseOnPlayerCancelTargeting.action -= CancelTarget;
        rseOnDespawnProjectile.action -= ReturnProjectileToPool;
        rseOnSpawnProjectile.action -= GetProjectileFromPool;
    }

    private void ChangeNewTargt(GameObject newTarget)
    {
        target = newTarget.transform;
    }

    private void CancelTarget(GameObject oldTarget)
    {
        target = null;
    }

    private void GetProjectileFromPool(S_StructProjectileData projectileInitializeData)
    {
        var projectile = projectilePool.Get();
        projectile.transform.position = projectileInitializeData.locationSpawn;
        projectile.Initialize(target, projectileInitializeData.direction);
    }

    private void ReturnProjectileToPool(S_PlayerProjectile projectile)
    {
        projectilePool.ReturnToPool(projectile);
    }
}