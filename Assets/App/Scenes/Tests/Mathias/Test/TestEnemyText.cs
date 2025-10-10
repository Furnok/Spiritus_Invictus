using UnityEngine;

public class TestEnemyText : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] RSO_PlayerPosition _playerPosition;

    //[Header("Input")]

    //[Header("Output")]

    void LateUpdate()
    {
        //transform.rotation = Quaternion.LookRotation(_playerPosition.Value);

        var lookPos = _playerPosition.Value;
        lookPos.y = 3;
        transform.LookAt(lookPos);
    }
}