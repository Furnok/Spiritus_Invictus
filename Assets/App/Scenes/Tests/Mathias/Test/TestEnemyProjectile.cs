using UnityEngine;

public class TestEnemyProjectile : MonoBehaviour, IAttackProvider, IReflectableProjectile
{
    [Header("Settings")]
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _lifeTime = 3f;
    [SerializeField] Rigidbody _rb;
    [SerializeField] string _enemyLayer = "EnemyProjectile";
    [SerializeField] string _playerLayer = "PlayerProjectile";
    [SerializeField] Material _enemyMat;
    [SerializeField] Material _playerMat;
    [SerializeField] Renderer _renderer;
    [SerializeField] float _reflectSpeedMul = 1.5f;
    [SerializeField] float _reflectDmgMul = 1.5f;


    [Header("References")]
    [SerializeField] SSO_EnemyAttackData _testAttackData;

    //[Header("Inputs")]

    //[Header("Outputs")]

    Transform _owner;
    float _timeAlive = 0f;
    Transform _target = null;
    bool _isInitialized = false;
    Vector3 _direction = Vector3.zero;
    EnemyAttackData _attackData;
    bool _reflected;
    Vector3 _lastDirection;

    private void Awake()
    {
        _attackData = _testAttackData.Value;
    }
    public void Initialize(Transform owner, Transform target = null)
    {
        this._target = target;
        this._direction = transform.forward;
        _isInitialized = true;
        _reflected = false;
        _owner = owner;
    }

    private void Update()
    {
        if (!_isInitialized) return;

        _timeAlive += Time.deltaTime;

        if (_timeAlive >= _lifeTime)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir;
        if (_target != null && _target.gameObject.activeInHierarchy)
        {
            dir = (_target.position - transform.position).normalized;
        }
        else
        {
            dir = _direction;
        }

        transform.position += dir * _speed * Time.deltaTime;
        transform.forward = dir;
        _direction = dir;
        
    }

    public void Reflect()
    {
        _reflected = true;

        _attackData.damage *= _reflectDmgMul;
        _speed *= _reflectSpeedMul;
        _timeAlive = 0;

        gameObject.layer = LayerMask.NameToLayer(_playerLayer);
        if (_renderer && _playerMat) _renderer.material = _playerMat;

        if ( _owner != null) _target = _owner;
        else _direction = -transform.forward;

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

    public ref EnemyAttackData GetAttackData()
    {

        return ref _attackData;
    }
}