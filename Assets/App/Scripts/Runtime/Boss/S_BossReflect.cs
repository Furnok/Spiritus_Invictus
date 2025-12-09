using Sirenix.OdinInspector;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class S_BossReflect : MonoBehaviour
{
    //[Header("Settings")]

    [TabGroup("References")]
    [Title("Body")]
    [SerializeField] private GameObject body;

    //[Header("Inputs")]

    //[Header("Outputs")]
    private void OnTriggerEnter(Collider other)
    {
        // Cherche si l'objet entrant est un projectile réfléchissable
        if (other.TryGetComponent<I_ReflectableProjectile>(out var reflectable))
        {
            if (reflectable.CanReflect() == false)
                return;

            Debug.Log("Boss reflected a projectile!" + reflectable.CanReflect());

            var reflectOwner = body != null ? body.transform : transform;
            reflectable.Reflect(reflectOwner);
        }
    }
}