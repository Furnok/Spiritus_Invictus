using DG.Tweening;
using UnityEngine;

public class TestEnemySimpleMove : MonoBehaviour
{
    //[Header("Settings")]

    //[Header("References")]

    //[Header("Input")]

    //[Header("Output")]

    private void Start()
    {
        transform.DOMoveX(5, 2).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
}