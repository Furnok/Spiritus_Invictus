using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "RSO_AttackDataInDodgeableArea", menuName = "Data/RSO/Player/AttackDataInDodgeableArea")]
public class RSO_AttackDataInDodgeableArea : BT.ScriptablesObject.RuntimeScriptableObject<Dictionary<int, EnemyAttackData>>{}