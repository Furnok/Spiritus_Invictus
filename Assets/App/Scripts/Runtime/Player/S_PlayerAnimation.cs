using UnityEngine;

public class S_PlayerAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator playerAnimator;

    [Header("Input")]
    [SerializeField] private RSE_OnAnimationBoolValueChange rseOnAnimationBoolValueChange;
    [SerializeField] private RSE_OnAnimationFloatValueChange rseOnAnimationFloatValueChange;
    [SerializeField] RSE_OnAnimationTriggerValueChange rseOnAnimationTriggerValueChange;

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