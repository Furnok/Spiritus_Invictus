using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class S_UIContent : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Default")]
    [SerializeField] private Selectable defaultSelectable;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_DefaultSelectable rsoDefaultSelectable;

    private void OnEnable()
    {
        if (Gamepad.current != null)
        {
            defaultSelectable?.Select();
            defaultSelectable?.GetComponent<S_UISelectable>()?.Selected(defaultSelectable);
        }

        rsoDefaultSelectable.Value = defaultSelectable;
    }

    private void OnDisable()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}