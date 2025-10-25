using Sirenix.OdinInspector;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class S_Version : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Text")]
    [SerializeField] private TextMeshProUGUI versionText;

    private string lastVersionText = "";

    #if UNITY_EDITOR
    private void OnEnable()
    {
        if (!Application.isPlaying)
        {
            EditorApplication.update += UpdateVersionLabelEditor;
        }
    }

    private void OnDisable()
    {
        if (!Application.isPlaying)
        {
            EditorApplication.update -= UpdateVersionLabelEditor;
        }
    }

    private void UpdateVersionLabelEditor()
    {
        UpdateVersionLabel();
    }
    #endif

    private void Start()
    {
        if (Application.isPlaying)
        {
            UpdateVersionLabel();
        }
    }

    private void UpdateVersionLabel()
    {
        if (versionText == null)
            return;

        string rawVersion = !string.IsNullOrWhiteSpace(Application.version) ? Application.version : "0.0.0";

        string normalizedVersion = NormalizeVersion(rawVersion);

        if (normalizedVersion != lastVersionText)
        {
            versionText.text = $"Version: {normalizedVersion}";
            lastVersionText = normalizedVersion;
        }
    }

    private string NormalizeVersion(string version)
    {
        var parts = version.Split('.').Where(part => !string.IsNullOrEmpty(part)).ToArray();

        string[] normalized = new string[3] { "0", "0", "0" };

        for (int i = 0; i < Mathf.Min(parts.Length, 3); i++)
        {
            normalized[i] = parts[i];
        }

        return string.Join(".", normalized);
    }
}