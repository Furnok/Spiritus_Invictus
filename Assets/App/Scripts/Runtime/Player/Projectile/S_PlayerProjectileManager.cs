using System.Linq;
using UnityEngine;

public class S_PlayerProjectileManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int initialPoolSize;

    [Header("References")]
    [SerializeField] private Transform spawnProjectileParent;
    [SerializeField] private S_PlayerProjectile projectilePrefab;
    [SerializeField] RSO_PlayerPosition _playerPosition;
    [SerializeField] RSO_PlayerRotation _playerRotation;
    [SerializeField] SSO_PlayerAttackSteps _playerAttackSteps;

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


    private void GetProjectileFromPool(float attackconviction)
    {
        if (attackconviction < 1) return;
        
        var stepsUpperCurrentConviction = _playerAttackSteps.Value.FindAll(x => x.ammountConvitionNeeded <= attackconviction);
        var currentStepAttack = stepsUpperCurrentConviction.OrderByDescending(x => x.ammountConvitionNeeded).First();

        var projectile = projectilePool.Get();
        projectile.transform.position = _playerPosition.Value + _playerRotation.Value * new Vector3(0,1.5f,0.5f);
        projectile.transform.rotation = _playerRotation.Value;

        Transform aimPoint = null;

        if (target != null)
        {
            target.TryGetComponent<I_AimPointProvider>(out I_AimPointProvider aimPointProvider);
            aimPoint = aimPointProvider != null ? aimPointProvider.GetAimPoint() : target;
        }

        projectile.Initialize(attackconviction * currentStepAttack.multipliers, aimPoint, currentStepAttack.step);
    }

    private void ReturnProjectileToPool(S_PlayerProjectile projectile)
    {
        projectilePool.ReturnToPool(projectile);
    }
}