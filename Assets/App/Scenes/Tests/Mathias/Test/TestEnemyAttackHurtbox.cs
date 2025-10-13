using UnityEngine;

public class TestEnemyAttackHurtbox : MonoBehaviour, IAttackProvider
{
    [Header("Settings")]
    //[SerializeField] EnemyAttackType _attackType;

    [Header("References")]
    [SerializeField] SSO_AttackData _testAttackData;

    //[Header("Input")]

    //[Header("Output")]

    AttackData _attackData /*=> _testAttackData.Value*/;

    void Awake()
    {
        _attackData = _testAttackData.Value;
        //_attackData.goSourceId = 50;
        ////Debug.Log(_attackData == _testAttackData.Value);
        //Debug.Log(_attackData.goSourceId);
        //Debug.Log(_testAttackData.Value.goSourceId);
        //AttackData test = _testAttackData.Value;
        //Debug.Log($"{test.damage} && {test.attackType}");
    }

    public ref AttackData GetAttackData()
    {
        return ref _attackData;
    }

    //private void ChangeAttackData(AttackData attackData)
    //{
    //    _attackData = attackData;
    //}

}