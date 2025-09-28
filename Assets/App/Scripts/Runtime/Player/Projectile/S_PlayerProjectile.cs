using UnityEngine;

public class S_PlayerProjectile : MonoBehaviour
{
    [Header("Settings")]

    [Header("References")]

    //[Header("Input")]

    [Header("Output")]
    [SerializeField] RSE_OnDespawnProjectile _onDespawnProjectile;


    [Header("SSO")]
    [SerializeField] SSO_PlayerProjectileData _playerProjectileData;

    Transform _target;
    float _timeAlive = 0f;
    float _speed => _playerProjectileData.Value.speed;
    float _lifeTime => _playerProjectileData.Value.lifeTime;
    float _baseDamage => _playerProjectileData.Value.baseDamage;

    Vector3 _direction;

    bool _isInitialized = false;

    private void OnDisable()
    {
        _isInitialized = false;
        _timeAlive = 0f;
        _target = null;
        _direction = Vector3.zero;
    }

    private void Update()
    {
        if (!_isInitialized) return;

        _timeAlive += Time.deltaTime;
        if (_timeAlive >= _lifeTime)
        {
            _onDespawnProjectile.Call(this);
            return;
        }

        Vector3 dir;
        if (_target != null)
        {
            dir = (_target.position - transform.position).normalized;
        }
        else
        {
            dir = _direction;
        }

        transform.position += dir * _speed * Time.deltaTime;
        transform.forward = dir;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            

            Debug.Log($"Hit enemy for {_baseDamage} damage.");
        }

        _onDespawnProjectile.Call(this);
    }

    public void Initialize(Transform target, Vector3 direction)
    {
        _target = target;
        _direction = direction.normalized;
        _isInitialized = true;
    }
}