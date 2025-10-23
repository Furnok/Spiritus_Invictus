using Sirenix.OdinInspector;
using UnityEngine;

public class S_UICredits : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Default")]
    [SerializeField] private GameObject defaultWindow;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerPause rseOnPlayerPause;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnCloseWindow rseOnCloseWindow;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_Navigation rsoNavigation;

    private void OnEnable()
    {
        rseOnPlayerPause.action += Close;
    }

    private void OnDisable()
    {
        rseOnPlayerPause.action -= Close;
    }

    public void Close()
    {
        rseOnCloseWindow.Call(gameObject);

        if (rsoNavigation.Value.selectablePressOldWindow == null)
        {
            rsoNavigation.Value.selectableFocus = null;
            defaultWindow.SetActive(true);
        }
        else
        {
            rsoNavigation.Value.selectablePressOldWindow?.Select();
            rsoNavigation.Value.selectablePressOldWindow = null;
        }
    }
}