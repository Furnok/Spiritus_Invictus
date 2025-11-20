using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class S_UIConsole : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Audio")]
    [SerializeField] private EventReference uiSound;

    [TabGroup("References")]
    [Title("Scroll View")]
    [SerializeField] private ScrollRect scrollRect;

    [TabGroup("References")]
    [Title("Slider")]
    [SerializeField] private Scrollbar sliderScroll;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnShowMouseCursor rseOnShowMouseCursor;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_ConsoleDisplay rsoConsoleDisplay;

    private int lastSoundFrame = -1;
    private float lastValue = 0;

    private void OnEnable()
    {
        rsoConsoleDisplay.Value = true;

        if (Gamepad.current == null)
        {
            rseOnShowMouseCursor.Call();
        }

        scrollRect.verticalNormalizedPosition = 0;
    }

    public void SliderAudio(BaseEventData eventData)
    {
        if (Gamepad.current != null)
        {
            if (lastSoundFrame == Time.frameCount)
                return;

            AxisEventData axisData = eventData as AxisEventData;
            MoveDirection direction = axisData.moveDir;
            float roundedValue = 0;

            switch (direction)
            {
                case MoveDirection.Up:
                    lastSoundFrame = Time.frameCount;

                    roundedValue = Mathf.Round(sliderScroll.value * 1000f) / 1000f;
                    if (roundedValue != lastValue && sliderScroll.size < 1)
                    {
                        lastValue = roundedValue;
                        RuntimeManager.PlayOneShot(uiSound);
                    }
                    break;
                case MoveDirection.Down:
                    lastSoundFrame = Time.frameCount;

                    roundedValue = Mathf.Round(sliderScroll.value * 1000f) / 1000f;
                    if (roundedValue != lastValue && sliderScroll.size < 1)
                    {
                        lastValue = roundedValue;
                        RuntimeManager.PlayOneShot(uiSound);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}