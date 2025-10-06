using UnityEngine;

[CreateAssetMenu(fileName = "RSO_EnemyAttackType", menuName = "Data/RSO/RSO_EnemyAttackType")]
public class RSO_EnemyAttackType : BT.ScriptablesObject.RuntimeScriptableObject<EnemyAttackType> {}

public enum EnemyAttackType
{
    Dodgeable,
    Parryable,
    Projectile,
}