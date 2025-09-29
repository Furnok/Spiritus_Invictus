using System.Collections.Generic;
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

    void OnSwapTargetInput(float directionSwap)
    {
        //Debug.Log("Swap Target Input Received: " + directionSwap);
        if (_targetsPosible.Count == 0 || _playerIsTargeting.Value == false) return;

        var newTarget = TargetSelection();

        if (newTarget != null && newTarget != _currentTarget)
        {
            _onPlayerCancelTargeting.Call(_currentTarget);

            _currentTarget = newTarget;

            _onNewTargeting.Call(newTarget);
        }
    }

    GameObject TargetSelection()
    {
        GameObject selectedTarget = null;
        float minDistance = _playerTargetRangeRadius.Value + 1.0f;
        foreach (var target in _targetsPosible)
        {
            float distance = Vector3.Distance(_playerPosition.Value, target.transform.position);
            if (distance <= minDistance && _currentTarget != target)
            {
                minDistance = distance;
                selectedTarget = target;
            }
        }

        if(selectedTarget != null)
        {
            _targetPosition.Value = selectedTarget.transform.position;
        }

        return selectedTarget;
    }
}