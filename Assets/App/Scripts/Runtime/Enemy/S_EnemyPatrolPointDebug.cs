using System.Collections.Generic;
using UnityEngine;

public class S_EnemyPatrolPointDebug : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private List<Transform> patrolPointList;

    private void OnValidate()
    {
        patrolPointList.Clear();

        foreach (Transform child in transform)
        {
            patrolPointList.Add(child);
        }
    }

    private void Awake()
    {
        patrolPointList.Clear();

        foreach (Transform child in transform)
        {
            patrolPointList.Add(child);
        }
    }

    private void OnDrawGizmos()
    {
        if (patrolPointList == null || patrolPointList.Count < 2)
            return;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < patrolPointList.Count; i++)
        {
            Transform current = patrolPointList[i];
            Transform next = patrolPointList[(i + 1) % patrolPointList.Count];

            if (current != null && next != null)
            {
                Gizmos.DrawLine(current.position, next.position);
                Gizmos.DrawSphere(current.position, 0.2f);
            }
        }
    }
}