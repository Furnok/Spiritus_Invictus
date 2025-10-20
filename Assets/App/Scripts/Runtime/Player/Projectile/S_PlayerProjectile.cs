using UnityEngine;

public class S_PlayerProjectile : MonoBehaviour
{
    [Header("Output")]
    [SerializeField] private RSE_OnDespawnProjectile rseOnDespawnProjectile;
    [SerializeField] private SSO_PlayerProjectileData ssoPlayerProjectileData;

    private Transform target = null;
    private float timeAlive = 0f;
    private float speed => ssoPlayerProjectileData.Value.speed;
    private float lifeTime => ssoPlayerProjectileData.Value.lifeTime;
    private float baseDamage => ssoPlayerProjectileData.Value.baseDamage;
    private Vector3 direction = Vector3.zero;
    private bool isInitialized = false;

    public void Initialize(Transform target, Vector3 direction)
    {
        this.target = target;
        this.direction = direction.normalized;
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
                damageable.TakeDamage(baseDamage);
            }
            Debug.Log($"Hit enemy for {baseDamage} damage.");
        }

        rseOnDespawnProjectile.Call(this);
    }
}