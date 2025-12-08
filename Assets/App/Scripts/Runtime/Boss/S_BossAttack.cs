using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.AI;

public class S_BossAttack : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Ichimanji")]
    [SerializeField, S_AnimationName("animator")] private string ichimanji;

    [TabGroup("Settings")]
    [Title("Makashi")]
    [SerializeField, S_AnimationName("animator")] private string makashi;

    [TabGroup("Settings")]
    [Title("Margit")]
    [SerializeField, S_AnimationName("animator")] private string margit;

    [TabGroup("Settings")]
    [Title("Valkyrie")]
    [SerializeField, S_AnimationName("animator")] private string valkyrie;

    [TabGroup("Settings")]
    [Title("Makashi Special")]
    [SerializeField, S_AnimationName("animator")] private string makashiSpe;

    [TabGroup("Settings")]
    [Title("Ataru")]
    [SerializeField, S_AnimationName("animator")] private string ataru;

    [TabGroup("Settings")]
    [Title("Vaapad")]
    [SerializeField, S_AnimationName("animator")] private string vaapad;

    [TabGroup("Settings")]
    [Title("Genichiro")]
    [SerializeField, S_AnimationName("animator")] private string genichiro;

    [TabGroup("Settings")]
    [Title("Simon")]
    [SerializeField, S_AnimationName("animator")] private string simon;

    [TabGroup("Settings")]
    [Title("Dualliste")]
    [SerializeField, S_AnimationName("animator")] private string dualliste;

    [TabGroup("Settings")]
    [Title("PingPong")]
    [SerializeField, S_AnimationName("animator")] private string pingPong;
    [TabGroup("Settings")]
    [SerializeField] private GameObject projectilePingPongSpawn;
    [TabGroup("Settings")]
    [Title("Waves")]
    [SerializeField, S_AnimationName("animator")] private string waves;
    [TabGroup("Settings")]
    [SerializeField] private GameObject wavesPrefabs;
    [TabGroup("Settings")]
    [SerializeField] private List<GameObject> wavesSpawn = new List<GameObject>();
    [TabGroup("Settings")]
    [SerializeField] private GameObject middle;
    [TabGroup("Settings")]
    [SerializeField] private float timeBetweenWaves;
    private Coroutine rotateWavesCoroutine;

    [TabGroup("Settings")]
    [Title("Balls")]
    [SerializeField, S_AnimationName("animator")] private string balls;

    [TabGroup("Settings")]
    [Title("Genion")]
    [SerializeField, S_AnimationName("animator")] private string genion;

    [TabGroup("Settings")]
    [Title("Gathering")]
    [SerializeField, S_AnimationName("animator")] private string gathering;

    [TabGroup("Settings")]
    [Title("WingsOfHell")]
    [SerializeField, S_AnimationName("animator")] private string wingsOfHell;

    [TabGroup("References")]
    [Title("Colliders")]
    [SerializeField] private Collider bodyCollider;

    [TabGroup("References")]
    [Title("Animator")]
    [SerializeField] private Animator animator;

    [TabGroup("References")]
    [SerializeField] private S_BossProjectile bossProjectile;

    [TabGroup("References")]
    [Title("Boss")]
    [SerializeField] private GameObject boss;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnExecuteAttack onExecuteAttack;

    public Transform aimPoint;

    private S_ClassBossAttack currentAttack;
    private void OnEnable()
    {
        onExecuteAttack.action += DoAttackChoose;
    }
    private void OnDisable()
    {
        onExecuteAttack.action -= DoAttackChoose;
    }

    void DoAttackChoose(S_ClassBossAttack attack)
    {
        currentAttack = attack;

        if (currentAttack.attackName=="Ichimanji")
        {
            Ichimanji();
        }
        else if(currentAttack.attackName=="Makashi")
        {
            Makashi();
        }
        else if(currentAttack.attackName=="Margit")
        {
            Margit();
        }
        else if(currentAttack.attackName=="Valkyrie")
        {
            Valkyrie();
        }
        else if(currentAttack.attackName=="MakashiSpe")
        {
            MakashiSpe();
        }
        else if(currentAttack.attackName=="Ataru")
        {
            Ataru();
        }
        else if(currentAttack.attackName=="Vaapad")
        {
            Vaapad();
        }
        else if(currentAttack.attackName=="Genichiro")
        {
            Genichiro();
        }
        else if(currentAttack.attackName=="Simon")
        {
            Simon();
        }
        else if(currentAttack.attackName=="Dualliste")
        {
            Dualliste();
        }
        else if(currentAttack.attackName=="PingPong")
        {
            PingPong();
        }
        else if(currentAttack.attackName=="Waves")
        {
            Waves();
        }
        else if(currentAttack.attackName=="Balls")
        {
            Balls();
        }
        else if(currentAttack.attackName=="Genion")
        {
            Genion();
        }
        else if(currentAttack.attackName=="Gathering")
        {
            Gathering();
        }
        else if(currentAttack.attackName=="WingsOfHell")
        {
            WingsOfHell();
        }
    }
    #region Attack Phase 1
    void Ichimanji()
    {
        animator.SetTrigger(ichimanji);
    }
    void Makashi()
    {
        animator.SetTrigger(makashi);
    }
    void Margit()
    {
        animator.SetTrigger(margit);
    }
    void Valkyrie()
    {
        animator.SetTrigger(valkyrie);
    }
    void MakashiSpe()
    {
        animator.SetTrigger(makashiSpe);
    }
    void Ataru()
    {
        animator.SetTrigger(ataru);
    }
    #endregion

    #region Attack Phase 2
    void Vaapad()
    {
        animator.SetTrigger(vaapad);
    }
    void Genichiro()
    {
        animator.SetTrigger(genichiro);
    }
    void Simon()
    {
        animator.SetTrigger(simon);
    }

    void Dualliste()
    {
        animator.SetTrigger(dualliste);
    }
    void PingPong()
    {
        for(int i=0; i< currentAttack.listComboData.Count; i++)
        {
            animator.SetTrigger(pingPong);
            if (currentAttack.listComboData[i].attackType == S_EnumEnemyAttackType.Projectile)
            {
                S_BossProjectile projectileInstance = Instantiate(bossProjectile, projectilePingPongSpawn.transform.position, Quaternion.identity);
                projectileInstance.Initialize(bodyCollider.transform, aimPoint, currentAttack.listComboData[i]);
            }
        }
    }
    void Waves()
    {
        StartCoroutine(StopWaves(currentAttack.attackTime));
        boss.transform.DOMove(middle.transform.position, 0.5f).OnComplete(() =>
        {
            animator.SetTrigger(waves);
            if(rotateWavesCoroutine!=null)
            {
                StopCoroutine(rotateWavesCoroutine);
                rotateWavesCoroutine = null;
            }
            rotateWavesCoroutine = StartCoroutine(RotateWavesSpawn());
        });  
    }
    IEnumerator RotateWavesSpawn()
    {
        middle.transform.rotation = Quaternion.Euler(0, 0, 0);
        SpawnWaves();
        yield return new WaitForSeconds(timeBetweenWaves);
        middle.transform.rotation = Quaternion.Euler(0, 45, 0);
        SpawnWaves();
        yield return new WaitForSeconds(timeBetweenWaves);
        if (rotateWavesCoroutine != null)
        {
            StopCoroutine(rotateWavesCoroutine);
            rotateWavesCoroutine = null;
        }
        rotateWavesCoroutine = StartCoroutine(RotateWavesSpawn());
    }
    IEnumerator StopWaves(float delay)
    {
        yield return new WaitForSeconds(delay);
        StopCoroutine(rotateWavesCoroutine);
        rotateWavesCoroutine = null;

    }
    void SpawnWaves()
    {
        foreach (GameObject wavesSpawn in wavesSpawn)
        {
            Instantiate(wavesPrefabs, wavesSpawn.transform.position, Quaternion.identity);
        }
    }
    void Balls()
    {
        animator.SetTrigger(balls);
    }
    void Genion()
    {
        animator.SetTrigger(genion);
    }
    void Gathering()
    {
        animator.SetTrigger(gathering);
    }
    void WingsOfHell()
    {
        animator.SetTrigger(wingsOfHell);
    }
    #endregion
}