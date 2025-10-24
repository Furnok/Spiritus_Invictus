using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RSO_AttackCanHitPlayer", menuName = "Data/RSO/Player/AttackCanHitPlayer")]
public class RSO_AttackCanHitPlayer : BT.ScriptablesObject.RuntimeScriptableObject<Dictionary<int, EnemyAttackData>> {}