using UnityEngine;

public class S_PlayerHurtboxCollider : MonoBehaviour
{
    //[Header("Settings")]
    

    [Header("References")]
    [SerializeField] RSO_CanParry _canParry;
    [SerializeField] RSO_ParryStartTime _parryStartTime;
    [SerializeField] SSO_PlayerStats _playerStats;
    [SerializeField] RSO_AttackDataInDodgeableArea _attackDataInDodgeableArea;


    //[Header("Input")]

    [Header("Output")]
    [SerializeField] RSE_OnAttackCollide _onAttackCollide;

    float _parryToleranceBeforeHit => _playerStats.Value.parryToleranceBeforeHit;
    float _parryToleranceAfterHit => _playerStats.Value.parryToleranceAfterHit;
    float _parryDuration => _playerStats.Value.parryDuration;

    float _hitTime;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hitbox") && other.TryGetComponent(out IAttackProvider attack))
        {
            var attackData = attack.GetAttackData();
            //var goId = other.gameObject.GetInstanceID();

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
    public EnemyAttackData data;
    public Collider source;
}