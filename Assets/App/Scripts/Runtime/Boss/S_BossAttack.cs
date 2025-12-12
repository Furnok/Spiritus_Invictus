using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_BossAttack : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Waves")]
    [SuffixLabel("s", Overlay = true)]
    [SerializeField] private float timeBetweenWaves;

    [TabGroup("References")]
    [Title("Colliders")]
    [SerializeField] private Collider bodyCollider;

    [TabGroup("References")]
    [Title("Animator")]
    [SerializeField] private Animator animator;

    [TabGroup("References")]
    [Title("Animation Parameters")]
    [SerializeField, S_AnimationName("animator")] private string ichimanji;

    [TabGroup("References")]
    [SerializeField, S_AnimationName("animator")] private string makashi;

    [TabGroup("References")]
    [SerializeField, S_AnimationName("animator")] private string margit;

    [TabGroup("References")]
    [SerializeField, S_AnimationName("animator")] private string valkyrie;

    [TabGroup("References")]
    [SerializeField, S_AnimationName("animator")] private string makashiSpe;

    [TabGroup("References")]
    [SerializeField, S_AnimationName("animator")] private string ataru;

    [TabGroup("References")]
    [SerializeField, S_AnimationName("animator")] private string vaapad;

    [TabGroup("References")]
    [SerializeField, S_AnimationName("animator")] private string genichiro;

    [TabGroup("References")]
    [SerializeField, S_AnimationName("animator")] private string simon;

    [TabGroup("References")]
    [SerializeField, S_AnimationName("animator")] private string dualliste;

    [TabGroup("References")]
    [SerializeField, S_AnimationName("animator")] private string pingPong;

    [TabGroup("References")]
    [SerializeField, S_AnimationName("animator")] private string waves;

    [TabGroup("References")]
    [SerializeField, S_AnimationName("animator")] private string balls;

    [TabGroup("References")]
    [SerializeField, S_AnimationName("animator")] private string genion;

    [TabGroup("References")]
    [SerializeField, S_AnimationName("animator")] private string gathering;

    [TabGroup("References")]
    [SerializeField, S_AnimationName("animator")] private string wingsOfHell;

    [TabGroup("References")]
    [Title("Projectile")]
    [SerializeField] private S_BossProjectile bossProjectile;

    [TabGroup("References")]
    [SerializeField] private GameObject projectilePingPongSpawn;

    [TabGroup("References")]
    [Title("Waves")]
    [SerializeField] private GameObject wavesPrefabs;

    [TabGroup("References")]
    [SerializeField] private GameObject middle;

    [TabGroup("References")]
    [SerializeField] private List<GameObject> wavesSpawn = new List<GameObject>();

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

    private Coroutine rotateWavesCoroutine = null;

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
            case "Ichimanji":
                Ichimanji();
                break;
            case "Makashi":
                Makashi();
                break;
            case "Margit":
                Margit();
                break;
            case "Valkyrie":
                Valkyrie();
                break;
            case "MakashiSpe":
                MakashiSpe();
                break;
            case "Ataru":
                Ataru();
                break;
            case "Vaapad":
                Vaapad();
                break;
            case "Genichiro":
                Genichiro();
                break;
            case "Simon":
                Simon();
                break;
            case "Dualliste":
                Dualliste();
                break;
            case "PingPong":
                PingPong();
                break;
            case "Waves":
                Waves();
                break;
            case "Balls":
                Balls();
                break;
            case "Genion":
                Genion();
                break;
            case "Gathering":
                Gathering();
                break;
            case "WingsOfHell":
                WingsOfHell();
                break;
        }
    }

    #region Attack Phase 1
    private void Ichimanji()
    {
        animator.SetTrigger(ichimanji);
    }

    private void Makashi()
    {
        animator.SetTrigger(makashi);
    }

    private void Margit()
    {
        animator.SetTrigger(margit);
    }

    private void Valkyrie()
    {
        animator.SetTrigger(valkyrie);
    }

    private void MakashiSpe()
    {
        animator.SetTrigger(makashiSpe);
    }

    private void Ataru()
    {
        animator.SetTrigger(ataru);
    }
    #endregion

    #region Attack Phase 2
    private void Vaapad()
    {
        animator.SetTrigger(vaapad);
    }

    private void Genichiro()
    {
        animator.SetTrigger(genichiro);
    }

    private void Simon()
    {
        animator.SetTrigger(simon);
    }

    private void Dualliste()
    {
        animator.SetTrigger(dualliste);
    }

    private void PingPong()
    {
        animator.SetTrigger(pingPong);

        S_BossProjectile projectileInstance = Instantiate(bossProjectile, projectilePingPongSpawn.transform.position, Quaternion.identity);
        projectileInstance.Initialize(aimPointBoss, aimPointPlayer, currentAttack.listComboData[0].attackData);
    }

    private void Waves()
    {
        StartCoroutine(StopWaves(currentAttack.attackTime));

        boss.transform.DOMove(middle.transform.position, 0.5f).OnComplete(() =>
        {
            animator.SetTrigger(waves);

            if (rotateWavesCoroutine != null)
            {
                StopCoroutine(rotateWavesCoroutine);
                rotateWavesCoroutine = null;
            }

            rotateWavesCoroutine = StartCoroutine(RotateWavesSpawn());
        });  
    }

    private IEnumerator RotateWavesSpawn()
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

    private IEnumerator StopWaves(float delay)
    {
        yield return new WaitForSeconds(delay);

        StopCoroutine(rotateWavesCoroutine);
        rotateWavesCoroutine = null;

    }

    private void SpawnWaves()
    {
        foreach (GameObject wavesSpawn in wavesSpawn) Instantiate(wavesPrefabs, wavesSpawn.transform.position, Quaternion.identity);
    }

    private void Balls()
    {
        animator.SetTrigger(balls);
    }

    private void Genion()
    {
        animator.SetTrigger(genion);
    }

    private void Gathering()
    {
        animator.SetTrigger(gathering);
    }

    private void WingsOfHell()
    {
        animator.SetTrigger(wingsOfHell);
    }
    #endregion
}