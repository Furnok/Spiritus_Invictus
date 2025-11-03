using Sirenix.OdinInspector;
using UnityEngine;

public class S_EnemyProjectile : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Projectile")]
    [SuffixLabel("s", Overlay = true)]
    [SerializeField] float projectileLifeTime;

    [TabGroup("Settings")]
    [SerializeField] float projectileSpeed;

    [TabGroup("Settings")]
    [SuffixLabel("s", Overlay = true)]
    [SerializeField] private float travelTime;

    [TabGroup("Settings")]
    [Header("Arc Settings")]
    [SerializeField] private float arcHeightMultiplier;

    [TabGroup("Settings")]
    [SerializeField] private float arcDirection;

    [TabGroup("Settings")]
    [SerializeField] private bool randomizeArc;

    [TabGroup("Settings")]
    [SerializeField, Range(-5, 5)] private float arcRandomDirectionMin;

    [TabGroup("Settings")]
    [SerializeField, Range(-5, 5)] private float arcRandomDirectionMax;

    [TabGroup("Inputs")]
    [SerializeField] RSE_OnPlayerDeath rseOnPlayerDeath;

    [TabGroup("Outputs")]
    [SerializeField] RSE_OnDespawnEnemyProjectile rseOnDespawnEnemyProjectile;

    private float projectileDamage = 0f;
    private Transform enemyTarget = null;
    private float timeAlive = 0f;
    private Vector3 direction = Vector3.zero;
    private bool isInitialized = false;
    private Vector3 startPos = Vector3.zero;
    private Vector3 controlPoint = Vector3.zero;
    private float speed = 0;

    private void Awake()
    {
        if (randomizeArc)
        {
            if (arcRandomDirectionMax < arcRandomDirectionMin)
            {
                float temp = arcRandomDirectionMax;
                arcRandomDirectionMax = arcRandomDirectionMin;
                arcRandomDirectionMin = temp;
            }
        }
    }

    private void OnEnable()
    {
        rseOnPlayerDeath.action += OnTargetDie;
    }

    private void OnDisable()
    {
        isInitialized = false;
        timeAlive = 0f;
        enemyTarget = null;
        direction = Vector3.zero;

        rseOnPlayerDeath.action -= OnTargetDie;
    }

    private void Update()
    {
        if (!isInitialized) return;

        timeAlive += Time.deltaTime;
        float t = timeAlive / travelTime;

        if (timeAlive >= projectileLifeTime)
        {
            rseOnDespawnEnemyProjectile.Call(this);
            return;
        }

        Vector3 endPos = enemyTarget != null ? enemyTarget.position : startPos + transform.forward * 10f;

        Vector3 a = Vector3.Lerp(startPos, controlPoint, t);
        Vector3 b = Vector3.Lerp(controlPoint, endPos, t);
        Vector3 newPos = Vector3.Lerp(a, b, t);
        Vector3 tangent = (b - a).normalized;


        if (enemyTarget != null && enemyTarget.gameObject.activeInHierarchy)
        {
            transform.position = newPos;
            transform.forward = tangent;

            direction = tangent;
        }
        else
        {
            transform.position += direction * speed * Time.deltaTime;
            transform.forward = direction;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hurtbox" && other.TryGetComponent(out IDamageable damageable))
        {
            if (damageable != null)
            {

                damageable.TakeDamage(projectileDamage);
                rseOnDespawnEnemyProjectile.Call(this);

                Debug.Log($"Hit enemy for {projectileDamage} damage.");

            }
        }
        else
        {
            rseOnDespawnEnemyProjectile.Call(this);
        }

    }

    public void Initialize(float damage, Transform target = null)
    {
        enemyTarget = target;
        direction = transform.forward;
        projectileDamage = damage;

        startPos = transform.position;

        Vector3 toTarget = target != null ? (target.position - startPos) : transform.forward * 10f;
        Vector3 midPoint = startPos + toTarget * 0.5f;

        Vector3 arcDir = Vector3.up;

        if (arcDirection != 0f || randomizeArc == true)
        {
            Vector3 side = Vector3.Cross(Vector3.up, toTarget.normalized).normalized;
            if (randomizeArc)
            {
                side *= Random.Range(arcRandomDirectionMin, arcRandomDirectionMax);
                arcDir = (Vector3.up + side).normalized;

            }
            else
            {
                arcDir = (Vector3.up + side * arcDirection).normalized;
            }
        }

        float arcHeight = toTarget.magnitude * 0.25f * arcHeightMultiplier;
        controlPoint = midPoint + arcDir * arcHeight;

        isInitialized = true;

    }

    private void OnTargetDie()
    {
        if (enemyTarget != null)
        {
            enemyTarget = null;
        }
    }
}