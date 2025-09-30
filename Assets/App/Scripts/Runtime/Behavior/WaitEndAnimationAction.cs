using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Wait End Animation", story: "Wait [Seconds] End [Animation]", category: "Action", id: "6056a1cfc7beaad2f297f5cd5ee6981f")]
public partial class WaitEndAnimationAction : Action
{
    [SerializeReference] public BlackboardVariable<float> Seconds;
    [SerializeReference] public BlackboardVariable<Animator> Animation;

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

