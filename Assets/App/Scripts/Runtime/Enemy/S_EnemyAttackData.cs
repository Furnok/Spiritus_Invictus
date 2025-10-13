using UnityEngine;
using UnityEngine.Events;

public class S_EnemyAttackData : MonoBehaviour
{
    //[Header("Settings")]

    //[Header("References")]

    //[Header("Input")]

    //[Header("Output")]
    [HideInInspector] public UnityEvent<SSO_AttackData> onChangeAttackData;


    public void SetAttackMode(SSO_AttackData SSO_AttackData)
    {
        onChangeAttackData.Invoke(SSO_AttackData);
        Debug.Log("GetData");
    }
}