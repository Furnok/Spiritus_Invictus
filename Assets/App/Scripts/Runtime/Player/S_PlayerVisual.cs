using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class S_PlayerVisual : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Mesh Renderers")]
    [SerializeField] private List<Renderer> playerMeshRenderer;

    [TabGroup("References")]
    [SerializeField] private MeshRenderer lanternMeshRenderer;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnUpdateVisibility rseOnUpdateVisibility;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnUpdateEmissiveIntensity rseOnUpdateEmissiveIntensity;

    private List<Material> playerMaterialInstance = new();

    private Material lanternMaterialInstance = null;

    private void OnEnable()
    {
        rseOnUpdateVisibility.action += UpdateVisibility;
        rseOnUpdateEmissiveIntensity.action += UpdateEmissiveIntensity;

        playerMaterialInstance.Clear();

        foreach (Renderer renderer in playerMeshRenderer)
        {
            playerMaterialInstance.Add(renderer.material);
        }

        lanternMaterialInstance = lanternMeshRenderer.material;
    }

    private void OnDisable()
    {
        rseOnUpdateVisibility.action -= UpdateVisibility;
        rseOnUpdateEmissiveIntensity.action -= UpdateEmissiveIntensity;
    }

    private void UpdateVisibility(float visibility)
    {
        foreach (Material material in playerMaterialInstance)
        {
            var color = material.color;
            color.a = visibility;
            material.color = color;
        }

        var color2 = lanternMaterialInstance.color;
        color2.a = visibility;
        lanternMaterialInstance.color = color2;
    }

    private void UpdateEmissiveIntensity(float intensity)
    {
        HDMaterial.SetEmissiveIntensity(lanternMaterialInstance, intensity, EmissiveIntensityUnit.EV100);
    }
}