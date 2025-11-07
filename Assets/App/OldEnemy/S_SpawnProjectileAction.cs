using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SpawnProjectile", story: "Spawn [Projectile] With [Damage]", category: "Action", id: "5768fe02acfca8158dea3130a5ba1119")]
public partial class S_SpawnProjectileAction : Action
{
    [SerializeReference] public BlackboardVariable<RSE_OnSpawnEnemyProjectile> Projectile;
    [SerializeReference] public BlackboardVariable<float> Damage;

    protected override Status OnStart()
    {
        //Projectile.Value.Call(Damage.Value);
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

