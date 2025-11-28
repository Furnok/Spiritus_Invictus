using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;

public class S_PlayerProjectileManager : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Size")]
    [SerializeField] private int initialPoolSize;

    [TabGroup("References")]
    [Title("Spawn Point")]
    [SerializeField] private Transform spawnProjectileParent;

    [TabGroup("References")]
    [Title("Prefab")]
    [SerializeField] private S_PlayerProjectile projectilePrefab;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnNewTargeting rseOnNewTargeting;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerCancelTargeting rseOnPlayerCancelTargeting;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnDespawnProjectile rseOnDespawnProjectile;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnSpawnProjectile rseOnSpawnProjectile;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_PlayerPosition _playerPosition;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_PlayerRotation _playerRotation;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_PlayerAttackSteps _playerAttackSteps;

    private S_ObjectPool<S_PlayerProjectile> projectilePool = null;
    private Transform target = null;

    private void Awake()
    {
        projectilePool = new S_ObjectPool<S_PlayerProjectile>(projectilePrefab, initialPoolSize, spawnProjectileParent);
    }

    private void OnEnable()
    {
        rseOnNewTargeting.action += ChangeNewTarget;
        rseOnPlayerCancelTargeting.action += CancelTarget;
        rseOnDespawnProjectile.action += ReturnProjectileToPool;
        rseOnSpawnProjectile.action += GetProjectileFromPool;
    }

    private void OnDisable()
    {
        rseOnNewTargeting.action -= ChangeNewTarget;
        rseOnPlayerCancelTargeting.action -= CancelTarget;
        rseOnDespawnProjectile.action -= ReturnProjectileToPool;
        rseOnSpawnProjectile.action -= GetProjectileFromPool;
    }

    private void ChangeNewTarget(GameObject newTarget)
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