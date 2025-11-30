using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class S_IndicatorMovement : MonoBehaviour
{
    [TabGroup("Settings")]
    [SerializeField] private float _amplitude = 0.5f;

    [TabGroup("Settings")]
    [SuffixLabel("s", Overlay = true)]
    [SerializeField] private float _duration = 0.9f;

    private Tween floatTween = null;

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

    private void StartFloatAnim()
    {
        floatTween?.Kill();

        floatTween = transform
            .DOBlendableLocalMoveBy(new Vector3(0, _amplitude, 0), _duration / 2)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
}