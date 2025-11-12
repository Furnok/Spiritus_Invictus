using UnityEngine;

public class S_LookAtCamera : MonoBehaviour
{
    private void LateUpdate()
    {
        var lookPos = Camera.main.transform;

        transform.LookAt(lookPos);
    }
}