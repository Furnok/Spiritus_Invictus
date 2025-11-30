using Sirenix.OdinInspector;
using UnityEngine;

public class S_PlayerHurtboxCollider : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Filter")]
    [SerializeField, S_TagName] private string tagHit;

    [TabGroup("References")]
    [SerializeField] private Collider _hurtboxCollider;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnAttackCollide _onAttackCollide;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagHit) && other.TryGetComponent(out I_AttackProvider attack))
        {
            var attackData = attack.GetAttackData();
            attackData.attackDirection = (transform.position - other.transform.position).normalized;

            Vector3 contactOnMe = _hurtboxCollider.ClosestPoint(other.transform.position);
            Vector3 contactOnOther = other.ClosestPoint(contactOnMe);
            Vector3 finalContact = (contactOnMe + contactOnOther) * 0.5f;

            attackData.contactPoint = finalContact;

            var contact = new S_StructAttackContact
            {
                data = attackData,
                source = other
            };

            _onAttackCollide.Call(contact);
        }
    }
}