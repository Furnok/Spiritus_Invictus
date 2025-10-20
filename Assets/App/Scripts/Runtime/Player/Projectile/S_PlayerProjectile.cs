using UnityEngine;

public class S_PlayerProjectile : MonoBehaviour
{
    [Header("References")]
    [SerializeField] SSO_PlayerAttackSteps _playerAttackSteps;
    [SerializeField] private SSO_PlayerProjectileData ssoPlayerProjectileData;
    [SerializeField] SSO_PlayerStats _playerStats;
    //[SerializeField]  ;

    [Header("Output")]
    [SerializeField] private RSE_OnDespawnProjectile rseOnDespawnProjectile;

    private Transform target = null;
    private float timeAlive = 0f;
    private float speed;
    private float lifeTime => _playerStats.Value.projectileLifeTime;
    private float baseDamage => ssoPlayerProjectileData.Value.baseDamage;
    private Vector3 direction = Vector3.zero;
    private bool isInitialized = false;

    int _attackStep = 0;

    public void Initialize(Transform target = null, int attackStep = 0)
    {
        this.target = target;
        this.direction = transform.forward;
        _attackStep = attackStep;
        speed = _playerAttackSteps.Value.Find(x => x.step == attackStep).speed;
        isInitialized = true;

    }

    private void OnDisable()
    {
        isInitialized = false;
        timeAlive = 0f;
        target = null;
        direction = Vector3.zero;
    }

    private void Update()
    {
        if (!isInitialized) return;

        timeAlive += Time.deltaTime;
        if (timeAlive >= lifeTime)
        {
            rseOnDespawnProjectile.Call(this);
            return;
        }

        Vector3 dir;
        if (target != null)
        {
            dir = (target.position - transform.position).normalized;
        }
        else
        {
            dir = direction;
        }

        transform.position += dir * speed * Time.deltaTime;
        transform.forward = dir;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hurtbox" && other.TryGetComponent(out IDamageable damageable))
        {
            if (damageable != null)
            {

                var damage = baseDamage * _playerAttackSteps.Value.Find(x => x.step == _attackStep).multipliers;
                damageable.TakeDamage(damage);
                rseOnDespawnProjectile.Call(this);

                Debug.Log($"Hit enemy for {damage} damage.");

            }
        }
        else
        {
            rseOnDespawnProjectile.Call(this);
        }

    }
}