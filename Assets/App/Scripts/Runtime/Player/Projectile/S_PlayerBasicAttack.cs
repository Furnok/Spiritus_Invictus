using UnityEngine;

public class S_PlayerBasicAttack : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Vector3 _attackOffset;
    [SerializeField] SSO_BasicAttackDelayIncantation _delayIncantationAttack;

    [Header("Input")]
    [SerializeField] RSE_OnPlayerAttackInput _onPlayerAttack;

    [Header("Output")]
    [SerializeField] RSE_OnSpawnProjectile _onSpawnProjectile;

    [Header("RSO")]
    [SerializeField] RSO_PlayerIsTargeting _playerIsTargeting;
    

    bool _canAttack = true;

    private void OnEnable()
    {
        _onPlayerAttack.action += OnPlayerAttackInput;
    }

    private void OnDisable()
    {
        _onPlayerAttack.action -= OnPlayerAttackInput;
    }

    void OnPlayerAttackInput()
    {
        if (_canAttack == false) return;

        ProjectileInitializeData attackposition = new ProjectileInitializeData
        {
            locationSpawn = transform.position + transform.TransformVector(_attackOffset),
            direction = transform.forward
        };

        _canAttack = false;

        StartCoroutine(S_Utils.Delay(_delayIncantationAttack.Value, () =>
        {
            _onSpawnProjectile.Call(attackposition);

            _canAttack = true;
        }));
    }
}

