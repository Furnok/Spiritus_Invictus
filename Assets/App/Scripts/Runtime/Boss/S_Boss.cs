using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.Events;

public class AttackOwned
{
    public S_ClassBossAttack bossAttack;
    public float frequency;
    public float score;
}
public class S_Boss : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Boss Parameters")]
    [SerializeField] float bossDifficultyLevel;

    [TabGroup("Settings")]
    [SerializeField] float maxDifficultyLevel;

    [TabGroup("Settings")]
    [SerializeField] float difficultyGainPerSecond;

    [TabGroup("Settings")]
    [SerializeField] float difficultyLoseWhenPlayerHit;

    [TabGroup("Settings")]
    [SerializeField] float difficultyScore;

    [TabGroup("Settings")]
    [SerializeField] float frequencyScore;

    [TabGroup("Settings")]
    [SerializeField] float synergieScore;

    [TabGroup("Settings")]
    [Title("Animations Parameters")]
    [SerializeField, S_AnimationName] private string moveParam;

    [TabGroup("Settings")]
    [SerializeField, S_AnimationName] private string deathParam;

    [TabGroup("Settings")]
    [SerializeField, S_AnimationName] private string attackParam;

    [TabGroup("Settings")]
    [SerializeField, S_AnimationName] private string stunParam;

    [TabGroup("References")]
    [Title("Agent")]
    [SerializeField] private BehaviorGraphAgent behaviorAgent;

    [TabGroup("References")]
    [Title("Colliders")]
    [SerializeField] private Collider bodyCollider;

    [TabGroup("References")]
    [SerializeField] private Collider detectionCollider;

    [TabGroup("References")]
    [Title("Animator")]
    [SerializeField] private Animator animator;

    [TabGroup("References")]
    [Title("Scripts")]
    [SerializeField] private S_BossDetectionRange bossDetectionRange;

    [TabGroup("References")]
    [SerializeField] private S_EnemyHurt bossHurt;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerGettingHit rseOnPlayerGettingHit;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_BossData ssoBossData;

    [HideInInspector] public UnityEvent<float> onUpdateBossHealth = null;
    [HideInInspector] public UnityEvent onGetHit = null;

    private List<AttackOwned> listAttackOwneds = new List<AttackOwned>();
    private List<AttackOwned> listAttackOwnedPossibilities = new List<AttackOwned>();
    private S_EnumBossPhaseState currentPhaseState;
    private float health;
    private float maxHealth;
    private GameObject target = null;
    private bool isDead = false;
    private float lastValueHealth;
    private AttackOwned lastAttack;
    private AttackOwned currentAttack;
    private void Awake()
    {
        lastValueHealth = 101f;

        health = ssoBossData.Value.healthPhase1;
        maxHealth = ssoBossData.Value.healthPhase1;
        currentPhaseState = S_EnumBossPhaseState.Phase1;
        behaviorAgent.SetVariableValue<S_EnumBossPhaseState>("PhaseState", S_EnumBossPhaseState.Phase1);
        behaviorAgent.SetVariableValue<S_EnumBossState>("State", S_EnumBossState.Idle);
        behaviorAgent.SetVariableValue<Animator>("Animator", animator);
        behaviorAgent.SetVariableValue<float>("Health", health);
        behaviorAgent.SetVariableValue<float>("MoveSpeed", ssoBossData.Value.walkSpeed);
        behaviorAgent.SetVariableValue<Collider>("BodyCollider", bodyCollider);
        behaviorAgent.SetVariableValue<string>("DeathParam", deathParam);
        behaviorAgent.SetVariableValue<string>("MoveParam", moveParam);
        behaviorAgent.SetVariableValue<string>("StunParam", stunParam);
        behaviorAgent.SetVariableValue<string>("AttackParam", attackParam);

        foreach ( var bossAttack in ssoBossData.Value.listAttackPhase1)
        {
            var attackData = new AttackOwned
            {
                bossAttack = bossAttack,
                frequency = 0,
                score = 0,
            };
            listAttackOwneds.Add(attackData);
        }

        UpdateLastHealthValue();

        foreach (var name in listAttackOwnedPossibilities)
        {
            Debug.Log("List"+name.bossAttack.attackName);
        }
    }
    private void OnEnable()
    {
        bossDetectionRange.onTargetDetected.AddListener(SetTarget);
        bossHurt.onUpdateEnemyHealth.AddListener(UpdateHealth);
        rseOnPlayerGettingHit.action += LoseDifficultyLevel;
    }
    private void OnDisable()
    {
        bossDetectionRange.onTargetDetected.RemoveListener(SetTarget);
        bossHurt.onUpdateEnemyHealth.AddListener(UpdateHealth);
        rseOnPlayerGettingHit.action -= LoseDifficultyLevel;
    }
    private void Start()
    {
        ChooseAttack();
    }
    void SetTarget(GameObject newTarget)
    {
        if (newTarget == target || isDead)
        {
            return;
        }
        target = newTarget;

        behaviorAgent.SetVariableValue<GameObject>("Target", newTarget);
        if(target != null)
        {
            behaviorAgent.SetVariableValue<S_EnumBossState>("State", S_EnumBossState.Chase);
            StartCoroutine(GainDifficultyLevel());
        }
        else
        {
            behaviorAgent.SetVariableValue<S_EnumBossState>("State", S_EnumBossState.Idle);
        }
    }
    void AddListAttackPossible(AttackOwned bossAttack)
    {
        listAttackOwnedPossibilities.Add(bossAttack);
    }

    void UpdateHealth(float damage)
    {
        health = Mathf.Max(health - damage, 0);
        onUpdateBossHealth.Invoke(health);

        behaviorAgent.SetVariableValue<float>("Health", health);
        onGetHit.Invoke();

        UpdateLastHealthValue();
        foreach (var name in listAttackOwnedPossibilities)
        {
            Debug.Log(name.bossAttack.attackName);
        }

        if (currentPhaseState == S_EnumBossPhaseState.Phase2)
        {
            if(health <= 0)
            {
                isDead = true;

                behaviorAgent.SetVariableValue<S_EnumBossState>("State", S_EnumBossState.Death);
            }
        }
        else
        {
            if(health <= 0)
            {
                currentPhaseState = S_EnumBossPhaseState.Phase2;
                health = ssoBossData.Value.healthPhase2;
                maxHealth = ssoBossData.Value.healthPhase2;
                behaviorAgent.SetVariableValue<float>("Health", health);
                behaviorAgent.SetVariableValue<S_EnumBossPhaseState>("PhaseState", S_EnumBossPhaseState.Phase2);
            }
        }
    }
    void UpdateLastHealthValue()
    {
        var minValue = (health / maxHealth) * 100;

        SetListAttackPossible(minValue, lastValueHealth);

        lastValueHealth = minValue;
    }
    void SetListAttackPossible(float minValue, float maxValue)
    {
        foreach (var attack in listAttackOwneds)
        {
            if (attack.bossAttack.pvBossUnlock >= minValue && attack.bossAttack.pvBossUnlock < maxValue)
            {
                AddListAttackPossible(attack);
            }
        }
    }
    void LoseDifficultyLevel()
    {
        bossDifficultyLevel -= difficultyLoseWhenPlayerHit;
        bossDifficultyLevel = Mathf.Clamp(bossDifficultyLevel, 0,maxDifficultyLevel);
    }
    IEnumerator GainDifficultyLevel()
    {
        bossDifficultyLevel += difficultyGainPerSecond;
        bossDifficultyLevel = Mathf.Clamp(bossDifficultyLevel, 0, maxDifficultyLevel);
        yield return new WaitForSeconds(1);

        StartCoroutine(GainDifficultyLevel());
    }
    public void ChooseAttack()
    {
        var minAttackFrequency = listAttackOwnedPossibilities.Min(a => a.frequency);
        int roundDifficulty = Mathf.RoundToInt(bossDifficultyLevel);

        foreach(var attack in listAttackOwnedPossibilities)
        {
            if(attack.bossAttack.difficultyLevel == roundDifficulty)
            {
                attack.score+= difficultyScore;
            }

            if (attack.frequency == minAttackFrequency)
            {
                attack.score += frequencyScore;
            }

            if (lastAttack == null) continue;

            if (attack.bossAttack.listComboData[0].attackType != lastAttack.bossAttack.listComboData[^1].attackType)
            {
                attack.score+= synergieScore;
            }
        }

        var maxScore = listAttackOwnedPossibilities.Max(a => a.score);

        var bestAttacks = listAttackOwnedPossibilities
            .Where(a => a.score == maxScore)
            .ToList();

        var chosenAttack = bestAttacks[Random.Range(0, bestAttacks.Count)];

        currentAttack = chosenAttack;
        foreach(var attack in listAttackOwnedPossibilities)
        {
            attack.score = 0;
        }
        ExecuteAttack(currentAttack);
    }

    void ExecuteAttack(AttackOwned attack)
    {
        lastAttack = attack;
        attack.frequency++;
        Debug.Log(attack.bossAttack.attackName);
    }
}