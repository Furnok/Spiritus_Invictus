using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class S_TargetsManager : MonoBehaviour
{
    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnEnemyEnterTargetingRange rseOnEnemyEnterTargetingRange;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnEnemyExitTargetingRange rseOnEnemyExitTargetingRange;

    [TabGroup("Outputs")]
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