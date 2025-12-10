using Sirenix.OdinInspector;
using UnityEngine;

public class S_EnemyTransformProvider : MonoBehaviour, I_EnemyTransformProvider
{
    [TabGroup("References")]
    [Title("Enemy")]
    [SerializeField] Transform _enemyTransform;

    public Transform GetEnemyTransform()
    {
        return _enemyTransform;
    }
}