using UnityEngine;

public class S_PlayerHurtboxCollider : MonoBehaviour
{
    //[Header("Settings")]
    

    [Header("References")]
    [SerializeField] RSO_CanParry _canParry;
    [SerializeField] RSO_ParryStartTime _parryStartTime;
    [SerializeField] SSO_PlayerStats _playerStats;
    [SerializeField] RSO_AttackDataInDodgeableArea _attackDataInDodgeableArea;
    [SerializeField] Collider _hurtboxCollider;


    //[Header("Input")]

    [Header("Output")]
    [SerializeField] RSE_OnAttackCollide _onAttackCollide;

    float _parryDuration => _playerStats.Value.parryDuration;

    float _hitTime;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hitbox") && other.TryGetComponent(out I_AttackProvider attack))
        {
            var attackData = attack.GetAttackData();
            //var goId = other.gameObject.GetInstanceID();
            attackData.attackDirection = (transform.position - other.transform.position).normalized;

            Vector3 contactOnMe = _hurtboxCollider.ClosestPoint(other.transform.position);
            Vector3 contactOnOther = other.ClosestPoint(contactOnMe);
            Vector3 finalContact = (contactOnMe + contactOnOther) * 0.5f;

            attackData.contactPoint = finalContact;

            var contact = new AttackContact
            {
                data = attackData,
                source = other
            };

            _onAttackCollide.Call(contact);

        }
    }
}

public struct AttackContact
{
    public S_StructEnemyAttackData data;
    public Collider source;
}