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
    [Title("Scripts")]
    [SerializeField] private S_BossRootMotionModifier rootMotionModifier;

    [TabGroup("References")]
    [SerializeField] private S_BossAttackData bossAttackData;

    [TabGroup("References")]
    [Title("Center")]
    [SerializeField] private Transform aimPointBoss;

    [TabGroup("References")]
    [Title("Boss")]
    [SerializeField] private GameObject boss;

    [TabGroup("References")]
    [Title("Animator")]
    [SerializeField] private Animator animator;

    [TabGroup("References")]
    [SerializeField, S_AnimationName("animator")] private string attackParam;

    [TabGroup("References")]
    [SerializeField, S_AnimationName("animator")] private string comboParam;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnExecuteAttack onExecuteAttack;

    [HideInInspector] public Transform aimPointPlayer = null;

    private S_ClassBossAttack currentAttack = null;
    [HideInInspector] public AnimatorOverrideController overrideController = null;
    private Coroutine pingPongCoroutine = null;
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
        if(pingPongCoroutine  != null)
        {
            StopCoroutine(pingPongCoroutine);
            pingPongCoroutine = null;
        }
        pingPongCoroutine = StartCoroutine(PingPongCoroutine());  
    }

    private IEnumerator PingPongCoroutine()
    {
        yield return null;

        for (int i = 0; i < currentAttack.listComboData.Count; i++)
        {
            string overrideKey = (i % 2 == 0) ? "AttackAnimation" : "AttackAnimation2";
            overrideController[overrideKey] = currentAttack.listComboData[i].animation;

            rootMotionModifier.Setup(currentAttack.listComboData[i].rootMotionMultiplier);

            bossAttackData.SetAttackMode(currentAttack.listComboData[i].attackData);

            if (currentAttack.listComboData[i].showVFXAttackType) bossAttackData.VFXAttackType();

            animator.SetTrigger(i == 0 ? attackParam : comboParam);

            yield return new WaitForSeconds(currentAttack.listComboData[i].animation.length);

            if(currentAttack.listComboData[i].attackData.attackType == S_EnumEnemyAttackType.Projectile)
            {
                S_BossProjectile projectileInstance = Instantiate(bossProjectile, projectilePingPongSpawn.transform.position, Quaternion.identity);
                projectileInstance.Initialize(aimPointBoss, aimPointPlayer, currentAttack.listComboData[i].attackData);
            }
            yield return null;
        }
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