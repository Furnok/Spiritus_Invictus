using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class S_UIContent : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Default")]
    [SerializeField] private Selectable defaultFocusSelectable;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_Navigation rsoNavigation;

    private void OnEnable()
    {
        StartCoroutine(S_Utils.DelayFrame(() => rsoNavigation.Value.selectableDefault = defaultFocusSelectable));

        if (Gamepad.current != null && rsoNavigation.Value.selectableFocus == null)
        {
            StartCoroutine(S_Utils.DelayFrame(() => defaultFocusSelectable?.Select()));
        }
    }

    private void OnDisable()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}