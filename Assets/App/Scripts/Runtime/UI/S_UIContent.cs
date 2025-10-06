using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class S_UIContent : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Selectable defaultSelectable;

    [Header("Output")]
    [SerializeField] private RSO_DefaultSelectable rsoDefaultSelectable;

    private void OnEnable()
    {
        if (Gamepad.current != null)
        {
            defaultSelectable?.Select();
        }

        rsoDefaultSelectable.Value = defaultSelectable;
    }

    private void OnDisable()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}