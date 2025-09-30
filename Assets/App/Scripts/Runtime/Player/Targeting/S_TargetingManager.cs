using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class S_TargetingManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] LayerMask _obstacleMask;
    [SerializeField] bool _drawGizmos;

    [Header("Input")]
    [SerializeField] RSE_OnTargetsInRangeChange _onTargetsInRangeChange;
    [SerializeField] RSE_OnPlayerTargeting _onPlayerTargeting;
    [SerializeField] RSE_OnPlayerTargetingCancel _onPlayerTargetingCancel;
    [SerializeField] RSE_OnPlayerSwapTarget _onPlayerSwapTarget;
    [SerializeField] RSE_OnEnemyTargetDied _onEnemyTargetDied;

    [Header("Output")]
    [SerializeField] RSE_OnNewTargeting _onNewTargeting;
    [SerializeField] RSE_OnPlayerCancelTargeting _onPlayerCancelTargeting;

    [Header("RSO")]
    [SerializeField] RSO_PlayerIsTargeting _playerIsTargeting;
    [SerializeField] RSO_PlayerPosition _playerPosition;
    [SerializeField] RSO_TargetPosition _targetPosition;

    [Header("SSO")]
    [SerializeField] SSO_PlayerMaxDistanceTargeting _playerMaxDistanceTargeting;
    [SerializeField] SSO_PlayerTargetRangeRadius _playerTargetRangeRadius;
    [SerializeField] SSO_TargetObstacleBreakDelay _targetObstacleBreakDelay;
    [SerializeField] SSO_FrontConeAngle _frontConeAngle;


    GameObject _currentTarget;
    HashSet<GameObject> _targetsPosible = new HashSet<GameObject>();
    float _obstacleTimer = 0f;


    private void Awake()
    {
        _playerIsTargeting.Value = false;
    }

    private void OnEnable()
    {
        _playerIsTargeting.Value = false;

        _onTargetsInRangeChange.action += OnChangeTargetsPosible;
        _onPlayerTargeting.action += OnPlayerTargetingInput;
        _onPlayerTargetingCancel.action += OnPlayerCancelTargetingInput;
        _onPlayerSwapTarget.action += OnSwapTargetInput;

        _onEnemyTargetDied.action += OnEnemyTargetDied;
    }

    private void OnDisable()
    {
        _onTargetsInRangeChange.action -= OnChangeTargetsPosible;
        _onPlayerTargeting.action -= OnPlayerTargetingInput;
        _onPlayerTargetingCancel.action -= OnPlayerCancelTargetingInput;
        _onPlayerSwapTarget.action -= OnSwapTargetInput;

        _onEnemyTargetDied.action -= OnEnemyTargetDied;

        _playerIsTargeting.Value = false;
    }

    private void FixedUpdate()
    {
        if(_currentTarget != null && _playerIsTargeting.Value == true)
        {
            float distance = Vector3.Distance(_playerPosition.Value, _currentTarget.transform.position);

            if (distance > _playerMaxDistanceTargeting.Value && _currentTarget != null)
            {
                CancelTargeting();
            }

            var playerPos = new Vector3(_playerPosition.Value.x, _playerPosition.Value.y + 1.0f, _playerPosition.Value.z);

            if (_currentTarget == null) return;

            Vector3 dir = (_currentTarget.transform.position - playerPos).normalized;
            if (Physics.Raycast(playerPos, dir, out RaycastHit hit, distance, _obstacleMask))
            {

                if (hit.collider.gameObject != _currentTarget)
                {
                    _obstacleTimer += Time.fixedDeltaTime;
                    if (_obstacleTimer >= _targetObstacleBreakDelay.Value)
                    {
                        CancelTargeting();
                    }
                }
                else
                {
                    _obstacleTimer = 0f;
                }
            }
            else
            {
                _obstacleTimer = 0f;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (_currentTarget != null && _playerIsTargeting != null && _playerIsTargeting.Value && _drawGizmos == true)
        {
            var playerPos = new Vector3(_playerPosition.Value.x, _playerPosition.Value.y + 1.0f, _playerPosition.Value.z);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(playerPos, _currentTarget.transform.position);

            Vector3 dir = (_currentTarget.transform.position - playerPos).normalized;
            float distance = Vector3.Distance(playerPos, _currentTarget.transform.position);
            if (Physics.Raycast(playerPos, dir, out RaycastHit hit, distance, _obstacleMask))
            {
                if (hit.collider.gameObject != _currentTarget)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(playerPos, hit.point);
                }
            }
        }

        if (_drawGizmos == true)
        {
            var playerPos = new Vector3(_playerPosition.Value.x, _playerPosition.Value.y + 1.0f, _playerPosition.Value.z);
            if (_playerPosition == null || _playerTargetRangeRadius == null) return;

            Vector3 origin = playerPos;
            float radius = _playerTargetRangeRadius.Value;

            float halfAngle = _frontConeAngle.Value * 0.5f;

            Quaternion leftRot = Quaternion.AngleAxis(-halfAngle, Vector3.up);
            Quaternion rightRot = Quaternion.AngleAxis(halfAngle, Vector3.up);

            Vector3 leftDir = leftRot * transform.forward;
            Vector3 rightDir = rightRot * transform.forward;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin, origin + leftDir * radius);
            Gizmos.DrawLine(origin, origin + rightDir * radius);
        }
    }

    

    void OnChangeTargetsPosible(HashSet<GameObject> targetsList)
    {
        _targetsPosible = targetsList;
    }

    void OnPlayerTargetingInput()
    {
        if (_targetsPosible.Count == 0 || _playerIsTargeting.Value == true) return;

        _currentTarget = TargetSelection();

        if (_currentTarget != null)
        {
            _onNewTargeting.Call(_currentTarget);
        }

        _playerIsTargeting.Value = true;
    }

    void OnPlayerCancelTargetingInput()
    {
        CancelTargeting();
    }

    void CancelTargeting()
    {
        _playerIsTargeting.Value = false;

        if (_currentTarget != null)
        {
            _onPlayerCancelTargeting.Call(_currentTarget);
            _targetPosition.Value = Vector3.zero;
        }

        _currentTarget = null;
        _obstacleTimer = 0f;
    }

    void OnEnemyTargetDied(GameObject enemy)
    {
        if(_currentTarget == enemy)
        {
            _playerIsTargeting.Value = false;
            if (_currentTarget != null)
            {
                _onPlayerCancelTargeting.Call(_currentTarget);
                _targetPosition.Value = Vector3.zero;
            }

            _currentTarget = null;
        }
    }

    void OnSwapTargetInput(float axis) // axis = -1 gauche, +1 droite
    {
        if (_targetsPosible.Count == 0 || _playerIsTargeting.Value == false) return;

        List<(GameObject go, float angle)> candidates = new List<(GameObject, float)>();

        foreach (var target in _targetsPosible)
        {
            if (target == null) continue;

            Vector3 toTarget = (target.transform.position - _playerPosition.Value).normalized;

            float signedAngle = Vector3.SignedAngle(transform.forward, toTarget, Vector3.up);

            candidates.Add((target, signedAngle));
        }

        if (candidates.Count == 0) return;

        float currentAngle = candidates.Find(c => c.go == _currentTarget).angle; // Angle from the current target

        GameObject bestTarget = null;
        float bestDelta = float.MaxValue;

        foreach (var (go, angle) in candidates)
        {
            if (go == _currentTarget) continue;

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

        if (bestTarget != null && bestTarget != _currentTarget)
        {
                _onPlayerCancelTargeting.Call(_currentTarget);
                _currentTarget = bestTarget;
                _targetPosition.Value = _currentTarget.transform.position;
                _onNewTargeting.Call(bestTarget);
        }
    }
  
    GameObject TargetSelection()
    {
        GameObject selectedTarget = null;

        float bestScore = float.MaxValue;

        foreach (var target in _targetsPosible)
        {
            if (target == null) continue;

            Vector3 toTarget = (target.transform.position - _playerPosition.Value);
            float distance = toTarget.magnitude;
            float angle = Vector3.Angle(transform.forward, toTarget);

            bool inFrontCone = angle <= _frontConeAngle.Value * 0.5f;

            //Priority for the taget in the front cone
            float score = inFrontCone ? distance : distance + 1000f;

            if (score < bestScore && target != _currentTarget)
            {
                bestScore = score;
                selectedTarget = target;
            }
        }

        if (selectedTarget != null)
        {
            _targetPosition.Value = selectedTarget.transform.position;
        }

        return selectedTarget;
    }

    
}