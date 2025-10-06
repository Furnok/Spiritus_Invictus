using UnityEngine;

public class TestEnemyAttackHurtbox : MonoBehaviour, IAttackProvider
{
    [Header("Settings")]
    [SerializeField] EnemyAttackType _attackType;

    //[Header("References")]

    //[Header("Input")]

    //[Header("Output")]

    private RSO_EnemyAttackType _rsoEnemyAttackType;

    void Awake()
    {
        _rsoEnemyAttackType = ScriptableObject.CreateInstance<RSO_EnemyAttackType>();
        _rsoEnemyAttackType.Value = _attackType;
    }

    public EnemyAttackType GetAttackType()
    {
        return _rsoEnemyAttackType.Value;
    }

}