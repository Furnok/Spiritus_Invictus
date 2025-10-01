using Sirenix.OdinInspector;
using UnityEngine;

public class S_Reminiscence : MonoBehaviour
{
    [TabGroup("General Settings", "Settings")]
    [BoxGroup("General Settings/Settings/General Settings")]
    [S_TagName][SerializeField] private string tagPlayer;

    [TabGroup("General Settings", "Settings")]
    [BoxGroup("General Settings/Settings/General Settings")]
    [ShowIf("HasCamerasAvailable")]
    [ValueDropdown("GetCameraIndexDropdown")]
    [SerializeField] private int cameraIndex;

    [TabGroup("General Settings", "References")]
    [BoxGroup("General Settings/References/Input")]
    [SerializeField] private RSE_OnPlayerInteract rseOnPlayerInteract;

    [TabGroup("General Settings", "References")]
    [BoxGroup("General Settings/References/Output")]
    [SerializeField] private RSE_CameraCinematic rseCameraCinematic;

    private bool HasCamerasAvailable()
    {
        var managers = FindObjectsByType<S_CameraManager>(FindObjectsSortMode.None);

        foreach (var manager in managers)
        {
            if (manager != null && manager.GetListCameraCinematic() != null && manager.GetListCameraCinematic().Count > 0) return true;
        }

        return false;
    }

    private ValueDropdownList<int> GetCameraIndexDropdown()
    {
        var dropdown = new ValueDropdownList<int>();

        var managers = FindObjectsByType<S_CameraManager>(FindObjectsSortMode.None);

        int globalIndex = 0;

        foreach (var manager in managers)
        {
            if (manager == null || manager.GetListCameraCinematic() == null) continue;

            for (int i = 0; i < manager.GetListCameraCinematic().Count; i++)
            {
                var cam = manager.GetListCameraCinematic()[i];
                if (cam == null) continue;

                dropdown.Add($"{cam.name}", globalIndex);

                globalIndex++;
            }
        }

        return dropdown;
    }

    private void OnDisable()
    {
        rseOnPlayerInteract.action -= Interract;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagPlayer))
        {
            rseOnPlayerInteract.action += Interract;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagPlayer))
        {
            rseOnPlayerInteract.action -= Interract;
        }
    }

    private void Interract()
    {
        rseCameraCinematic.Call(cameraIndex);
    }
}