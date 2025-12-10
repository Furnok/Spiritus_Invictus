using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class S_ClassBossData
{
    [Title("General Settings")]
    public float healthPhase1 = 0;

    public float healthPhase2 = 0;

    public float walkSpeed = 0;

    public float runSpeed = 0;

    [Title("Chase")]
    public float distanceToChase = 0;

    [SuffixLabel("%", Overlay = true)]
    public float distanceToRun = 0;

    [Title("Strafe")]
    public float strafeChangeInterval = 1.5f;

    public float strafeRadius = 5f;

    public float strafeDistance = 2f;

    public float rotationSpeed = 6f;

    [Title("Combat")]
    public float bossDifficultyLevel;

    public float maxDifficultyLevel;

    public float difficultyGainPerSecond;

    public float difficultyLoseWhenPlayerHit;

    public float difficultyScore;

    public float frequencyScore;

    public float synergieScore;

    [SuffixLabel("s", Overlay = true)]
    public float minTimeChooseAttack;

    [SuffixLabel("s", Overlay = true)]
    public float maxTimeChooseAttack;

    [SuffixLabel("s", Overlay = true)]
    public float rotationTime = 0;

    [Title("Animations")]
    public AnimatorOverrideController controllerOverride = null;

    public List<S_ClassBossAttack> listAttackPhase1 = new();

    public List<S_ClassBossAttack> listAttackPhase2 = new();
}