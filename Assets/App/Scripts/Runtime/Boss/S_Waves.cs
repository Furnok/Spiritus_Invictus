using Sirenix.OdinInspector;
using UnityEngine;

public class S_Waves : MonoBehaviour
{
    [Header("Settings")]
    [Title("Parameters")]
    [SerializeField] private float lifeTime;

    [Header("Settings")]
    [SerializeField] private float speed;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}