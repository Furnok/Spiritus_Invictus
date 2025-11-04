using DG.Tweening.Plugins.Core.PathCore;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.GridLayoutGroup;

public class S_EnemyProjectile : MonoBehaviour, IAttackProvider, IReflectableProjectile
{
    [TabGroup("Settings")]
    [Title("Projectile")]
    [SuffixLabel("s", Overlay = true)]
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _lifeTime = 5f;
    [SerializeField] Rigidbody _rb;
    [SerializeField] string _playerLayer = "PlayerProjectile";
    [SerializeField] Material _enemyMat;
    [SerializeField] Material _playerMat;
    [SerializeField] Renderer _renderer;
    [SerializeField] float _reflectSpeedMul = 1.5f;
    [SerializeField] float _reflectDmgMul = 1.5f;

    [TabGroup("Settings")]
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
    [Tooltip("How long does it take for the projectile to reach the target (s)?")]
    [SerializeField] private float _travelTime = 1f;

    [TabGroup("References")]
    [SerializeField] SSO_EnemyAttackData _testAttackData;

    Transform _owner;
    float _timeAlive = 0f;
    Transform _target = null;
    bool _isInitialized = false;
    Vector3 _direction = Vector3.zero;
    S_StructEnemyAttackData _attackData;
    Vector3 _lastDirection;
    private Vector3 _startPos;
    private Vector3 _controlPoint;

    private void Awake()
    {
        _attackData = _testAttackData.Value;
    }

    public void Initialize(Transform owner, Transform target = null)
    {
        this._target = target;
        this._direction = transform.forward;
        _isInitialized = true;
        _owner = owner;

        CalculateControlPoint();
    }

    private void Update()
    {
        if (!_isInitialized) return;

        _timeAlive += Time.deltaTime;
        float t = _timeAlive / _travelTime;

        if (_timeAlive >= _lifeTime)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 endPos = _target != null ? _target.position : _startPos + transform.forward * 10f;

        Vector3 a = Vector3.Lerp(_startPos, _controlPoint, t);
        Vector3 b = Vector3.Lerp(_controlPoint, endPos, t);
        Vector3 newPos = Vector3.Lerp(a, b, t);
        Vector3 tangent = (b - a).normalized;


        if (_target != null && _target.gameObject.activeInHierarchy)
        {
            transform.position = newPos;
            transform.forward = tangent;

            _direction = tangent;
        }
        else
        {
            transform.position += _direction * _speed * Time.deltaTime;
            transform.forward = _direction;
        }
    }

    public void Reflect(Transform reflectOwner)
    {
        _attackData.damage *= _reflectDmgMul;
        _speed *= _reflectSpeedMul;
        _timeAlive = 0;

        gameObject.layer = LayerMask.NameToLayer(_playerLayer);
        if (_renderer && _playerMat) _renderer.material = _playerMat;

        if (_owner != null && _owner.gameObject.activeInHierarchy)
        {
            _target = _owner;
            CalculateControlPoint();
        }
        else
        {
            _target = null;
            _direction = reflectOwner.forward;
            transform.forward = _direction;
        }
    }

    void CalculateControlPoint()
    {
        this._startPos = transform.position;

        Vector3 toTarget = _target != null ? (_target.position - _startPos) : transform.forward * 10f;
        Vector3 midPoint = _startPos + toTarget * 0.5f;

        //Default arc direction (top)
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
        _controlPoint = midPoint + arcDir * arcHeight;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hurtbox" && other.TryGetComponent(out IDamageable damageable))
        {
            if (damageable != null)
            {
                damageable.TakeDamage(_attackData.damage);
                Destroy(gameObject);
            }
        }
    }

    public ref S_StructEnemyAttackData GetAttackData()
    {

        return ref _attackData;
    }
}