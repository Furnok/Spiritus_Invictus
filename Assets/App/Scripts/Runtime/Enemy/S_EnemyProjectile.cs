using DG.Tweening.Plugins.Core.PathCore;
using UnityEngine;

public class S_EnemyProjectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float projectileLifeTime;
    float projectileDamage;
    [SerializeField] float projectileSpeed;

    [SerializeField] private float _travelTime = 1f;

    [Header("Arc Settings")]
    [Tooltip("Arc height factor 1 = average, 2 = hight")]
    [SerializeField] private float _arcHeightMultiplier = 1f;
    [Tooltip("Curve direction : 0=top, 1=right diagonal, -1=left diagonal")]
    [SerializeField] private float _arcDirection = 0f;
    [Tooltip("Makes the trajectory random")]
    [SerializeField] private bool _randomizeArc = true;
    [Tooltip("Min arc direction if random")]
    [SerializeField, Range(-5, 5)] private float _arcRandomDirectionMin = -1f;
    [Tooltip("Max arc direction if random")]
    [SerializeField, Range(-5, 5)] private float _arcRandomDirectionMax = 1f;

    Transform enemyTarget = null;
    float timeAlive = 0f;
    Vector3 direction = Vector3.zero;
    bool isInitialized = false;
    private Vector3 startPos;
    private Vector3 controlPoint;
    private float _speed;

    //[Header("References")]

    [Header("Inputs")]
    [SerializeField] RSE_OnPlayerDeath RSE_OnPlayerDeath;
    [SerializeField] RSE_OnDespawnEnemyProjectile RSE_OnDespawnEnemyProjectile;
    //[Header("Outputs")]

    private void Awake()
    {
        if (_randomizeArc)
        {
            if (_arcRandomDirectionMax < _arcRandomDirectionMin)
            {
                float temp = _arcRandomDirectionMax;
                _arcRandomDirectionMax = _arcRandomDirectionMin;
                _arcRandomDirectionMin = temp;
            }
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

        if (_arcDirection != 0f || _randomizeArc == true)
        {
            Vector3 side = Vector3.Cross(Vector3.up, toTarget.normalized).normalized;
            if (_randomizeArc)
            {
                side *= Random.Range(_arcRandomDirectionMin, _arcRandomDirectionMax);
                arcDir = (Vector3.up + side).normalized;

            }
            else
            {
                arcDir = (Vector3.up + side * _arcDirection).normalized;
            }
        }

        float arcHeight = toTarget.magnitude * 0.25f * _arcHeightMultiplier;
        controlPoint = midPoint + arcDir * arcHeight;

        isInitialized = true;

    }
    private void OnEnable()
    {
        RSE_OnPlayerDeath.action += OnTargetDie;
    }

    private void OnDisable()
    {
        isInitialized = false;
        timeAlive = 0f;
        enemyTarget = null;
        direction = Vector3.zero;

        RSE_OnPlayerDeath.action -= OnTargetDie;
    }
    void OnTargetDie()
    {
        if (enemyTarget != null)
        {
            enemyTarget = null;
        }
    }

    private void Update()
    {
        if (!isInitialized) return;

        timeAlive += Time.deltaTime;
        float t = timeAlive / _travelTime;

        if (timeAlive >= projectileLifeTime)
        {
            RSE_OnDespawnEnemyProjectile.Call(this);
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
            transform.position += direction * _speed * Time.deltaTime;
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
                RSE_OnDespawnEnemyProjectile.Call(this);

                Debug.Log($"Hit enemy for {projectileDamage} damage.");

            }
        }
        else
        {
            RSE_OnDespawnEnemyProjectile.Call(this);
        }

    }
}