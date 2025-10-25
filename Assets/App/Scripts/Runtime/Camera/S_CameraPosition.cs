using Sirenix.OdinInspector;
using UnityEngine;

public class S_CameraPosition : MonoBehaviour
{
    [TabGroup("Outputs")]
    [SerializeField] private RSO_CameraRotation rsoCameraRotation;

    private void OnDisable()
    {
        rsoCameraRotation.Value = new Quaternion();
    }

    private void Update()
    {
        CameraRotation();
    }

    private void CameraRotation()
    {
        if (rsoCameraRotation != null)
        {
            rsoCameraRotation.Value = transform.rotation;
        }
    }
}