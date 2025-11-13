using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class S_TargetingManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private bool drawGizmos;

    [Header("Reference")]
    [SerializeField] GameObject _previewIndicatorPrefab;
    [SerializeField] GameObject _lockedIndicatorPrefab;

    [Header("Input")]
    [SerializeField] private RSE_OnTargetsInRangeChange rseOnTargetsInRangeChange;
    [SerializeField] private RSE_OnPlayerTargeting rseOnPlayerTargeting;
    [SerializeField] private RSE_OnPlayerTargetingCancel rseOnPlayerTargetingCancel;
    [SerializeField] private RSE_OnCancelTargeting rseOnCancelTargeting;
    [SerializeField] private RSE_OnPlayerSwapTarget rseOnPlayerSwapTarget;
    [SerializeField] private RSE_OnEnemyTargetDied rseOnEnemyTargetDied;
    [SerializeField] private RSE_OnPlayerCenter _rseOnPlayerCenter;

    [Header("Output")]
    [SerializeField] private RSE_OnNewTargeting rseOnNewTargeting;
    [SerializeField] private RSE_OnPlayerCancelTargeting rseOnPlayerCancelTargeting;
    [SerializeField] private RSO_PlayerIsTargeting rsoPlayerIsTargeting;
    [SerializeField] private RSO_PlayerPosition rsoPlayerPosition;
    [SerializeField] private RSO_TargetPosition rsoTargetPosition;
    [SerializeField] private SSO_PlayerMaxDistanceTargeting ssoPlayerMaxDistanceTargeting;
    [SerializeField] private SSO_PlayerTargetRangeRadius ssoPlayerTargetRangeRadius;
    [SerializeField] private SSO_TargetObstacleBreakDelay ssoPargetObstacleBreakDelay;
    [SerializeField] private SSO_FrontConeAngle ssoFrontConeAngle;
    [SerializeField] private RSE_OnAnimationBoolValueChange rseOnAnimationBoolValueChange;
    [SerializeField] private RSO_SettingsSaved rsoSettingsSaved;

    private GameObject currentTarget = null;
    private HashSet<GameObject> targetsPossible = new();
    private float obstacleTimer = 0f;
    private Transform _playerCenterTransform;

    private GameObject _previewGO;
    private GameObject _lockedGO;
    private Transform _previewTargetTransform;
    private Transform _lockedTargetTransfrom;

    private void Awake()
    {
        rsoPlayerIsTargeting.Value = false;

        _previewGO = Instantiate(_previewIndicatorPrefab, gameObject.transform);
        _lockedGO = Instantiate(_lockedIndicatorPrefab, gameObject.transform);

        _previewGO.SetActive(false);
        _lockedGO.SetActive(false);
    }

    private void OnEnable()
    {
        rsoPlayerIsTargeting.Value = false;

        rseOnTargetsInRangeChange.action += OnChangeTargetsPosible;
        rseOnPlayerTargeting.action += OnPlayerTargetingInput;
        rseOnPlayerTargetingCancel.action += OnPlayerCancelTargetingInput;
        rseOnCancelTargeting.action += CancelTargeting;
        rseOnPlayerSwapTarget.action += OnSwapTargetInput;

        rseOnEnemyTargetDied.action += OnEnemyTargetDied;

        _rseOnPlayerCenter.action += GetPlayerCenterTransform;
    }

    private void OnDisable()
    {
        rseOnTargetsInRangeChange.action -= OnChangeTargetsPosible;
        rseOnPlayerTargeting.action -= OnPlayerTargetingInput;
        rseOnPlayerTargetingCancel.action -= OnPlayerCancelTargetingInput;
        rseOnCancelTargeting.action -= CancelTargeting;
        rseOnPlayerSwapTarget.action -= OnSwapTargetInput;

        rseOnEnemyTargetDied.action -= OnEnemyTargetDied;

        _rseOnPlayerCenter.action -= GetPlayerCenterTransform;

        rsoPlayerIsTargeting.Value = false;
    }

    void GetPlayerCenterTransform(Transform playerCenter)
    {
        _playerCenterTransform = playerCenter;
    }

    private void Update()
    {
        var selection = TargetSelection();

        if (targetsPossible.Count > 0 && selection != null || currentTarget != null)
        {
            if (currentTarget == null)
            {
                if (selection.TryGetComponent(out ITargetable targetable))
                {
                    _previewTargetTransform = targetable.GetTargetLockOnAnchorTransform();
                }
                else
                {
                    _previewTargetTransform = selection.transform;
                }

                _previewGO.SetActive(true);
                _lockedGO.SetActive(false);

                _previewGO.transform.position = _previewTargetTransform.position;
            }
            else if (currentTarget != null)
            {
                if (currentTarget.TryGetComponent(out ITargetable targetable))
                {
                    _lockedTargetTransfrom = targetable.GetTargetLockOnAnchorTransform();
                }
                else
                {
                    _lockedTargetTransfrom = currentTarget.transform;
                }

                _previewGO.SetActive(false);
                _lockedGO.SetActive(true);

                _lockedGO.transform.position = _lockedTargetTransfrom.position;
            }
        }
        else
        {
            if (_previewGO.activeInHierarchy || _lockedGO.activeInHierarchy)
            {
                _previewGO.SetActive(false);
                _lockedGO.SetActive(false);
            }
        }
    }

    private void FixedUpdate()
    {
        if (currentTarget != null && rsoPlayerIsTargeting.Value == true)
        {
            float distance = Vector3.Distance(rsoPlayerPosition.Value, currentTarget.transform.position);

            if (distance > ssoPlayerMaxDistanceTargeting.Value && currentTarget != null)
            {
                CancelTargeting();
            }

            var playerPos = new Vector3(rsoPlayerPosition.Value.x, rsoPlayerPosition.Value.y + 1.0f, rsoPlayerPosition.Value.z);

            if (currentTarget == null) return;

            Vector3 dir = (currentTarget.transform.position - playerPos).normalized;
            if (Physics.Raycast(playerPos, dir, out RaycastHit hit, distance, obstacleMask))
            {

                if (hit.collider.gameObject != currentTarget)
                {
                    obstacleTimer += Time.fixedDeltaTime;
                    if (obstacleTimer >= ssoPargetObstacleBreakDelay.Value)
                    {
                        CancelTargeting();
                    }
                }
                else
                {
                    obstacleTimer = 0f;
                }
            }
            else
            {
                obstacleTimer = 0f;
            }
        }
    }

    private void OnChangeTargetsPosible(HashSet<GameObject> targetsList)
    {
        targetsPossible = targetsList;
    }

    private void OnPlayerTargetingInput()
    {
        if (rsoSettingsSaved.Value.holdLockTarget == false && rsoPlayerIsTargeting.Value == true)
        {
            CancelTargeting();
            return;
        }

        if (targetsPossible.Count == 0 || rsoPlayerIsTargeting.Value == true) return;

        currentTarget = TargetSelection();

        if (currentTarget != null)
        {
            rseOnNewTargeting.Call(currentTarget);
            rsoPlayerIsTargeting.Value = true;
        }
    }

    private void OnPlayerCancelTargetingInput()
    {
        if(rsoSettingsSaved.Value.holdLockTarget == false) return;
        
        CancelTargeting();
        
    }

    private void CancelTargeting()
    {
        rsoPlayerIsTargeting.Value = false;

        if (currentTarget != null)
        {
            rseOnPlayerCancelTargeting.Call(currentTarget);
            rsoTargetPosition.Value = Vector3.zero;
            rseOnAnimationBoolValueChange.Call("TargetLock", false);
        }

        currentTarget = null;
        obstacleTimer = 0f;
    }

    private void OnEnemyTargetDied(GameObject enemy)
    {
        if (currentTarget == enemy)
        {
            rsoPlayerIsTargeting.Value = false;
            if (currentTarget != null)
            {
                rseOnPlayerCancelTargeting.Call(currentTarget);

                currentTarget = TargetSelection();

                if (currentTarget != null)
                {
                    rseOnNewTargeting.Call(currentTarget);
                    rsoPlayerIsTargeting.Value = true;
                }
                else
                {
                    rseOnAnimationBoolValueChange.Call("TargetLock", false);
                }
            }
        }
    }

    private void OnSwapTargetInput(float axis) // axis = -1 gauche, +1 droite
    {
        if (targetsPossible.Count == 0 || rsoPlayerIsTargeting.Value == false) return;

        List<(GameObject go, float angle)> candidates = new List<(GameObject, float)>();

        foreach (var target in targetsPossible)
        {
            if (target == null) continue;

            Vector3 toTarget = (target.transform.position - rsoPlayerPosition.Value).normalized;

            if (_playerCenterTransform == null) return;
            float signedAngle = Vector3.SignedAngle(_playerCenterTransform.forward, toTarget, Vector3.up);


            float distanceMax = Vector3.Distance(_playerCenterTransform.position, target.transform.position);

            Vector3 dir = (target.transform.position - _playerCenterTransform.position).normalized;
            if (Physics.Raycast(_playerCenterTransform.position, dir, out RaycastHit hit, distanceMax, obstacleMask))
            {
                continue;
            }

            candidates.Add((target, signedAngle));
        }

        if (candidates.Count == 0) return;

        float currentAngle = candidates.Find(c => c.go == currentTarget).angle; // Angle from the current target

        GameObject bestTarget = null;
        float bestDelta = float.MaxValue;

        foreach (var (go, angle) in candidates)
        {
            if (go == currentTarget) continue;

            float delta = angle - currentAngle;

            // Normalise in [-180, 180]
            if (delta > 180)
            {
                delta -= 360;
            }
            if (delta < -180)
            {
                delta += 360;
            }

            // if axis > 0 ? right ? look for the smallest positive delta
            // if axis < 0 ? left ? look for the largest negative delta (the closest to 0)
            if (axis > 0 && delta > 0 && delta < bestDelta)
            {
                bestDelta = delta;
                bestTarget = go;
            }
            else if (axis < 0 && delta < 0 && Mathf.Abs(delta) < bestDelta)
            {
                bestDelta = Mathf.Abs(delta);
                bestTarget = go;
            }
        }

        // if nothing found
        if (bestTarget == null)
        {
            if (axis > 0)
            {
                bestTarget = candidates.OrderBy(c => c.angle).First().go; // further left
            }
            else
            {
                bestTarget = candidates.OrderByDescending(c => c.angle).First().go; // further right
            }
        }

        if (bestTarget != null && bestTarget != currentTarget)
        {
            rseOnPlayerCancelTargeting.Call(currentTarget);
            currentTarget = bestTarget;
            rsoTargetPosition.Value = currentTarget.transform.position;
            rseOnNewTargeting.Call(bestTarget);
        }
    }

    private GameObject TargetSelection()
    {
        GameObject selectedTarget = null;

        float bestScore = float.MaxValue;

        foreach (var target in targetsPossible)
        {
            if (target == null) continue;

            Vector3 toTarget = (target.transform.position - rsoPlayerPosition.Value);
            float distance = toTarget.magnitude;

            if(_playerCenterTransform == null) return null;
            float angle = Vector3.Angle(_playerCenterTransform.forward, toTarget);

            bool inFrontCone = angle <= ssoFrontConeAngle.Value * 0.5f;

            //Priority for the taget in the front cone
            float score = inFrontCone ? distance : distance + 1000f;

            if (score < bestScore && target != currentTarget)
            {
                float distanceMax = Vector3.Distance(_playerCenterTransform.position, target.transform.position);

                Vector3 dir = (target.transform.position - _playerCenterTransform.position).normalized;
                if (Physics.Raycast(_playerCenterTransform.position, dir, out RaycastHit hit, distanceMax, obstacleMask))
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

        if (selectedTarget != null)
        {
            rsoTargetPosition.Value = selectedTarget.transform.position;

            rseOnAnimationBoolValueChange.Call("TargetLock", true);
        }

        return selectedTarget;
    }

    private void OnDrawGizmos()
    {
        if (currentTarget != null && rsoPlayerIsTargeting != null && rsoPlayerIsTargeting.Value && drawGizmos == true)
        {
            var playerPos = new Vector3(rsoPlayerPosition.Value.x, rsoPlayerPosition.Value.y + 1.0f, rsoPlayerPosition.Value.z);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(playerPos, currentTarget.transform.position);

            Vector3 dir = (currentTarget.transform.position - playerPos).normalized;
            float distance = Vector3.Distance(playerPos, currentTarget.transform.position);
            if (Physics.Raycast(playerPos, dir, out RaycastHit hit, distance, obstacleMask))
            {
                if (hit.collider.gameObject != currentTarget)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(playerPos, hit.point);
                }
            }
        }

        if (drawGizmos == true)
        {
            var playerPos = new Vector3(rsoPlayerPosition.Value.x, rsoPlayerPosition.Value.y + 1.0f, rsoPlayerPosition.Value.z);
            if (rsoPlayerPosition == null || ssoPlayerTargetRangeRadius == null) return;

            Vector3 origin = playerPos;
            float radius = ssoPlayerTargetRangeRadius.Value;

            float halfAngle = ssoFrontConeAngle.Value * 0.5f;

            Quaternion leftRot = Quaternion.AngleAxis(-halfAngle, Vector3.up);
            Quaternion rightRot = Quaternion.AngleAxis(halfAngle, Vector3.up);

            if (_playerCenterTransform == null) return;
            Vector3 leftDir = leftRot * _playerCenterTransform.forward;
            Vector3 rightDir = rightRot * _playerCenterTransform.forward;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin, origin + leftDir * radius);
            Gizmos.DrawLine(origin, origin + rightDir * radius);
        }
    }
}