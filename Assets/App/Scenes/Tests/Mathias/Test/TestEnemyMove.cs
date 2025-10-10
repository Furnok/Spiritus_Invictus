using DG.Tweening;
using UnityEngine;

public class TestEnemyMove : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Vector3 _direction = Vector3.right;
    [SerializeField] float _distance;
    [SerializeField] float _duration;
    [SerializeField] bool _loop = true;

    private Vector3 _startPos;
    private Tween _moveTween;

    void Start()
    {
        _startPos = transform.position;

        Vector3 targetPos = _startPos + _direction.normalized * _distance;

        _moveTween = transform.DOMove(targetPos, _duration)
            .SetEase(Ease.Linear)
            .SetLoops(_loop ? -1 : 0, LoopType.Yoyo);
    }

    private void OnDisable()
    {
        if (_moveTween != null && _moveTween.IsActive())
            _moveTween.Kill();
    }
}
