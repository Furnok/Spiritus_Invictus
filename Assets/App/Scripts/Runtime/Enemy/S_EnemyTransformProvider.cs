using UnityEngine;

public class S_EnemyTransformProvider : MonoBehaviour, I_EnemyTransformProvider
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] Transform _enemyTransform;
    
    //[Header("Inputs")]

    //[Header("Outputs")]

    public Transform GetEnemyTransform()
    {
        return _enemyTransform;
    }
}