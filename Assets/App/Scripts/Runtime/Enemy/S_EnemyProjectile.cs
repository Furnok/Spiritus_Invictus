using Sirenix.OdinInspector;
using UnityEngine;

public class S_EnemyProjectile : MonoBehaviour, IAttackProvider, IReflectableProjectile
{
    [TabGroup("Settings")]
    [Title("Layer")]
    [SerializeField] private string playerLayer;

    [TabGroup("Settings")]
    [Title("Projectile")]
    [SerializeField] private float speed;

    [TabGroup("Settings")]
    [SuffixLabel("s", Overlay = true)]
    [SerializeField] private float lifeTime;

    [TabGroup("Settings")]
    [Title("Reflect")]
    [SerializeField] private float reflectSpeedMul;

    [TabGroup("Settings")]
    [SerializeField] private float reflectDmgMul;

    [TabGroup("References")]
    [Title("Rigidbody")]
    [SerializeField] private Rigidbody rb;

    [TabGroup("References")]
    [Title("Materials")]
    [SerializeField] private Material enemyMat;

    [TabGroup("References")]
    [SerializeField] private Material playerMat;

    [TabGroup("References")]
    [Title("Renderer")]
    [SerializeField] private Renderer rendered;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_ProjectileData ssoProjectileData;

    private Transform owner = null;
    private float timeAlive = 0f;
    private Transform target = null;
    private bool isInitialized = false;
    private Vector3 direction = Vector3.zero;
    private S_StructEnemyAttackData attackData;
    private Vector3 startPos = Vector3.zero;
    private Vector3 controlPoint = Vector3.zero;

    private float arcHeightMultiplier => ssoProjectileData.Value.arcHeightMultiplier;
    private float arcDirection => ssoProjectileData.Value.arcDirection;
    private bool randomizeArc => ssoProjectileData.Value.randomizeArc;
    private float arcRandomDirectionMin => ssoProjectileData.Value.arcRandomDirectionMin;
    private float arcRandomDirectionMax => ssoProjectileData.Value.arcRandomDirectionMax;
    private float travelTime => ssoProjectileData.Value.travelTime;

    public void Initialize(Transform owner, Transform target = null, S_StructEnemyAttackData attackData = new())
    {
        this.target = target;
        this.direction = transform.forward;
        this.attackData = attackData;
        isInitialized = true;
        this.owner = owner;

        CalculateControlPoint();
    }

    private void Update()
    {
        if (!isInitialized) return;

        timeAlive += Time.deltaTime;
        float t = timeAlive / travelTime;

        if (timeAlive >= lifeTime)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 endPos = target != null ? target.position : startPos + transform.forward * 10f;

        Vector3 a = Vector3.Lerp(startPos, controlPoint, t);
        Vector3 b = Vector3.Lerp(controlPoint, endPos, t);
        Vector3 newPos = Vector3.Lerp(a, b, t);
        Vector3 tangent = (b - a).normalized;


        if (target != null && target.gameObject.activeInHierarchy)
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

    public void Reflect(Transform reflectOwner)
    {
        attackData.damage *= reflectDmgMul;
        speed *= reflectSpeedMul;
        timeAlive = 0;

        gameObject.layer = LayerMask.NameToLayer(playerLayer);
        if (rendered && playerMat) rendered.material = playerMat;

        if (owner != null && owner.gameObject.activeInHierarchy)
        {
            target = owner;
            CalculateControlPoint();
        }
        else
        {
            target = null;
            direction = reflectOwner.forward;
            transform.forward = direction;
        }
    }

    private void CalculateControlPoint()
    {
        this.startPos = transform.position;

        Vector3 toTarget = target != null ? (target.position - startPos) : transform.forward * 10f;
        Vector3 midPoint = startPos + toTarget * 0.5f;

        //Default arc direction (top)
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hurtbox" && other.TryGetComponent(out IDamageable damageable))
        {
            if (damageable != null)
            {
                damageable.TakeDamage(attackData.damage);
                Destroy(gameObject);
            }
        }
    }

    public ref S_StructEnemyAttackData GetAttackData()
    {
        return ref attackData;
    }
}