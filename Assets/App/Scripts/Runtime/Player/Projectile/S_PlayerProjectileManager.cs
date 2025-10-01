using UnityEngine;

public class S_PlayerProjectileManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int _initialPoolSize = 10;

    [Header("References")]
    [SerializeField] Transform _spawnProjectileParent;
    [SerializeField] S_PlayerProjectile _projectilePrefab;

    [Header("Input")]
    [SerializeField] RSE_OnNewTargeting _onNewTargeting;
    [SerializeField] RSE_OnPlayerCancelTargeting _onPlayerCancelTargeting;
    [SerializeField] RSE_OnDespawnProjectile _onDespawnProjectile;
    [SerializeField] RSE_OnSpawnProjectile _onSpawnProjectile;


    [Header("Outputs")]


    S_ObjectPool<S_PlayerProjectile> _projectilePool;
    Transform _target;


    private void Awake()
    {
        _projectilePool = new S_ObjectPool<S_PlayerProjectile>(_projectilePrefab, _initialPoolSize, _spawnProjectileParent);
    }

    private void OnEnable()
    {
        _onNewTargeting.action += ChangeNewTargt;
        _onPlayerCancelTargeting.action += CancelTarget;
        _onDespawnProjectile.action += ReturnProjectileToPool;
        _onSpawnProjectile.action += GetProjectileFromPool;
    }

    private void OnDisable()
    {
        _onNewTargeting.action -= ChangeNewTargt;
        _onPlayerCancelTargeting.action -= CancelTarget;
        _onDespawnProjectile.action -= ReturnProjectileToPool;
        _onSpawnProjectile.action -= GetProjectileFromPool;
    }

    void ChangeNewTargt(GameObject newTarget)
    {
        _target = newTarget.transform;
    }
    void CancelTarget(GameObject oldTarget)
    {
        _target = null;
    }

    void GetProjectileFromPool(ProjectileInitializeData projectileInitializeData)
    {
        var projectile = _projectilePool.Get();
        projectile.transform.position = projectileInitializeData.locationSpawn;
        projectile.Initialize(_target, projectileInitializeData.direction);
    }

    void ReturnProjectileToPool(S_PlayerProjectile projectile)
    {
        _projectilePool.ReturnToPool(projectile);
    }
}