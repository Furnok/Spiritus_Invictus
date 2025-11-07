using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class S_EnemyPatrolPoints : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Patrol Points")]
    [SerializeField] private List<GameObject> patrolPointsList;

    private void Awake()
    {
        Refresh();
    }

    [Button(ButtonSizes.Medium)]
    private void RefreshPatrolPoints()
    {
        Refresh();
    }

    public List<GameObject> GetPatrolPointsList()
    {
        return patrolPointsList;
    }

    private void Refresh()
    {
        patrolPointsList.Clear();

        foreach (Transform child in transform)
        {
            patrolPointsList.Add(child.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        if (patrolPointsList == null || patrolPointsList.Count < 2)
            return;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < patrolPointsList.Count; i++)
        {
            GameObject current = patrolPointsList[i];
            GameObject next = patrolPointsList[(i + 1) % patrolPointsList.Count];

            if (current != null && next != null)
            {
                Gizmos.DrawLine(current.transform.position, next.transform.position);
                Gizmos.DrawSphere(current.transform.position, 0.2f);
            }
        }
    }
}