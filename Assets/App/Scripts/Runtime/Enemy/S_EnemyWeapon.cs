using Sirenix.OdinInspector;
using UnityEngine;

public class S_EnemyWeapon : MonoBehaviour, IAttackProvider
{
    [TabGroup("References")]
    [Title("Enemy")]
    [SerializeField] S_EnemyAttackData S_EnemyAttackData;

    private EnemyAttackData AttackData;

    private void OnEnable()
    {
        S_EnemyAttackData.onChangeAttackData.AddListener(ChangeAttackData);
    }

    private void OnDisable()
    {
        S_EnemyAttackData.onChangeAttackData.RemoveListener(ChangeAttackData);
    }

    public ref EnemyAttackData GetAttackData()
    {
        return ref AttackData;
    }

    private void ChangeAttackData(SSO_EnemyAttackData SSO_attackData)
    {
        AttackData = SSO_attackData.Value;
    }
}