using DG.Tweening;
using UnityEngine;

public class S_Waves : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float lifeTime;
    [SerializeField] private float speed;

    //[Header("References")]

    //[Header("Inputs")]

    //[Header("Outputs")]

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}