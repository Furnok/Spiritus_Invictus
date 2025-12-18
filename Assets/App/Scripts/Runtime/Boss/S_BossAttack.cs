using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.WSA;

public class S_BossAttack : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Gethering Settings")]
    [SerializeField] private float backDistance;

    [TabGroup("Settings")]
    [SerializeField] private float jumpPower;

    [TabGroup("Settings")]
    [SerializeField] private float duration;

    [TabGroup("References")]
    [Title("Colliders")]
    [SerializeField] private Collider bodyCollider;

    [TabGroup("References")]
    [Title("RigidBody")]
    [SerializeField] private Rigidbody rbBody;

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
    [SerializeField] private NavMeshAgent bossNavMeshAgent;

    [TabGroup("References")]
    [Title("Animator")]
    [SerializeField] private Animator animator;

    [TabGroup("References")]
    [SerializeField, S_AnimationName("animator")] private string attackParam;

    [TabGroup("References")]
    [SerializeField, S_AnimationName("animator")] private string comboParam;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnExecuteAttack onExecuteAttack;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnPlayParticle rseOnPlayParticle;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnStopParticle rseOnStopParticle;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnEndAttack rseOnEndAttack;

    [HideInInspector] public Transform aimPointPlayer = null;

    private S_ClassBossAttack currentAttack = null;
    [HideInInspector] public AnimatorOverrideController overrideController = null;
    private Coroutine pingPongCoroutine = null;
    private Coroutine launch = null;

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

        Debug.Log("Gathering Jump");
        StartCoroutine(GatheringCoroutine(rbBody, aimPointPlayer.position, backDistance, jumpPower, duration, 1));

    }

    private void DoJumpAwayFrom(Rigidbody rb, Vector3 originPoint, float distance, float jumpPower, float duration, int numJumps = 1, bool keepY = true)
    {
        if (rb == null) return;
        bossNavMeshAgent.enabled = false;

        Vector3 dir = rb.position - originPoint;
        if (dir.sqrMagnitude <= 1e-6f) dir = rb.transform.forward;
        dir.Normalize();

        Vector3 target = rb.position + dir * distance;
        if (keepY) target.y = rb.position.y;

        Sequence seq = rb.DOJump(target, jumpPower, numJumps, duration)
            .SetEase(Ease.OutQuad).OnStart(() =>
            {
                Debug.Log("Play Jump Animation");
            })
            .OnComplete(() =>
            {
                Debug.Log("Stop Particle");
                bossNavMeshAgent.enabled = true;
                rseOnStopParticle.Call();
                if (launch != null)
                {
                    StopCoroutine(launch);
                    launch = null;
                }

                launch = StartCoroutine(S_Utils.Delay(3f, () => { 
                    rseOnPlayParticle.Call();
                    rseOnEndAttack.Call();
                }));
            });
    }

    private IEnumerator GatheringCoroutine(Rigidbody rb, Vector3 playerPos, float distance, float jumpPower, float duration, int numJumps, bool keepY = true)
    {
        int animNumb = 0;
        bossNavMeshAgent.enabled = false;

        Vector3 dir = rb.position - playerPos;
        if (dir.sqrMagnitude <= 1e-6f) dir = rb.transform.forward;
        dir.Normalize();

        Vector3 target = rb.position + dir * distance;
        if (keepY) target.y = rb.position.y;

        string overrideKey = (animNumb % 2 == 0) ? "AttackAnimation" : "AttackAnimation2";
        overrideController[overrideKey] = currentAttack.listComboData[animNumb].animation;

        Debug.Log("Play Jump Prepa Animation");
        animator.SetTrigger(animNumb == 0 ? attackParam : comboParam);
        animNumb++;
        yield return new WaitForSeconds(currentAttack.listComboData[animNumb].animation.length);

        Sequence seq = rb.DOJump(target, jumpPower, numJumps, duration)
            .SetEase(Ease.OutQuad).OnStart(() =>
            {
                overrideKey = (animNumb % 2 == 0) ? "AttackAnimation" : "AttackAnimation2";
                overrideController[overrideKey] = currentAttack.listComboData[animNumb].animation;

                Debug.Log("Play Jump Animation");
                animator.SetTrigger(animNumb == 0 ? attackParam : comboParam);
                animNumb++;
            })
            .OnComplete(() =>
            {
                StartCoroutine(GatheringAttack(animNumb));
            });
        yield return null;

    }
    private IEnumerator GatheringAttack(int value)
    {
        int animNumb = value;

        string overrideKey = (animNumb % 2 == 0) ? "AttackAnimation" : "AttackAnimation2";
        overrideController[overrideKey] = currentAttack.listComboData[animNumb].animation;

        Debug.Log("Play JumpFall Animation");
        animator.SetTrigger(animNumb == 0 ? attackParam : comboParam);
        animNumb++;

        yield return new WaitForSeconds(currentAttack.listComboData[animNumb].animation.length);

        overrideKey = (animNumb % 2 == 0) ? "AttackAnimation" : "AttackAnimation2";
        overrideController[overrideKey] = currentAttack.listComboData[animNumb].animation;

        Debug.Log("Stop Particle + Animation");
        animator.SetTrigger(animNumb == 0 ? attackParam : comboParam);
        animNumb++;

        rseOnStopParticle.Call();

        yield return new WaitForSeconds(currentAttack.listComboData[animNumb].animation.length);

        overrideKey = (animNumb % 2 == 0) ? "AttackAnimation" : "AttackAnimation2";
        overrideController[overrideKey] = currentAttack.listComboData[animNumb].animation;

        bossAttackData.SetAttackMode(currentAttack.listComboData[animNumb].attackData);

        if (currentAttack.listComboData[animNumb].showVFXAttackType) bossAttackData.VFXAttackType();

        Debug.Log("Play Particle + Animation + Set AttackMode");
        animator.SetTrigger(animNumb == 0 ? attackParam : comboParam);
        rseOnPlayParticle.Call();
        bossNavMeshAgent.enabled = true;
        rseOnEndAttack.Call();
        
    }
    private void WingsOfHell()
    {
    }
    #endregion
}