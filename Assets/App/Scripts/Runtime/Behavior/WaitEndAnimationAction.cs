using System;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.Properties;
using UnityEditor.Animations;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Wait End Animation", story: "Wait [time] a [animation] end", category: "Action", id: "6056a1cfc7beaad2f297f5cd5ee6981f")]
public partial class WaitEndAnimationAction : Action
{
    [SerializeReference] public BlackboardVariable<float> time;
    [SerializeReference] public BlackboardVariable<Animator> animator;
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

