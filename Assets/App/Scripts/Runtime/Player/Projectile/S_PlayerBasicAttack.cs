using UnityEngine;

public class S_PlayerBasicAttack : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector3 attackOffset;

    [Header("Input")]
    [SerializeField] private RSE_OnPlayerAttackInput rseOnPlayerAttack;

    [Header("Output")]
    [SerializeField] private SSO_BasicAttackDelayIncantation ssoDelayIncantationAttack;
    [SerializeField] private RSE_OnSpawnProjectile rseOnSpawnProjectile;
    [SerializeField] private RSO_PlayerIsTargeting rsoPlayerIsTargeting;

    private bool _canAttack = true;

    private void OnEnable()
    {
        rseOnPlayerAttack.action += OnPlayerAttackInput;
    }

    private void OnDisable()
    {
        rseOnPlayerAttack.action -= OnPlayerAttackInput;
    }

    private void OnPlayerAttackInput()
    {
        if (_canAttack == false) return;

        S_StructProjectileData attackposition = new S_StructProjectileData
        {
            locationSpawn = transform.position + transform.TransformVector(attackOffset),
            direction = transform.forward
        };

        _canAttack = false;

        StartCoroutine(S_Utils.Delay(ssoDelayIncantationAttack.Value, () =>
        {
            rseOnSpawnProjectile.Call(attackposition);

            _canAttack = true;
        }));
    }
}

