using UnityEngine;

public class TestEnemyText : MonoBehaviour
{
    private void LateUpdate()
    {
        var lookPos = Camera.main.transform;

        transform.LookAt(lookPos);
    }
}