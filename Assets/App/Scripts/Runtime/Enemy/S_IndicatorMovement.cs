using DG.Tweening;
using UnityEngine;

public class S_IndicatorMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _amplitude = 0.5f;
    [SerializeField] private float _duration = 0.9f;

    //[Header("References")]

    //[Header("Inputs")]

    //[Header("Outputs")]

    Tween floatTween;

    private void OnEnable()
    {
        transform.localPosition = Vector3.zero;
        StartFloatAnim();
    }

    private void OnDisable()
    {
        floatTween?.Kill();
        transform.localPosition = Vector3.zero;
    }

    void StartFloatAnim()
    {
        floatTween?.Kill();

        floatTween = transform
            .DOBlendableLocalMoveBy(new Vector3(0, _amplitude, 0), _duration / 2)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
}