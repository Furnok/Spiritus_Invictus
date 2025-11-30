using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class S_ClassPlayerStateTransitions
{
    [Title("Data")]
    [SerializeField] private List<S_StructTransitionRule> _forbiddenTransitions = new();

    public bool CanTransition(S_EnumPlayerState from, S_EnumPlayerState to)
    {
        foreach (var rule in _forbiddenTransitions)
        {
            if (rule.forbiddenFrom == from && rule.forbiddenTo == to)
                return false;
        }
        return true;
    }
}