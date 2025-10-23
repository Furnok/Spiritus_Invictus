using UnityEngine;

public class S_PlayerProjectile : MonoBehaviour
{
    [Header("References")]
    [SerializeField] SSO_PlayerAttackSteps _playerAttackSteps;
    [SerializeField] SSO_PlayerStats _playerStats;

    [Header("Input")]

    [SerializeField] private RSE_OnEnemyTargetDied _onEnemyTargetDied;

    //[SerializeField]  ;

    [Header("Output")]
    [SerializeField] private RSE_OnDespawnProjectile rseOnDespawnProjectile;

    private Transform target = null;
    private float timeAlive = 0f;
    private float speed;
    private float lifeTime => _playerStats.Value.projectileLifeTime;
    private float _damage;
    private Vector3 direction = Vector3.zero;
    private bool isInitialized = false;

    int _attackStep = 0;

    public void Initialize(float damage, Transform target = null, int attackStep = 0)
    {
        this.target = target;
        this.direction = transform.forward;
        _attackStep = attackStep;
        speed = _playerAttackSteps.Value.Find(x => x.step == attackStep).speed;
        _damage = damage;
        isInitialized = true;

    }

    private void OnEnable()
    {
        _onEnemyTargetDied.action += OnTargetDie;
    }

    private void OnDisable()
    {
        isInitialized = false;
        timeAlive = 0f;
        target = null;
        direction = Vector3.zero;

        _onEnemyTargetDied.action -= OnTargetDie;

    }

    void OnTargetDie(GameObject enemyDie)
    {
        if (target != null && enemyDie == target.gameObject && enemyDie != null)
        {
            target = null;
        }
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

                damageable.TakeDamage(_damage);
                rseOnDespawnProjectile.Call(this);

                Debug.Log($"Hit enemy for {_damage} damage.");

            }
        }
        else
        {
            rseOnDespawnProjectile.Call(this);
        }

    }
}