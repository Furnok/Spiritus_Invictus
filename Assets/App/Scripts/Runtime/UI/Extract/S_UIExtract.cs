using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class S_UIExtract : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Scroll")]
    [SerializeField] private float scrollStart;

    [TabGroup("Settings")]
    [Title("Display")]
    [SerializeField] private float startDisplay;

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
    [SerializeField] private RSO_Navigation rsoNavigation;

    private bool displayText = false;
    private bool userIsScrolling = false;
    private Tween textDisplay = null;
    private Tween scrollTween = null;

    private void OnEnable()
    {
        rseOnDisplayExtract.action += DisplayTextContent;
        rseOnPlayerPause.action += Close;

        scrollRect.verticalNormalizedPosition = 1;
    }

    private void OnDisable()
    {
        rseOnDisplayExtract.action -= DisplayTextContent;
        rseOnPlayerPause.action -= Close;

        displayText = false;
        textContent.text = "";
        userIsScrolling = false;
        textDisplay?.Kill();
        scrollTween?.Kill();
        textDisplay = null;
        scrollTween = null;
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
                    OnScrollChanged();
                    break;
                case MoveDirection.Down:
                    OnScrollChanged();
                    break;
                default:
                    break;
            }
        }
    }

    public void Close()
    {
        rseOnGameInputEnabled.Call();
        rseOnCloseWindow.Call(gameObject);
        rsoNavigation.Value.selectableFocus = null;
        rseOnResetFocus.Call();
    }

    private void DisplayTextContent(S_ClassExtract classExtract)
    {
        string fullText = classExtract.text;

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