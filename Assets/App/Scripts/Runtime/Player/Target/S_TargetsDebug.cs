using System.Collections.Generic;
using UnityEngine;

public class S_TargetsDebug : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Color gizmoColor;
    [SerializeField] private Color gizmoTargetColor;
    [SerializeField] private float gizmoRadius;
    [SerializeField] private float gizmoTargetRadius;
    [SerializeField] private float gizmoHeightOffset;
    [SerializeField] private bool drawGizmos;

    [Header("Input")]
    [SerializeField] private RSE_OnEnemyEnterTargetingRange rseOnEnemyEnterTargetingRange;
    [SerializeField] private RSE_OnEnemyExitTargetingRange rseOnEnemyExitTargetingRange;
    [SerializeField] private RSE_OnNewTargeting rseOnNewTargeting;
    [SerializeField] private RSE_OnPlayerCancelTargeting rseOnPlayerCancelTargeting;

    [Header("Output")]
    [SerializeField] private RSO_PlayerIsTargeting rsoPlayerIsTargeting;

    private HashSet<Transform> targets = new();
    private bool canDrawTarget = false;
    private Transform target = null;

    private void OnEnable()
    {
        rseOnEnemyEnterTargetingRange.action += AddTarget;
        rseOnEnemyExitTargetingRange.action += RemoveTarget;
        rseOnNewTargeting.action += OnNewTargeting;
        rseOnPlayerCancelTargeting.action += OnCancelTargeting;
    }

    private void OnDisable()
    {
        rseOnEnemyEnterTargetingRange.action -= AddTarget;
        rseOnEnemyExitTargetingRange.action -= RemoveTarget;
        rseOnNewTargeting.action -= OnNewTargeting;
        rseOnPlayerCancelTargeting.action -= OnCancelTargeting;
    }

    private void AddTarget(GameObject target)
    {
        if (!targets.Contains(target.transform))
        {
            targets.Add(target.transform);
        }
    }

    private void RemoveTarget(GameObject target)
    {
        if (targets.Contains(target.transform))
        {
            targets.Remove(target.transform);
        }
    }

    private void DrawAll()
    {
        var count = targets.Count;

        foreach (var target in targets)
        {
            if (rsoPlayerIsTargeting.Value == true && canDrawTarget == true && target == this.target) continue;

            Gizmos.color = gizmoColor;
            Vector3 pos = new Vector3(target.position.x, target.position.y + gizmoHeightOffset, target.position.z);
            Gizmos.DrawSphere(pos, gizmoRadius);
        }
    }

    private void DrawTarget()
    {
        Gizmos.color = gizmoTargetColor;
        Vector3 pos = new Vector3(target.position.x, target.position.y + gizmoHeightOffset, target.position.z);
        Gizmos.DrawSphere(pos, gizmoTargetRadius);
    }

    private void OnNewTargeting(GameObject target)
    {
        this.target = target.transform;
        canDrawTarget = true;
    }

    private void OnCancelTargeting(GameObject target)
    {
        canDrawTarget = false;
        this.target = null;
    }

    private void OnDrawGizmos()
    {
        if (!enabled || !drawGizmos) return;

        DrawAll();

        if (rsoPlayerIsTargeting.Value == true && canDrawTarget == true)
        {
            DrawTarget();
        }
    }
}