using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class S_UISelectableScroll : MonoBehaviour, IScrollHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Settings")]
    [SerializeField] private bool isScrollBar;

    [Header("References")]
    [SerializeField] private ScrollRect parentScrollRect;

    public void Setup(ScrollRect scrollRect)
    {
        parentScrollRect = scrollRect;
    }

    public void OnScroll(PointerEventData eventData)
    {
        if (parentScrollRect != null)
        {
            parentScrollRect.OnScroll(eventData);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isScrollBar)
        {
            parentScrollRect.OnBeginDrag(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isScrollBar)
        {
            parentScrollRect.OnDrag(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isScrollBar)
        {
            parentScrollRect.OnEndDrag(eventData);
        }
    }
}