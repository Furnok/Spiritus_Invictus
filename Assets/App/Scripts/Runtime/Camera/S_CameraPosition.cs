using Sirenix.OdinInspector;
using UnityEngine;

public class S_CameraPosition : MonoBehaviour
{
    [TabGroup("Outputs")]
    [SerializeField] private RSO_CameraRotation rsoCameraRotation;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_InGame rsoInGame;

    private void OnDisable()
    {
        rsoCameraRotation.Value = new Quaternion();
    }

    private void Update()
    {
        if (rsoInGame.Value) CameraRotation();
    }

    private void CameraRotation()
    {
        if (rsoCameraRotation != null) rsoCameraRotation.Value = transform.rotation;
    }
}