using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class S_UIMainMenu : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("TEMP ENABLE CONTINUE")]
    [SerializeField] private bool isActive;

    [TabGroup("References")]
    [Title("Buttons")]
    [SerializeField] private Button buttonStart;

    [TabGroup("References")]
    [SerializeField] private Button buttonContinue;

    [TabGroup("References")]
    [SerializeField] private Button buttonSettings;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnUIInputEnabled rseOnUIInputEnabled;

    private void OnEnable()
    {
        StartCoroutine(S_Utils.DelayFrame(() => rseOnUIInputEnabled.Call()));

        if (isActive)
        {
            buttonContinue.gameObject.SetActive(true);

            Navigation nav = buttonStart.navigation;
            nav.mode = Navigation.Mode.Explicit;

            nav.selectOnDown = buttonContinue;

            buttonStart.navigation = nav;

            Navigation nav2 = buttonSettings.navigation;
            nav2.mode = Navigation.Mode.Explicit;

            nav2.selectOnUp = buttonContinue;

            buttonSettings.navigation = nav2;
        }
    }
}