using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class S_UIConsole : MonoBehaviour
{
    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnShowMouseCursor rseOnShowMouseCursor;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_ConsoleDisplay rsoConsoleDisplay;

    private void OnEnable()
    {
        rsoConsoleDisplay.Value = true;

        if (Gamepad.current == null)
        {
            rseOnShowMouseCursor.Call();
        }
    }
}