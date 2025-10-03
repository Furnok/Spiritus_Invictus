using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_UIExtract : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float scrollStart;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI textContent;
    [SerializeField] private ScrollRect scrollRect;

    [Header("Input")]
    [SerializeField] private RSE_OnDisplayExtract rseOnDisplayExtract;

    [Header("Output")]
    [SerializeField] private RSE_OnGameInputEnabled rseOnGameInputEnabled;

    private bool displayText = false;
    private bool userIsScrolling = false;
    private float userScrollThreshold = 0.05f;
    private Tweener textDisplay = null;
    private float lastScrollPos = 0;
    private bool hasLastScrollPos = false;

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        rseOnDisplayExtract.action += DisplayTextContent;
    }

    private void OnDisable()
    {
        rseOnDisplayExtract.action -= DisplayTextContent;

        rseOnGameInputEnabled.Call();

        textContent.text = "";
        displayText = false;
        textDisplay.Kill();
    }

    private void Update()
    {
        if (displayText)
        {
            float scrollPos = scrollRect.verticalNormalizedPosition;

            if (!hasLastScrollPos)
            {
                lastScrollPos = scrollPos;
                hasLastScrollPos = true;
                userIsScrolling = false;
                return;
            }

            if (Mathf.Abs(scrollPos - lastScrollPos) > userScrollThreshold)
            {
                userIsScrolling = true;
            }
            else
            {
                userIsScrolling = false;
            }

            lastScrollPos = scrollPos;
        }
    }

    private void DisplayTextContent(S_ClassExtract classExtract)
    {
        displayText = true;

        string fullText = classExtract.text;

        textContent.maxVisibleCharacters = 0;
        textContent.text = fullText;

        float initialScrollPos = scrollRect.verticalNormalizedPosition;
        textDisplay = DOTween.To(() => 0, x => {
            int length = Mathf.Clamp(x, 0, fullText.Length);
            textContent.maxVisibleCharacters = length;
            float progress = (float)length / fullText.Length;
            if (!userIsScrolling && progress >= scrollStart)
            {
                float adjustedProgress = Mathf.InverseLerp(scrollStart, 1f, progress);
                float targetPos = Mathf.Lerp(1f, 0f, adjustedProgress);
                scrollRect.verticalNormalizedPosition = Mathf.Lerp(scrollRect.verticalNormalizedPosition, targetPos, 0.1f);
            }
        }, fullText.Length, classExtract.duration).SetEase(Ease.Linear);
    }
}