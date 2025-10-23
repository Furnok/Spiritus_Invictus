using UnityEngine;

public class TestEnemyProjectile : MonoBehaviour, IAttackProvider
{
    [Header("Settings")]
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _lifeTime = 3f;
    //[SerializeField] private float _damage = 5f;

    [Header("References")]
    [SerializeField] SSO_EnemyAttackData _testAttackData;

    //[Header("Inputs")]

    //[Header("Outputs")]


    float _timeAlive = 0f;
    Transform _playerTarget = null;
    bool _isInitialized = false;
    Vector3 _direction = Vector3.zero;
    EnemyAttackData _attackData;

    private void Awake()
    {
        _attackData = _testAttackData.Value;
    }
    public void Initialize(Transform target = null)
    {
        this._playerTarget = target;
        this._direction = transform.forward;
        _isInitialized = true;

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
        if (_playerTarget != null)
        {
            dir = (_playerTarget.position - transform.position).normalized;
        }
        else
        {
            dir = _direction;
        }

        transform.position += dir * _speed * Time.deltaTime;
        transform.forward = dir;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.tag == "Hurtbox" && other.TryGetComponent(out IDamageable damageable))
    //    {
    //        Debug.Log("1");
    //        if (damageable != null)
    //        {
    //            Debug.Log("2");

    //            damageable.TakeDamage(_damage);
    //            Destroy(gameObject);


    //        }
    //    }
    //    else
    //    {
    //        Destroy(gameObject);
    //        Debug.Log("3");

    //    }

    //}

    public ref EnemyAttackData GetAttackData()
    {
        Destroy(gameObject);

        return ref _attackData;
    }
}