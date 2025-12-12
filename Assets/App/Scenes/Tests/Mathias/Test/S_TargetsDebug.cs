using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class S_TargetsDebug : MonoBehaviour
{
    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnEnemyEnterTargetingRange rseOnEnemyEnterTargetingRange;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnEnemyExitTargetingRange rseOnEnemyExitTargetingRange;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnNewTargeting rseOnNewTargeting;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerCancelTargeting rseOnPlayerCancelTargeting;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerCenter _rseOnPlayerCenter;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_PlayerIsTargeting rsoPlayerIsTargeting;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_PlayerPosition rsoPlayerPosition;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_FrontConeAngle ssoFrontConeAngle;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_TargetsDebug ssoTargetsDebug;

    private HashSet<Transform> targets = new();
    private bool canDrawTarget = false;
    private Transform target = null;
    private Transform _preSelectedTarget = null;
    private Transform _playerCenterTransform = null;

    private void OnEnable()
    {
        rseOnEnemyEnterTargetingRange.action += AddTarget;
        rseOnEnemyExitTargetingRange.action += RemoveTarget;
        rseOnNewTargeting.action += OnNewTargeting;
        rseOnPlayerCancelTargeting.action += OnCancelTargeting;
        _rseOnPlayerCenter.action += GetPlayerCenterTransform;
    }

    private void OnDisable()
    {
        rseOnEnemyEnterTargetingRange.action -= AddTarget;
        rseOnEnemyExitTargetingRange.action -= RemoveTarget;
        rseOnNewTargeting.action -= OnNewTargeting;
        rseOnPlayerCancelTargeting.action -= OnCancelTargeting;
        _rseOnPlayerCenter.action -= GetPlayerCenterTransform;
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
            if (rsoPlayerIsTargeting.Value == true && canDrawTarget == true && target == this.target || target == null) continue;

            
            Gizmos.color = ssoTargetsDebug.Value.gizmoColor;
            Vector3 pos = new Vector3(target.position.x, target.position.y + ssoTargetsDebug.Value.gizmoHeightOffset, target.position.z);
            Gizmos.DrawSphere(pos, ssoTargetsDebug.Value.gizmoRadius);
        }
    }

    private void DrawTarget()
    {
        if (target == null) return;
        Gizmos.color = ssoTargetsDebug.Value.gizmoTargetColor;
        Vector3 pos = new Vector3(target.position.x, target.position.y + ssoTargetsDebug.Value.gizmoHeightOffset, target.position.z);
        Gizmos.DrawSphere(pos, ssoTargetsDebug.Value.gizmoTargetRadius);
    }

    private void DrawPreSelectedTarget()
    {
        Gizmos.color = ssoTargetsDebug.Value.gizmoPreTargetColor;
        Vector3 pos = new Vector3(_preSelectedTarget.position.x, _preSelectedTarget.position.y + ssoTargetsDebug.Value.gizmoHeightOffset, _preSelectedTarget.position.z);
        Gizmos.DrawSphere(pos, ssoTargetsDebug.Value.gizmoPreTargetRadius);
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

    private void GetPlayerCenterTransform(Transform playerCenter)
    {
        _playerCenterTransform = playerCenter;
    }

    private Transform TargetSelection()
    {
        Transform selectedTarget = null;

        float bestScore = float.MaxValue;

        foreach (var target in targets)
        {
            if (target == null) continue;

            Vector3 toTarget = (target.transform.position - rsoPlayerPosition.Value);
            float distance = toTarget.magnitude;

            if (_playerCenterTransform == null) return null;
            float angle = Vector3.Angle(_playerCenterTransform.forward, toTarget);

            bool inFrontCone = angle <= ssoFrontConeAngle.Value * 0.5f;

            // Priority for the taget in the front cone
            float score = inFrontCone ? distance : distance + 1000f;

            if (score < bestScore /*&& target != currentTarget*/)
            {
                float distanceMax = Vector3.Distance(_playerCenterTransform.position, target.transform.position);

                Vector3 dir = (target.transform.position - _playerCenterTransform.position).normalized;
                if (Physics.Raycast(_playerCenterTransform.position, dir, out RaycastHit hit, distanceMax, ssoTargetsDebug.Value.obstacleMask))
                {
                    continue;
                }
                else
                {
                    bestScore = score;
                    selectedTarget = target;
                }
            }
        }

        return selectedTarget;
    }

    private void OnDrawGizmos()
    {
        if (!enabled) return;

        DrawAll();

        _preSelectedTarget = TargetSelection();

        if (_preSelectedTarget != null && rsoPlayerIsTargeting.Value == false)
        {
            DrawPreSelectedTarget();
        }

        if (rsoPlayerIsTargeting.Value == true && canDrawTarget == true)
        {
            DrawTarget();
        }
    }
}