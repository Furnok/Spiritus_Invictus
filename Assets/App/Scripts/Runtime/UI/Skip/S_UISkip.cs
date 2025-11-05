using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_UISkip : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Time")]
    [SuffixLabel("s", Overlay = true)]
    [SerializeField] private float timeFade;

    [TabGroup("References")]
    [Title("Canvas Group")]
    [SerializeField] private CanvasGroup canvasGroup;

    [TabGroup("References")]
    [Title("Images")]
    [SerializeField] private Image image;

    [TabGroup("References")]
    [Title("Text")]
    [SerializeField] private TextMeshProUGUI text;

    [TabGroup("References")]
    [SerializeField] private TextMeshProUGUI text2;

    [TabGroup("References")]
    [Title("Keyboard & Mouse")]
    [SerializeField] private Sprite imageKeyboardMouse;

    [TabGroup("References")]
    [Title("PlayStation")]
    [SerializeField] private Sprite imagePlayStation;

    [TabGroup("References")]
    [Title("Xbox")]
    [SerializeField] private Sprite imageXbox;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_Device rsoDevice;

    private void OnEnable()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, timeFade).SetEase(Ease.Linear);
    }

    private void LateUpdate()
    {
        if (rsoDevice.Value == S_EnumDevice.KeyboardMouse)
        {
            image.sprite = imageKeyboardMouse;
            text.text = "ESC";
            text2.text = "";
        }
        else if (rsoDevice.Value == S_EnumDevice.PlastationController)
        {
            image.sprite = imagePlayStation;
            text.text = "";
            text2.text = "Options";
        }
        else if (rsoDevice.Value == S_EnumDevice.XboxController)
        {
            image.sprite = imageXbox;
            text.text = "";
            text2.text = "";
        }
    }
}