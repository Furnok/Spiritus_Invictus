using UnityEngine;

public class S_CameraPosition : MonoBehaviour
{
    [Header("Output")]
    [SerializeField] private RSO_CameraRotation rsoCameraRotation;

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