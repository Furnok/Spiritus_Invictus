using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SSO_PlayerStateTransitions", menuName = "Data/SSO/Player/State/SSO_PlayerStateTransitions")]
public class SSO_PlayerStateTransitions : ScriptableObject
{
    [System.Serializable]
    public struct TransitionRule
    {
        public PlayerState forbiddenFrom;
        public PlayerState forbiddenTo;
    }

    [SerializeField] private List<TransitionRule> _forbiddenTransitions = new();

    public bool CanTransition(PlayerState from, PlayerState to)
    {
        foreach (var rule in _forbiddenTransitions)
        {
            if (rule.forbiddenFrom == from && rule.forbiddenTo == to)
                return false;
        }
        return true;
    }
}

public enum PlayerState
{
    None = 0,
    Idle = 1,
    Moving = 1 << 1,
    Attacking = 1 << 2,
    Dodging = 1 << 3,
    Parrying = 1 << 4,
    Healing = 1 << 5,
    Dying = 1 << 6,
    //Targeting = 1 << 7,
}
