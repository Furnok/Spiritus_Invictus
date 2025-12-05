using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class S_UISliderHorizontal : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Audio")]
    [SerializeField] private EventReference uiClick;

    [TabGroup("References")]
    [Title("Slider")]
    [SerializeField] private Slider slider;

    private int lastSoundFrame = -1;
    private float lastValue = -1;
    private bool startMove = false;
    private bool isStick = false;

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == slider.gameObject && Gamepad.current != null && startMove)
        {
            Vector2 dpad = Gamepad.current.dpad.ReadValue();
            Vector2 leftStick = Gamepad.current.leftStick.ReadValue();
            Vector2 rightStick = Gamepad.current.rightStick.ReadValue();

            bool stickActive = Mathf.Abs(leftStick.x) > 0.5f;
            bool stick2Active = Mathf.Abs(rightStick.x) > 0.5f;

            if (stickActive || stick2Active)
            {
                Sticks();
            }
            else if (isStick)
            {
                startMove = false;
                RuntimeManager.PlayOneShot(uiClick);
            }
            else
            {
                bool dpadLeft = dpad.x < 0;
                bool dpadRight = dpad.x > 0;
                DPad(dpadLeft, dpadRight);
            }
        }
        else
        {
            isStick = false;
        }
    }

    private void DPad(bool left, bool right)
    {
        if (!left && !right && startMove)
        {
            startMove = false;
        }
    }

    private void Sticks()
    {
        isStick = true;
    }

    public void SliderAudio(BaseEventData eventData)
    {
        if (Gamepad.current != null)
        {
            if (lastSoundFrame == Time.frameCount)
                return;

            AxisEventData axisData = eventData as AxisEventData;
            MoveDirection direction = axisData.moveDir;
            float value = 0;

            switch (direction)
            {
                case MoveDirection.Left:
                    lastSoundFrame = Time.frameCount;

                    value = slider.value;
                    if (value != lastValue)
                    {
                        lastValue = value;

                        if (!startMove)
                        {
                            startMove = true;
                            RuntimeManager.PlayOneShot(uiClick);
                        }
                    }
                    break;
                case MoveDirection.Right:
                    lastSoundFrame = Time.frameCount;

                    value = slider.value;
                    if (value != lastValue)
                    {
                        lastValue = value;

                        if (!startMove)
                        {
                            startMove = true;
                            RuntimeManager.PlayOneShot(uiClick);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}