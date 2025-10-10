using UnityEngine;

public class TestEnemyAttackMove : MonoBehaviour
{
    [SerializeField] private Transform _enemy;
    [SerializeField] private float _orbitSpeed = 50f;

    void Update()
    {
        transform.RotateAround(_enemy.position, Vector3.up, _orbitSpeed * Time.deltaTime);
    }
}

