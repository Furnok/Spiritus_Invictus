using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Calculate Animation Time", story: "Calculate [Time] of the [Animation]", category: "Action", id: "4a3d8251b7f67f0a5dd3952c7a2b298f")]
public partial class S_CalculateAnimationTimeAction : Action
{
    [SerializeReference] public BlackboardVariable<float> Time;
    [SerializeReference] public BlackboardVariable<SSO_EnemyListComboAnimation> Animation;
    private float timeAnim;

    protected override Status OnStart()
    {
        timeAnim = 0f;
        foreach (var anim in Animation.Value.Value)
        {
            timeAnim += anim.length;
        }
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
        Time.Value = timeAnim;
    }
}

