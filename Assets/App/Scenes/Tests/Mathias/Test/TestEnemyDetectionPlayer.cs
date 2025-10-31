using UnityEngine;

public class TestEnemyDetectionPlayer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField][S_TagName] private string _playerTag;
    [SerializeField] float _delayToShoot = 1f;

    [Header("References")]
    [SerializeField] TestEnemyProjectile _enemyProjectile;
    [SerializeField] GameObject _spwanProjectilePoint;
    [SerializeField] Transform _hurtbox;
    //[Header("Inputs")]

    //[Header("Outputs")]

    Transform _playerTransform;
    Coroutine _shootCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_playerTag))
        {
            _playerTransform = other.transform;
            ShootPlayer();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(_playerTag))
        {
            _playerTransform = null;
            StopCoroutine(_shootCoroutine);
        }
    }

    void ShootPlayer()
    {
        _shootCoroutine = StartCoroutine(S_Utils.Delay(_delayToShoot, () =>
        {
            if (_playerTransform != null && _playerTransform.gameObject.activeInHierarchy)
            {
                TestEnemyProjectile projectileInstance = Instantiate(_enemyProjectile, _spwanProjectilePoint.transform.position, Quaternion.identity);
                projectileInstance.Initialize(_hurtbox, _playerTransform);
                ShootPlayer();
            }
        }));
    }
}