using UnityEngine;

public class S_DodgeableAreaDetector : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] RSO_AttackDataInDodgeableArea _attackDataInDodgeableArea;
    [SerializeField] RSO_AttackCanHitPlayer _attackCanHitPlayer;

    //[Header("Input")]

    //[Header("Output")]

    private void Awake()
    {
        _attackDataInDodgeableArea.Value = new System.Collections.Generic.Dictionary<int, EnemyAttackData>();
        _attackDataInDodgeableArea.Value.Clear();

        _attackCanHitPlayer.Value = new System.Collections.Generic.Dictionary<int, EnemyAttackData>();
        _attackCanHitPlayer.Value.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hitbox") && other.TryGetComponent(out IAttackProvider attack))
        {
            var goId = other.gameObject.GetInstanceID();
            ref var attackData = ref attack.GetAttackData();
            attackData.goSourceId = goId;

            if (_attackDataInDodgeableArea.Value == null || _attackDataInDodgeableArea.Value.ContainsKey(goId) || attackData.attackType != EnemyAttackType.Dodgeable) return;
            
            _attackDataInDodgeableArea.Value.Add(goId, attack.GetAttackData());

            if (_attackCanHitPlayer.Value == null || _attackCanHitPlayer.Value.ContainsKey(goId) || attackData.attackType != EnemyAttackType.Dodgeable) return;

            _attackCanHitPlayer.Value.Add(goId, attack.GetAttackData());

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hitbox") && other.TryGetComponent(out IAttackProvider attack))
        {

            var goId = other.gameObject.GetInstanceID();
            if (_attackDataInDodgeableArea.Value.ContainsKey(goId) == false) return;
            _attackDataInDodgeableArea.Value.Remove(goId);

            if (_attackCanHitPlayer.Value.ContainsKey(goId) == false) return;
            _attackCanHitPlayer.Value.Remove(goId);
        }
    }

}