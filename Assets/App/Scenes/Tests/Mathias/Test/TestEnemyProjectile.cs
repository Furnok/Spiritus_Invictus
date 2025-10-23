using UnityEngine;

public class TestEnemyProjectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _lifeTime = 3f;
    //[SerializeField] private float _damage = 5f;

    //[Header("References")]

    //[Header("Inputs")]

    //[Header("Outputs")]


    float _timeAlive = 0f;
    Transform _playerTarget = null;
    bool _isInitialized = false;
    Vector3 _direction = Vector3.zero;

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
}