using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class S_EnemyPatrolPointDebug : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Patrol Points")]
    [SerializeField] private List<Transform> patrolPointsList;

    private void Awake()
    {
        Refresh();
    }

    [Button(ButtonSizes.Medium)]
    private void RefreshPatrolPoints()
    {
        Refresh();
    }

    private void Refresh()
    {
        patrolPointsList.Clear();

        foreach (Transform child in transform)
        {
            patrolPointsList.Add(child);
        }
    }

    private void OnDrawGizmos()
    {
        if (patrolPointsList == null || patrolPointsList.Count < 2)
            return;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < patrolPointsList.Count; i++)
        {
            Transform current = patrolPointsList[i];
            Transform next = patrolPointsList[(i + 1) % patrolPointsList.Count];

            if (current != null && next != null)
            {
                Gizmos.DrawLine(current.position, next.position);
                Gizmos.DrawSphere(current.position, 0.2f);
            }
        }
    }
}