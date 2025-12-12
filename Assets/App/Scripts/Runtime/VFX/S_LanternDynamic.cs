using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class S_LanternDynamic : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Settings")]
    [SerializeField, Range(0, 3500)] private float minLightIntensity;

    [TabGroup("Settings")]
    [SerializeField, Range(0, 3500)] private float maxLightIntensity;

    [TabGroup("Settings")]
    [SerializeField, Range(15, 30)] private float minEmissiveIntensityIntensity = 15f;

    [TabGroup("Settings")]
    [SerializeField, Range(15, 30)] private float maxEmissiveIntensityIntensity = 30f;
    
    [TabGroup("References")]
    [Title("Light")]
    [SerializeField] private Light lanternLight;

    [TabGroup("References")]
    [Title("Mesh Renderer")]
    [SerializeField] private MeshRenderer lanternMeshRenderer;

    [TabGroup("Inputs")]
    [SerializeField] private RSO_PlayerCurrentConviction _currentPlayerConviction;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_PlayerConvictionData _playerConvictionData;

    private Material _lanternMaterialInstance = null;

    private void Awake()
    {
        lanternLight.intensity = 0f;
        _lanternMaterialInstance = lanternMeshRenderer.material;
    }

    private void OnEnable()
    {
        _currentPlayerConviction.onValueChanged += UpdateLanternGlowAndLigh;
    }

    private void OnDisable()
    {
        _currentPlayerConviction.onValueChanged -= UpdateLanternGlowAndLigh;
    }

    private void UpdateLanternGlowAndLigh(float value)
    {
        var maxConvition = _playerConvictionData.Value.maxConviction;
        var t = value / 100 * maxConvition;

        var tLightIntensity = minLightIntensity + (t / 100 * (maxLightIntensity - minLightIntensity));
        var tEmissiveIntensity = minEmissiveIntensityIntensity + (t / 100 * (maxEmissiveIntensityIntensity - minEmissiveIntensityIntensity));

        lanternLight.intensity = tLightIntensity;

        HDMaterial.SetEmissiveIntensity(_lanternMaterialInstance, tEmissiveIntensity, EmissiveIntensityUnit.EV100);
    }
}