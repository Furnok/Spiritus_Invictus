using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/Custom Toggle Group")]
    [DisallowMultipleComponent]
    public class S_ClassToggleGroup : UIBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool m_AllowSwitchOff = false;

        public bool allowSwitchOff
        {
            get => m_AllowSwitchOff;
            set => m_AllowSwitchOff = value;
        }

        protected List<S_ClassToggle> m_Toggles = new();

        protected S_ClassToggleGroup() { }

        protected override void Start()
        {
            base.Start();
            EnsureValidState();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            EnsureValidState();
        }

        private void ValidateToggleIsInGroup(S_ClassToggle toggle)
        {
            if (toggle == null || !m_Toggles.Contains(toggle))
                throw new ArgumentException(string.Format("Toggle {0} is not part of ToggleGroup {1}", new object[] { toggle, this }));
        }

        public void NotifyToggleOn(S_ClassToggle toggle, bool sendCallback = true)
        {
            ValidateToggleIsInGroup(toggle);

            foreach (var t in m_Toggles)
            {
                if (t == toggle) continue;
                if (sendCallback) t.isOn = false;
                else t.SetIsOnWithoutNotify(false);
            }
        }

        public void RegisterToggle(S_ClassToggle toggle)
        {
            if (toggle != null && !m_Toggles.Contains(toggle)) m_Toggles.Add(toggle);
        }

        public void UnregisterToggle(S_ClassToggle toggle)
        {
            if (toggle != null) m_Toggles.Remove(toggle);
        }

        public void EnsureValidState()
        {
            if (!allowSwitchOff && !m_Toggles.Any(t => t.isOn) && m_Toggles.Count > 0)
            {
                m_Toggles[0].isOn = true;
                NotifyToggleOn(m_Toggles[0]);
            }

            var activeToggles = m_Toggles.Where(t => t.isOn).ToList();
            if (activeToggles.Count <= 1) return;

            var firstActive = activeToggles.First();
            foreach (var toggle in activeToggles.Skip(1))
                toggle.isOn = false;
        }

        public bool AnyTogglesOn()
        {
            return m_Toggles.Any(t => t.isOn);
        }

        public IEnumerable<S_ClassToggle> ActiveToggles()
        {
            return m_Toggles.Where(t => t.isOn);
        }

        public S_ClassToggle GetFirstActiveToggle()
        {
            return m_Toggles.FirstOrDefault(t => t.isOn);
        }

        public void SetAllTogglesOff(bool sendCallback = true)
        {
            bool oldAllowSwitchOff = m_AllowSwitchOff;
            m_AllowSwitchOff = true;

            foreach (var toggle in m_Toggles)
            {
                if (sendCallback) toggle.isOn = false;
                else toggle.SetIsOnWithoutNotify(false);
            }

            m_AllowSwitchOff = oldAllowSwitchOff;
        }
    }
}