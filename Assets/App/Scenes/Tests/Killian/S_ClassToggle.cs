using System;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/Custom Toggle")]
    [RequireComponent(typeof(RectTransform))]
    public class S_ClassToggle : Selectable, IPointerClickHandler, ISubmitHandler, ISelectHandler
    {
        public enum ToggleTransition
        {
            None,
            Fade,
        }

        [Header("Settings")]
        [SerializeField] private bool m_IsOn;

        [Header("Visual")]
        [SerializeField] public ToggleTransition toggleTransition = ToggleTransition.Fade;
        [SerializeField] public Graphic graphic;

        [Header("Toggle Group")]
        [SerializeField] private S_ClassToggleGroup m_Group;

        [Header("Main Events")]
        public ToggleEvent onValueChanged = new();

        [Header("Other Events")]
        public GeneralEvent onPointerClick = new();
        public GeneralEvent onPointerEnter = new();
        public GeneralEvent onPointerDown = new();
        public GeneralEvent onPointerUp = new();
        public GeneralEvent onPointerExit = new();
        public GeneralEvent onSubmit = new();
        public GeneralEvent onSelect = new();
        public GeneralEvent onDeselect = new();

        [Serializable] public class ToggleEvent : UnityEvent<bool> { }

        [Serializable] public class GeneralEvent : UnityEvent<BaseEventData> { }

        public S_ClassToggleGroup group
        {
            get => m_Group;
            set
            {
                SetToggleGroup(value, true);
                PlayEffect(true);
            }
        }

        public bool isOn
        {
            get => m_IsOn;
            set => Set(value);
        }

        protected S_ClassToggle() { }

        #if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            SetIsOnWithoutNotify(m_IsOn);
            PlayEffect(true);
        }
        #endif

        public virtual void LayoutComplete() { }

        public virtual void GraphicUpdateComplete() { }

        protected override void OnDestroy()
        {
            m_Group?.EnsureValidState();
            base.OnDestroy();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetToggleGroup(m_Group, false);
            PlayEffect(true);
        }

        protected override void OnDisable()
        {
            SetToggleGroup(null, false);
            base.OnDisable();
        }

        protected override void OnDidApplyAnimationProperties()
        {
            if (graphic != null)
            {
                bool oldValue = !Mathf.Approximately(graphic.canvasRenderer.GetColor().a, 0);
                if (m_IsOn != oldValue)
                {
                    m_IsOn = oldValue;
                    Set(!oldValue);
                }
            }

            base.OnDidApplyAnimationProperties();
        }

        private void SetToggleGroup(S_ClassToggleGroup newGroup, bool setMemberValue)
        {
            if (m_Group == newGroup) return;

            m_Group?.UnregisterToggle(this);

            if (setMemberValue) m_Group = newGroup;

            if (newGroup != null && IsActive())
            {
                newGroup.RegisterToggle(this);
                if (isOn) newGroup.NotifyToggleOn(this);
            }
        }

        public void SetIsOnWithoutNotify(bool value)
        {
            Set(value, false);
        }

        private void Set(bool value, bool sendCallback = true)
        {
            if (m_IsOn == value) return;

            m_IsOn = value;

            if (m_Group != null && m_Group.isActiveAndEnabled && IsActive())
            {
                if (m_IsOn || (!m_Group.AnyTogglesOn() && !m_Group.allowSwitchOff))
                {
                    m_IsOn = true;
                    m_Group.NotifyToggleOn(this, sendCallback);
                }
            }

            PlayEffect(toggleTransition == ToggleTransition.None);

            if (sendCallback)
            {
                UISystemProfilerApi.AddMarker("Toggle.value", this);
                onValueChanged?.Invoke(m_IsOn);
            }
        }

        public void PlayEffect(bool instant)
        {
            if (graphic == null) return;

            if (instant) graphic.canvasRenderer.SetAlpha(m_IsOn ? 1f : 0f);
            else graphic.CrossFadeAlpha(m_IsOn ? 1f : 0f, 0.1f, true);
        }

        protected override void Start()
        {
            PlayEffect(true);
        }

        private void InternalToggle()
        {
            if (!IsActive() || !IsInteractable()) return;

            isOn = !isOn;
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;

            InternalToggle();

            onPointerClick?.Invoke(eventData);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);

            onPointerEnter?.Invoke(eventData);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            onPointerDown?.Invoke(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            onPointerUp?.Invoke(eventData);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);

            onPointerExit?.Invoke(eventData);
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            InternalToggle();

            onSubmit?.Invoke(eventData);
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);

            onSelect?.Invoke(eventData);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);

            onDeselect?.Invoke(eventData);
        }
    }
}