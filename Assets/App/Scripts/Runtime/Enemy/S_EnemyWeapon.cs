using Sirenix.OdinInspector;
using UnityEngine;

public class S_EnemyWeapon : MonoBehaviour, I_AttackProvider
{
    [TabGroup("References")]
    [Title("Enemy")]
    [SerializeField] private S_EnemyAttackData S_EnemyAttackData;

    private S_StructEnemyAttackData AttackData;

    private void OnEnable()
    {
        S_EnemyAttackData.onChangeAttackData.AddListener(ChangeAttackData);
    }

    private void OnDisable()
    {
        S_EnemyAttackData.onChangeAttackData.RemoveListener(ChangeAttackData);
    }

    public ref S_StructEnemyAttackData GetAttackData()
    {
        return ref AttackData;
    }

    private void ChangeAttackData(S_StructEnemyAttackData enemyAttackData)
    {
        AttackData = enemyAttackData;
    }
}