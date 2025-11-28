using Sirenix.OdinInspector;
using UnityEngine;

public class S_PlayerAnimation : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Animator")]
    [SerializeField] private Animator playerAnimator;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnAnimationBoolValueChange rseOnAnimationBoolValueChange;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnAnimationFloatValueChange rseOnAnimationFloatValueChange;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnAnimationTriggerValueChange rseOnAnimationTriggerValueChange;

    private void OnEnable()
    {
        rseOnAnimationFloatValueChange.action += AnimatorSetFloatValue;
        rseOnAnimationBoolValueChange.action += AnimatorSetBoolValue;
        rseOnAnimationTriggerValueChange.action += AnimatorSetTriggerValue;
    }

    private void OnDisable()
    {
        rseOnAnimationFloatValueChange.action -= AnimatorSetFloatValue;
        rseOnAnimationBoolValueChange.action -= AnimatorSetBoolValue;
        rseOnAnimationTriggerValueChange.action -= AnimatorSetTriggerValue;
    }

    private void AnimatorSetBoolValue(string parameterName, bool value)
    {
        playerAnimator.SetBool(parameterName, value);
    }

    private void AnimatorSetFloatValue(string parameterName, float value)
    {
        playerAnimator.SetFloat(parameterName, value);
    }

    private void AnimatorSetTriggerValue(string parameterName)
    {
        playerAnimator.SetTrigger(parameterName);
    }
}