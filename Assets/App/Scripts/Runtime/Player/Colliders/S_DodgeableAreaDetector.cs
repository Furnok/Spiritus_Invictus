using Sirenix.OdinInspector;
using UnityEngine;

public class S_DodgeableAreaDetector : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Filter")]
    [SerializeField, S_TagName] private string tagHit;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_AttackDataInDodgeableArea _attackDataInDodgeableArea;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_AttackCanHitPlayer _attackCanHitPlayer;

    private void Awake()
    {
        _attackDataInDodgeableArea.Value = new System.Collections.Generic.Dictionary<int, S_StructEnemyAttackData>();
        _attackDataInDodgeableArea.Value.Clear();

        _attackCanHitPlayer.Value = new System.Collections.Generic.Dictionary<int, S_StructEnemyAttackData>();
        _attackCanHitPlayer.Value.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagHit) && other.TryGetComponent(out I_AttackProvider attack))
        {
            var goId = other.gameObject.GetInstanceID();
            ref var attackData = ref attack.GetAttackData();
            attackData.goSourceId = goId;

            if (_attackDataInDodgeableArea.Value == null || _attackDataInDodgeableArea.Value.ContainsKey(goId) || attackData.attackType != S_EnumEnemyAttackType.Dodgeable) return;
            
            _attackDataInDodgeableArea.Value.Add(goId, attack.GetAttackData());

            if (_attackCanHitPlayer.Value == null || _attackCanHitPlayer.Value.ContainsKey(goId) || attackData.attackType != S_EnumEnemyAttackType.Dodgeable) return;

            _attackCanHitPlayer.Value.Add(goId, attack.GetAttackData());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagHit) && other.TryGetComponent(out I_AttackProvider attack))
        {
            var goId = other.gameObject.GetInstanceID();
            if (_attackDataInDodgeableArea.Value.ContainsKey(goId) == false) return;

            _attackDataInDodgeableArea.Value.Remove(goId);

            if (_attackCanHitPlayer.Value.ContainsKey(goId) == false) return;

            _attackCanHitPlayer.Value.Remove(goId);
        }
    }
}