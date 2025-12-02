using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
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
    [TabGroup("Settings")][SerializeField] private float timeEchoSimon = 1f;
    [TabGroup("Settings")][SerializeField] private GameObject echoSimonWeapon;

    [TabGroup("Settings")]
    [Title("Dualliste")]
    [SerializeField, S_AnimationName("animator")] private string dualliste;

    [TabGroup("Settings")]
    [Title("PingPong")]
    [SerializeField, S_AnimationName("animator")] private string pingPong;

    [TabGroup("Settings")]
    [Title("Waves")]
    [SerializeField, S_AnimationName("animator")] private string waves;

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
    [Title("Animator")]
    [SerializeField] private Animator animator;

    [TabGroup("References")]
    [SerializeField] private Animator animatorEcho;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnExecuteAttack onExecuteAttack;

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
        StartCoroutine(PlayEchoAnimation());
    }

    IEnumerator PlayEchoAnimation()
    {
        yield return new WaitForSeconds(timeEchoSimon);
        echoSimonWeapon.SetActive(true);
        animatorEcho.SetTrigger(simon);
        StartCoroutine(DisableEchoWeaponAfterDelay(currentAttack.attackTime));
    }
    IEnumerator DisableEchoWeaponAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay-1);
        echoSimonWeapon.SetActive(false);
    }

    void Dualliste()
    {
        animator.SetTrigger(dualliste);
    }
    void PingPong()
    {
        animator.SetTrigger(pingPong);
    }
    void Waves()
    {
        animator.SetTrigger(waves);
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