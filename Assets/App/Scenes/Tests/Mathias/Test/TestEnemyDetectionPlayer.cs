using UnityEngine;

public class TestEnemyDetectionPlayer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField][S_TagName] private string _playerTag;
    [SerializeField] float _delayToShoot = 1f;

    [Header("References")]
    [SerializeField] TestEnemyProjectile _enemyProjectile;
    [SerializeField] GameObject _spwanProjectilePoint;
    [SerializeField] Transform _motorBox;
    //[Header("Inputs")]

    //[Header("Outputs")]

    Transform _aimPoint;
    Coroutine _shootCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_playerTag))
        {
            other.TryGetComponent<I_AimPointProvider>(out I_AimPointProvider aimPointProvider);
            _aimPoint = aimPointProvider != null ? aimPointProvider.GetAimPoint() : other.transform;
            ShootPlayer();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(_playerTag))
        {
            _aimPoint = null;
            StopCoroutine(_shootCoroutine);
        }
    }

    void ShootPlayer()
    {
        _shootCoroutine = StartCoroutine(S_Utils.Delay(_delayToShoot, () =>
        {
            if (_aimPoint != null && _aimPoint.gameObject.activeInHierarchy)
            {
                TestEnemyProjectile projectileInstance = Instantiate(_enemyProjectile, _spwanProjectilePoint.transform.position, Quaternion.identity);
                projectileInstance.Initialize(_motorBox, _aimPoint);
                ShootPlayer();
            }
        }));
    }
}