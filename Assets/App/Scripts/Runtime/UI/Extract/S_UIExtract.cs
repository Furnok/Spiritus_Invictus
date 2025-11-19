using DG.Tweening;
using FMODUnity;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class S_UIExtract : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Scroll")]
    [SuffixLabel("s", Overlay = true)]
    [SerializeField] private float scrollStart;

    [TabGroup("Settings")]
    [Title("Display")]
    [SuffixLabel("s", Overlay = true)]
    [SerializeField] private float startDisplay;

    [TabGroup("References")]
    [Title("Audio")]
    [SerializeField] private EventReference uiSound;

    [TabGroup("References")]
    [Title("Text")]
    [SerializeField] private TextMeshProUGUI textContent;

    [TabGroup("References")]
    [Title("Scroll View")]
    [SerializeField] private ScrollRect scrollRect;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnDisplayExtract rseOnDisplayExtract;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerPause rseOnPlayerPause;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnCloseWindow rseOnCloseWindow;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnGameInputEnabled rseOnGameInputEnabled;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnResetFocus rseOnResetFocus;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnShowMouseCursor rseOnShowMouseCursor;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnHideMouseCursor rseOnHideMouseCursor;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_Navigation rsoNavigation;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_CurrentWindows rsoCurrentWindows;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_InConsole rsoInConsole;

    private bool displayText = false;
    private bool userIsScrolling = false;
    private Tween textDisplay = null;
    private Tween scrollTween = null;
    private bool isClosing = false;
    private bool isInitialise = false;
    private int lastSoundFrame = -1;

    private void OnEnable()
    {
        rseOnDisplayExtract.action += DisplayTextContent;
        rseOnPlayerPause.action += CloseEscape;

        if (Gamepad.current == null)
        {
            rseOnShowMouseCursor.Call();
        }

        scrollRect.verticalNormalizedPosition = 1;
        StartCoroutine(S_Utils.Delay(0.1f, () => isInitialise = true));
    }

    private void OnDisable()
    {
        rseOnDisplayExtract.action -= DisplayTextContent;
        rseOnPlayerPause.action -= CloseEscape;

        displayText = false;
        textContent.text = "";
        userIsScrolling = false;
        textDisplay?.Kill();
        scrollTween?.Kill();
        textDisplay = null;
        scrollTween = null;
        isClosing = false;
        isInitialise = false;
    }

    public void OnScrollChanged()
    {
        if (displayText && !userIsScrolling)
        {
            userIsScrolling = true;
            scrollTween?.Kill();
            scrollTween = null;
        }
    }

    public void MoveScroll(BaseEventData eventData)
    {
        if (displayText && !userIsScrolling)
        {
            AxisEventData axisData = eventData as AxisEventData;
            MoveDirection direction = axisData.moveDir;

            switch (direction)
            {
                case MoveDirection.Up:
                    RuntimeManager.PlayOneShot(uiSound);
                    OnScrollChanged();
                    break;
                case MoveDirection.Down:
                    RuntimeManager.PlayOneShot(uiSound);
                    OnScrollChanged();
                    break;
                default:
                    break;
            }
        }
    }

    private void CloseEscape()
    {
        if (!isClosing)
        {
            if (rsoCurrentWindows.Value[^1] == gameObject && !rsoInConsole.Value)
            {
                RuntimeManager.PlayOneShot(uiSound);

                Close();
            }
        }
    }

    public void Close()
    {
        if (!isClosing)
        {
            rseOnHideMouseCursor.Call();

            isClosing = true;
            rseOnGameInputEnabled.Call();
            rseOnCloseWindow.Call(gameObject);
            rsoNavigation.Value.selectableFocus = null;
            rseOnResetFocus.Call();
        }
    }

    public void SliderAudio(Selectable uiElement)
    {
        if (uiElement.interactable && Gamepad.current != null && isInitialise && userIsScrolling)
        {

            if (lastSoundFrame == Time.frameCount)
                return;

            lastSoundFrame = Time.frameCount;

            RuntimeManager.PlayOneShot(uiSound);
        }
    }

    private void DisplayTextContent(S_ClassExtract classExtract)
    {
        string fullText = classExtract.text.GetLocalizedString();

        textContent.maxVisibleCharacters = 0;
        textContent.text = fullText;

        StartCoroutine(S_Utils.Delay(startDisplay, () =>
        {
            displayText = true;

            textDisplay = DOTween.To(() => 0, x =>
            {
                int length = Mathf.Clamp(x, 0, fullText.Length);
                textContent.maxVisibleCharacters = length;

                float progress = (float)length / fullText.Length;

                if (!userIsScrolling && progress >= scrollStart)
                {
                    float adjustedProgress = Mathf.InverseLerp(scrollStart, 1f, progress);
                    float targetPos = Mathf.Lerp(1f, 0f, adjustedProgress);

                    scrollTween = scrollRect.DOVerticalNormalizedPos(targetPos, 0.1f).SetEase(Ease.Linear);
                }
            }, fullText.Length, classExtract.duration).SetEase(Ease.Linear);
        }));
    }
}