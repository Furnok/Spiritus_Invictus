using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class S_LanternDynamic : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Range(0, 3500)] private float minLightIntensity;
    [SerializeField, Range(0, 3500)] private float maxLightIntensity;
    [SerializeField, Range(15, 30)] private float minEmissiveIntensityIntensity = 15f;
    [SerializeField, Range(15, 30)] private float maxEmissiveIntensityIntensity = 30f;
    

    [Header("References")]
    [SerializeField] private Light lanternLight;
    [SerializeField] private MeshRenderer lanternMeshRenderer;

    [SerializeField] private RSO_PlayerCurrentConviction _currentPlayerConviction;
    [SerializeField] private SSO_PlayerConvictionData _playerConvictionData;

    //[Header("Inputs")]

    //[Header("Outputs")]

    private Material _lanternMaterialInstance;

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

    void UpdateLanternGlowAndLigh(float value)
    {
        var maxConvition = _playerConvictionData.Value.maxConviction;
        var t = value / 100 * maxConvition;

        var tLightIntensity = minLightIntensity + (t / 100 * (maxLightIntensity - minLightIntensity));
        var tEmissiveIntensity = minEmissiveIntensityIntensity + (t / 100 * (maxEmissiveIntensityIntensity - minEmissiveIntensityIntensity));

        lanternLight.intensity = tLightIntensity;

        HDMaterial.SetEmissiveIntensity(_lanternMaterialInstance, tEmissiveIntensity, EmissiveIntensityUnit.EV100);
    }
}