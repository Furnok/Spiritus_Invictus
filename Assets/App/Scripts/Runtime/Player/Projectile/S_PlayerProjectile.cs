using UnityEngine;

public class S_PlayerProjectile : MonoBehaviour
{
    [Header("References")]
    [SerializeField] SSO_PlayerAttackSteps _playerAttackSteps;
    [SerializeField] SSO_PlayerStats _playerStats;

    [Header("Input")]

    [SerializeField] private RSE_OnEnemyTargetDied _onEnemyTargetDied;

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

    [Header("Output")]
    [SerializeField] private RSE_OnDespawnProjectile rseOnDespawnProjectile;

    private Transform _target = null;
    private float _timeAlive = 0f;
    private float _speed;
    private float _lifeTime => _playerStats.Value.projectileLifeTime;
    private float _damage;
    private Vector3 _direction = Vector3.zero;
    private bool _isInitialized = false;
    private Vector3 _startPos;
    private Vector3 _controlPoint;

    int _attackStep = 0;

    private void Awake()
    {
        if (_randomizeArc)
        {
            if ( _arcRandomDirectionMax < _arcRandomDirectionMin )
            {
                float temp = _arcRandomDirectionMax;
                _arcRandomDirectionMax = _arcRandomDirectionMin;
                _arcRandomDirectionMin = temp;
            }
        }
    }
    public void Initialize(float damage, Transform target = null, int attackStep = 0)
    {
        this._target = target;
        this._direction = transform.forward;
        _attackStep = attackStep;
        _speed = _playerAttackSteps.Value.Find(x => x.step == attackStep).speed;
        _damage = damage;

        this._startPos = transform.position;

        Vector3 toTarget = target != null ? (target.position - _startPos) : transform.forward * 10f;
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

        _isInitialized = true;
    }

    private void OnEnable()
    {
        _onEnemyTargetDied.action += OnTargetDie;
    }

    private void OnDisable()
    {
        _isInitialized = false;
        _timeAlive = 0f;
        _target = null;
        _direction = Vector3.zero;

        _onEnemyTargetDied.action -= OnTargetDie;

    }

    void OnTargetDie(GameObject enemyDie)
    {
        if (_target != null && enemyDie == _target.gameObject && enemyDie != null)
        {
            _target = null;
        }
    }
    private void Update()
    {
        if (!_isInitialized) return;

        _timeAlive += Time.deltaTime;
        float t = _timeAlive / _travelTime;

        if (_timeAlive >= _lifeTime)
        {
            rseOnDespawnProjectile.Call(this);
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