using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_DodgeableAreaDetector : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Filter")]
    [SerializeField, S_TagName] private string tagHit;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_AttackDataInDodgeableArea _attackDataInDodgeableArea;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_AttackCanHitPlayer _attackCanHitPlayer;

    private Dictionary<I_AttackProvider, Collider> _tempAttackDataInDodgeableArea = new();

    private void Awake()
    {
        _attackDataInDodgeableArea.Value = new S_SerializableDictionary<int, S_StructEnemyAttackData>();
        _attackDataInDodgeableArea.Value.Clear();

        _attackCanHitPlayer.Value = new S_SerializableDictionary<int, S_StructEnemyAttackData>();
        _attackCanHitPlayer.Value.Clear();
        _tempAttackDataInDodgeableArea.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagHit) && other.TryGetComponent(out I_AttackProvider attack) && other.enabled == true)
        {
            var goId = other.gameObject.GetInstanceID();
            ref var attackData = ref attack.GetAttackData();
            attackData.goSourceId = goId;

            if (_tempAttackDataInDodgeableArea.ContainsKey(attack) == false)
            {
                _tempAttackDataInDodgeableArea.Add(attack, other);
            }

            if (_attackDataInDodgeableArea.Value == null || _attackDataInDodgeableArea.Value.ContainsKey(goId) || attackData.attackType != S_EnumEnemyAttackType.Dodgeable)
            {

            }
            else
            {
                _attackDataInDodgeableArea.Value.Add(goId, attack.GetAttackData());
            }


            if (_attackCanHitPlayer.Value == null || _attackCanHitPlayer.Value.ContainsKey(goId) || attackData.attackType != S_EnumEnemyAttackType.Dodgeable)
            {

            }
            else
            {
                _attackCanHitPlayer.Value.Add(goId, attack.GetAttackData());

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagHit) && other.TryGetComponent(out I_AttackProvider attack))
        {
            var goId = other.gameObject.GetInstanceID();
            if (_attackDataInDodgeableArea.Value.ContainsKey(goId) == true)
            {
                _attackDataInDodgeableArea.Value.Remove(goId);
            }

            if (_attackCanHitPlayer.Value.ContainsKey(goId) == true)
            {
                _attackCanHitPlayer.Value.Remove(goId);
            }

            if (_tempAttackDataInDodgeableArea.ContainsKey(attack) == true)
            {
                _tempAttackDataInDodgeableArea.Remove(attack);
            }
        }
    }

    private void LateUpdate()
    {
        if (_tempAttackDataInDodgeableArea.Count <= 0) return;

        var toRemove = new List<I_AttackProvider>();

        foreach (var kvp in _tempAttackDataInDodgeableArea)
        {
            if (kvp.Value == null || kvp.Value.enabled == false)
            {
                if (kvp.Value != null)
                {
                    var goId = kvp.Value.gameObject.GetInstanceID();
                    if (_attackDataInDodgeableArea.Value != null && _attackDataInDodgeableArea.Value.ContainsKey(goId) == true)
                    {
                        _attackDataInDodgeableArea.Value.Remove(goId);
                    }

                    if (_attackCanHitPlayer.Value != null && _attackCanHitPlayer.Value.ContainsKey(goId) == true)
                    {
                        _attackCanHitPlayer.Value.Remove(goId);
                    }
                }

                toRemove.Add(kvp.Key);
            }
        }

        foreach (var key in toRemove)
        {
            _tempAttackDataInDodgeableArea.Remove(key);
        }
    }
}