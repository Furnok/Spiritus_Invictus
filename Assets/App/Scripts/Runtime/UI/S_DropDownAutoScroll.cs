using DG.Tweening;
using System;
using TMPro;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class S_DropDownAutoScroll : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float transition;

    [Header("References")]
    [SerializeField] private TMP_Dropdown dropDown;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Transform content;

    [Header("Output")]
    [SerializeField] private RSO_SettingsSaved rsoSettingsSaved;

    private bool init = false;
    private int number = 0;
    private Tween moveTween = null;
    private S_SerializableDictionary<Selectable, int> selectables = new();

    private void OnEnable()
    {
        number = dropDown.options.Count - 1;

        StartCoroutine(S_Utils.Delay(0.5f, () => Setup()));
    }

    private void Setup()
    {
        selectables.Clear();

        for (int i = 0; i <= number; i++)
        {
            Transform item = content.GetChild(i + 1);
            if (item.TryGetComponent(out Selectable selectable))
            {
                selectables[selectable] = i;
            }
        }
    }

    private void OnDisable()
    {
        init = false;

        moveTween?.Kill();
    }

    private void Update()
    {
        if (dropDown.IsExpanded && !init)
        {
            ScrollOpen();
        }
    }

    private void ScrollOpen()
    {
        init = true;

        float targetPos = 1f - ((float)rsoSettingsSaved.Value.resolutionIndex / number);
        moveTween = scrollRect.DOVerticalNormalizedPos(targetPos, 0).SetEase(Ease.Linear);
    }

    public void ScrollToIndex(Selectable item)
    {
        if (selectables.TryGetValue(item, out int index) && Gamepad.current != null)
        {
            float targetPos = 1f - ((float)index / number);
            moveTween = scrollRect.DOVerticalNormalizedPos(targetPos, transition).SetEase(Ease.Linear);
        }
    }
}