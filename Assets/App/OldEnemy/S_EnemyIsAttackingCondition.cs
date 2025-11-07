using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Enemy is attacking", story: "[Enemy] is [attacking]", category: "Conditions", id: "41bba84523149be27de7c37ad01a3460")]
public partial class S_EnemyIsAttackingCondition : Condition
{
    [SerializeReference] public BlackboardVariable<Animator> Enemy;
    [SerializeReference] public BlackboardVariable<string> Attacking;
    private bool condition;
    public override bool IsTrue()
    {
        return condition;
    }

    public override void OnStart()
    {
        if (Enemy.Value.GetBool(Attacking.Value) == true)
        {
            condition = true;
        }
    }

    public override void OnEnd()
    {
    }
}
