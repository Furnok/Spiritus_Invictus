using UnityEngine;

public class S_EnemyWeapon : MonoBehaviour, IAttackProvider
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] S_EnemyAttackData S_EnemyAttackData;
    //[Header("Input")]

    //[Header("Output")]

    private AttackData AttackData;
    private void OnEnable()
    {
        S_EnemyAttackData.onChangeAttackData.AddListener(ChangeAttackData);
    }
    private void OnDisable()
    {
        S_EnemyAttackData.onChangeAttackData.RemoveListener(ChangeAttackData);
    }
    public AttackData GetAttackData()
    {
        return AttackData;
    }

    private void ChangeAttackData(SSO_AttackData SSO_attackData)
    {
        AttackData = SSO_attackData.Value;
        Debug.Log("SetData");
    }
}