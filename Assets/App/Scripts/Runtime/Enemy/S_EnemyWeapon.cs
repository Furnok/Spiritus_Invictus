using Sirenix.OdinInspector;
using UnityEngine;

public class S_EnemyWeapon : MonoBehaviour, I_AttackProvider
{
    [TabGroup("References")]
    [Title("Scripts")]
    [SerializeField] private S_EnemyAttackData S_EnemyAttackData;

    private S_StructEnemyAttackData AttackData;

    public ref S_StructEnemyAttackData GetAttackData()
    {
        return ref AttackData;
    }

    public void ChangeAttackData(S_StructEnemyAttackData enemyAttackData)
    {
        AttackData = enemyAttackData;
    }
}