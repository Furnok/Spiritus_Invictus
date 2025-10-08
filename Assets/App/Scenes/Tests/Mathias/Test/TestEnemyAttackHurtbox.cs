using UnityEngine;

public class TestEnemyAttackHurtbox : MonoBehaviour, IAttackProvider
{
    [Header("Settings")]
    //[SerializeField] EnemyAttackType _attackType;

    [Header("References")]
    [SerializeField] SSO_AttackData _testAttackData;

    //[Header("Input")]

    //[Header("Output")]

    AttackData _attackData => _testAttackData.Value;

    void Awake()
    {
        //AttackData test = _testAttackData.Value;
        //Debug.Log($"{test.damage} && {test.attackType}");
    }

    public AttackData GetAttackData()
    {
        return _attackData;
    }

}