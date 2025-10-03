using UnityEngine;
using System.Collections.Generic;

public class S_TargetsManager : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private RSE_OnEnemyEnterTargetingRange rseOnEnemyEnterTargetingRange;
    [SerializeField] private RSE_OnEnemyExitTargetingRange rseOnEnemyExitTargetingRange;

    [Header("Output")]
    [SerializeField] private RSE_OnTargetsInRangeChange rseOnTargetsInRangeChange;

    private HashSet<GameObject> enemiesInRange = new();

    private void OnEnable()
    {
        rseOnEnemyEnterTargetingRange.action += EnemyEnterRange;
        rseOnEnemyExitTargetingRange.action += EnemyExitRange;
    }

    private void OnDisable()
    {
        rseOnEnemyEnterTargetingRange.action -= EnemyEnterRange;
        rseOnEnemyExitTargetingRange.action -= EnemyExitRange;
    }

    private void EnemyEnterRange(GameObject enemy)
    {
        if (!enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Add(enemy);

            rseOnTargetsInRangeChange.Call(enemiesInRange);
        }
    }

    private void EnemyExitRange(GameObject enemy)
    {
        if (enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Remove(enemy);

            rseOnTargetsInRangeChange.Call(enemiesInRange);
        }
    }
}