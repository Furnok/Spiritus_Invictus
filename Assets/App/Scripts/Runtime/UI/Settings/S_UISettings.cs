using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class S_UISettings : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Default")]
    [SerializeField] private GameObject defaultWindow;

    [TabGroup("References")]
    [SerializeField] private GameObject defaultPanelSet;

    [TabGroup("References")]
    [Title("Buttons")]
    [SerializeField] private Button buttonGameplay;

    [TabGroup("References")]
    [SerializeField] private Button buttonGraphics;

    [TabGroup("References")]
    [SerializeField] private Button buttonAudio;

    [TabGroup("References")]
    [SerializeField] private Button buttonReturn;

    [TabGroup("References")]
    [Title("Selectables")]
    [SerializeField] private Selectable dropDownLanguages;

    [TabGroup("References")]
    [SerializeField] private Selectable dropDownResolutions;

    [TabGroup("References")]
    [SerializeField] private Selectable toggleFullscreen;

    [TabGroup("References")]
    [SerializeField] private Selectable sliderMainVolume;

    [TabGroup("References")]
    [SerializeField] private Selectable sliderUIVolume;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerPause rseOnPlayerPause;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnCloseWindow rseOnCloseWindow;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_Navigation rsoNavigation;

    private GameObject currentPanelSet = null;

    private void OnEnable()
    {
        rseOnPlayerPause.action += Close;

        if (defaultPanelSet != null)
        {
            defaultPanelSet.SetActive(true);
            currentPanelSet = defaultPanelSet;

            Navigation nav = buttonGameplay.navigation;
            nav.mode = Navigation.Mode.Explicit;

            nav.selectOnDown = dropDownLanguages;

            buttonGameplay.navigation = nav;

            Navigation nav2 = buttonReturn.navigation;
            nav2.mode = Navigation.Mode.Explicit;

            nav2.selectOnUp = dropDownLanguages;
            nav2.selectOnDown = buttonGameplay;

            buttonReturn.navigation = nav2;
        }
    }

    private void OnDisable()
    {
        rseOnPlayerPause.action -= Close;

        if (currentPanelSet != null)
        {
            currentPanelSet.SetActive(false);
            currentPanelSet = null;
        }
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

    private void ClosePanel()
    {
        if (currentPanelSet != null)
        {
            currentPanelSet.SetActive(false);

            Navigation nav = buttonGameplay.navigation;
            nav.mode = Navigation.Mode.Explicit;

            nav.selectOnDown = buttonReturn;

            buttonGameplay.navigation = nav;

            Navigation nav2 = buttonReturn.navigation;
            nav2.mode = Navigation.Mode.Explicit;

            nav2.selectOnUp = buttonGameplay;
            nav2.selectOnDown = buttonGameplay;

            buttonReturn.navigation = nav2;

            Navigation nav3 = buttonGraphics.navigation;
            nav3.mode = Navigation.Mode.Explicit;

            nav3.selectOnDown = buttonReturn;

            buttonGraphics.navigation = nav3;

            Navigation nav4 = buttonReturn.navigation;
            nav4.mode = Navigation.Mode.Explicit;

            nav4.selectOnUp = buttonGraphics;
            nav4.selectOnDown = buttonGraphics;

            buttonReturn.navigation = nav4;

            Navigation nav5 = buttonAudio.navigation;
            nav5.mode = Navigation.Mode.Explicit;

            nav5.selectOnDown = buttonReturn;

            buttonAudio.navigation = nav5;

            Navigation nav6 = buttonReturn.navigation;
            nav6.mode = Navigation.Mode.Explicit;

            nav6.selectOnUp = buttonAudio;
            nav6.selectOnDown = buttonAudio;

            buttonReturn.navigation = nav6;
        }
    }

    public void OpenPanelGameplay(GameObject panelSet)
    {
        if (currentPanelSet != panelSet)
        {
            ClosePanel();

            panelSet.SetActive(true);
            currentPanelSet = panelSet;

            Navigation nav = buttonGameplay.navigation;
            nav.mode = Navigation.Mode.Explicit;

            nav.selectOnDown = dropDownLanguages;

            buttonGameplay.navigation = nav;

            Navigation nav2 = buttonReturn.navigation;
            nav2.mode = Navigation.Mode.Explicit;

            nav2.selectOnUp = dropDownLanguages;
            nav2.selectOnDown = buttonGameplay;

            buttonReturn.navigation = nav2;
        }
    }

    public void OpenPanelGraphics(GameObject panelSet)
    {
        if (currentPanelSet != panelSet)
        {
            ClosePanel();

            panelSet.SetActive(true);
            currentPanelSet = panelSet;

            Navigation nav = buttonGraphics.navigation;
            nav.mode = Navigation.Mode.Explicit;

            nav.selectOnDown = dropDownResolutions;

            buttonGraphics.navigation = nav;

            Navigation nav2 = buttonReturn.navigation;
            nav2.mode = Navigation.Mode.Explicit;

            nav2.selectOnUp = toggleFullscreen;
            nav2.selectOnDown = buttonGraphics;

            buttonReturn.navigation = nav2;
        }
    }

    public void OpenPanelAudio(GameObject panelSet)
    {
        if (currentPanelSet != panelSet)
        {
            ClosePanel();

            panelSet.SetActive(true);
            currentPanelSet = panelSet;

            Navigation nav = buttonAudio.navigation;
            nav.mode = Navigation.Mode.Explicit;

            nav.selectOnDown = sliderMainVolume;

            buttonAudio.navigation = nav;

            Navigation nav2 = buttonReturn.navigation;
            nav2.mode = Navigation.Mode.Explicit;

            nav2.selectOnUp = sliderUIVolume;
            nav2.selectOnDown = buttonAudio;

            buttonReturn.navigation = nav2;
        }
    }
}