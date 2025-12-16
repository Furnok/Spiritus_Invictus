using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_BossAttack : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Colliders")]
    [SerializeField] private Collider bodyCollider;

    [TabGroup("References")]
    [Title("Projectile")]
    [SerializeField] private S_BossProjectile bossProjectile;

    [TabGroup("References")]
    [SerializeField] private GameObject projectilePingPongSpawn;

    [TabGroup("References")]
    [Title("Center")]
    [SerializeField] private Transform aimPointBoss;

    [TabGroup("References")]
    [Title("Boss")]
    [SerializeField] private GameObject boss;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnExecuteAttack onExecuteAttack;

    [HideInInspector] public Transform aimPointPlayer = null;

    private S_ClassBossAttack currentAttack = null;

    private void OnEnable()
    {
        onExecuteAttack.action += DoAttackChoose;
    }

    private void OnDisable()
    {
        onExecuteAttack.action -= DoAttackChoose;
    }

    private void DoAttackChoose(S_ClassBossAttack attack)
    {
        currentAttack = attack;

        switch (currentAttack.attackName)
        {
            case "Simon":
                Simon();
                break;
            case "Dualliste":
                Dualliste();
                break;
            case "PingPong":
                PingPong();
                break;
            case "Balls":
                Balls();
                break;
            case "Gathering":
                Gathering();
                break;
            case "WingsOfHell":
                WingsOfHell();
                break;
        }
    }

    #region Attack Phase 2
    private void Simon()
    {
    }

    private void Dualliste()
    {
    }

    private void PingPong()
    {
        S_BossProjectile projectileInstance = Instantiate(bossProjectile, projectilePingPongSpawn.transform.position, Quaternion.identity);
        projectileInstance.Initialize(aimPointBoss, aimPointPlayer, currentAttack.listComboData[0].attackData);
    }

    private void Balls()
    {
    }

    private void Gathering()
    {
    }

    private void WingsOfHell()
    {
    }
    #endregion
}