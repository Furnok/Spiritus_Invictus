using UnityEngine;
using UnityEngine.UI;

public class S_UIMainMenu : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool isActive;
    
    [Header("References")]
    [SerializeField] private Button buttonStart;
    [SerializeField] private Button buttonContinue;
    [SerializeField] private Button buttonSettings;

    [Header("Output")]
    [SerializeField] private RSE_OnUIInputEnabled rseOnUIInputEnabled;

    private void OnEnable()
    {
        rseOnUIInputEnabled.Call();

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