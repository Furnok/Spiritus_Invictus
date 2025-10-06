using DG.Tweening;
using UnityEngine;

public class TestEnemyMove : MonoBehaviour
{
    //[Header("Settings")]

    //[Header("References")]

    //[Header("Input")]

    //[Header("Output")]

    void Start()
    {
        gameObject.transform.DOMoveZ(5, 2).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
}