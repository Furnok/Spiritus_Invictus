using Sirenix.OdinInspector;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class S_CursorManager : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Cursor")]
    [SerializeField] private Texture2D handCursor;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnShowMouseCursor rseOnShowMouseCursor;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnHideMouseCursor rseOnHideMouseCursor;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnSetFocus rseOnSetFocus;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnResetCursor rseOnResetCursor;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnResetFocus rseOnResetFocus;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnMouseEnterUI rseOnMouseEnterUI;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnMouseLeaveUI rseOnMouseLeaveUI;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_Navigation rsoNavigation;

    private void Awake()
    {
        rsoNavigation.Value = new();
    }

    private void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
        rseOnShowMouseCursor.action += ShowMouseCursor;
        rseOnHideMouseCursor.action += HideMouseCursor;
        rseOnSetFocus.action += SetFocus;
        rseOnResetCursor.action += ResetCursor;
        rseOnResetFocus.action += ResetFocus;
        rseOnMouseEnterUI.action += MouseEnter;
        rseOnMouseLeaveUI.action += MouseLeave;
    }

    private void OnDisable() 
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
        rseOnShowMouseCursor.action -= ShowMouseCursor;
        rseOnHideMouseCursor.action -= HideMouseCursor;
        rseOnSetFocus.action -= SetFocus;
        rseOnResetCursor.action -= ResetCursor;
        rseOnResetFocus.action -= ResetFocus;
        rseOnMouseEnterUI.action -= MouseEnter;
        rseOnMouseLeaveUI.action -= MouseLeave;
    }

    private void Start()
    {
        if (Gamepad.current != null)
        {
            HideMouseCursor();
        }
        else
        {
            ShowMouseCursor();
        }
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device is Gamepad)
        {
            if (change == InputDeviceChange.Added)
            {
                HideMouseCursor();

                if (rsoNavigation.Value.selectableDefault != null)
                {
                    SetFocus(rsoNavigation.Value.selectableDefault);
                }
            }
            else if (change == InputDeviceChange.Removed)
            {
                ResetFocus();

                ShowMouseCursor();
            }
        }
    }

    private void ShowMouseCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    private void HideMouseCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    private void MouseEnter(Selectable uiElement)
    {
        if (uiElement.interactable)
        {
            Vector2 cursorOffset = new Vector2(handCursor.width / 3, handCursor.height / 40);

            Cursor.SetCursor(handCursor, cursorOffset, CursorMode.Auto);
        }
    }

    private void MouseLeave(Selectable uiElement)
    {
        if (uiElement.interactable)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    private void SetFocus(Selectable uiElement)
    {
        if (uiElement != null && uiElement.interactable && Gamepad.current != null)
        {
            uiElement.Select();
        }
    }

    private void ResetCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    private void ResetFocus()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
}
