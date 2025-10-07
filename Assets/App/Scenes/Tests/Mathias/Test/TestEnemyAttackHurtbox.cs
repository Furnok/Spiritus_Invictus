using UnityEngine;

public class TestEnemyAttackHurtbox : MonoBehaviour, IAttackProvider
{
    [Header("Settings")]
    //[SerializeField] EnemyAttackType _attackType;

    [Header("References")]
    [SerializeField] SSO_AttackData _testAttackData;

    //[Header("Input")]

    //[Header("Output")]

    private SSO_AttackData _ssoAttackData;

    void Awake()
    {

    }

    public AttackData GetAttackData()
    {
        return _testAttackData.Value;
    }

}