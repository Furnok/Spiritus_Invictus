using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RandomPatrolWaitTime", story: "Set [PatrolWaitTime] Random [Min] and [Max]", category: "Action", id: "57934c9b9c61a4b988428f512cc2a5ab")]
public partial class S_RandomPatrolWaitTimeAction : Action
{
    [SerializeReference] public BlackboardVariable<float> PatrolWaitTime;
    [SerializeReference] public BlackboardVariable<float> Min;
    [SerializeReference] public BlackboardVariable<float> Max;
    float rnd;
    protected override Status OnStart()
    {
        rnd = UnityEngine.Random.Range(Min.Value, Max.Value);
        PatrolWaitTime.Value = rnd;
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

