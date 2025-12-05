using UnityEngine.EventSystems;
using UnityEngine.UI;

public class S_CustomSlider : Slider
{
    public override void OnMove(AxisEventData eventData)
    {
        eventData.Use();
    }
}