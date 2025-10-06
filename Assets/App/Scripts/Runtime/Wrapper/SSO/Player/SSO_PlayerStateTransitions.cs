using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SSO_PlayerStateTransitions", menuName = "Data/SSO/Player/State/SSO_PlayerStateTransitions")]
public class SSO_PlayerStateTransitions : ScriptableObject
{
    [System.Serializable]
    public struct TransitionRule
    {
        public PlayerState from;
        public PlayerState to;
    }

    [SerializeField] private List<TransitionRule> _forbiddenTransitions = new();

    public bool CanTransition(PlayerState from, PlayerState to)
    {
        foreach (var rule in _forbiddenTransitions)
        {
            if (rule.from == from && rule.to == to)
                return false;
        }
        return true;
    }
}

public enum PlayerState
{
    Idle,
}
