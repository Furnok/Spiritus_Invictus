using UnityEngine;

public class S_AnimationNameAttribute : PropertyAttribute 
{
    public string animatorFieldName;

    public S_AnimationNameAttribute(string animatorFieldName = null)
    {
        this.animatorFieldName = animatorFieldName;
    }
}