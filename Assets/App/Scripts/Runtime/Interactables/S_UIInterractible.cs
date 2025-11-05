using Sirenix.OdinInspector;
using UnityEngine;

public class S_UIInterractible : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Transform")]
    [SerializeField] private Transform content;

    private void LateUpdate()
    {
        var lookPos = Camera.main.transform;

        content.LookAt(lookPos);
    }
}