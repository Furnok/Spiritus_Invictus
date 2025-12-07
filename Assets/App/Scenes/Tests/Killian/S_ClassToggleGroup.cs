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
        [SerializeField] private bool m_AllowSwitchOff = false;

        public bool allowSwitchOff { get { return m_AllowSwitchOff; } set { m_AllowSwitchOff = value; } }

        protected List<S_ClassToggle> m_Toggles = new();

        protected S_ClassToggleGroup() { }

        protected override void Start()
        {
            EnsureValidState();
            base.Start();
        }

        protected override void OnEnable()
        {
            EnsureValidState();
            base.OnEnable();
        }

        private void ValidateToggleIsInGroup(S_ClassToggle toggle)
        {
            if (toggle == null || !m_Toggles.Contains(toggle))
                throw new ArgumentException(string.Format("Toggle {0} is not part of ToggleGroup {1}", new object[] { toggle, this }));
        }

        public void NotifyToggleOn(S_ClassToggle toggle, bool sendCallback = true)
        {
            ValidateToggleIsInGroup(toggle);

            for (var i = 0; i < m_Toggles.Count; i++)
            {
                if (m_Toggles[i] == toggle)
                    continue;

                if (sendCallback)
                    m_Toggles[i].isOn = false;
                else
                    m_Toggles[i].SetIsOnWithoutNotify(false);
            }
        }

        public void UnregisterToggle(S_ClassToggle toggle)
        {
            if (m_Toggles.Contains(toggle))
                m_Toggles.Remove(toggle);
        }

        public void RegisterToggle(S_ClassToggle toggle)
        {
            if (!m_Toggles.Contains(toggle))
                m_Toggles.Add(toggle);
        }

        public void EnsureValidState()
        {
            if (!allowSwitchOff && !AnyTogglesOn() && m_Toggles.Count != 0)
            {
                m_Toggles[0].isOn = true;
                NotifyToggleOn(m_Toggles[0]);
            }

            IEnumerable<S_ClassToggle> activeToggles = ActiveToggles();

            if (activeToggles.Count() > 1)
            {
                S_ClassToggle firstActive = GetFirstActiveToggle();

                foreach (S_ClassToggle toggle in activeToggles)
                {
                    if (toggle == firstActive)
                    {
                        continue;
                    }
                    toggle.isOn = false;
                }
            }
        }

        public bool AnyTogglesOn()
        {
            return m_Toggles.Find(x => x.isOn) != null;
        }

        public IEnumerable<S_ClassToggle> ActiveToggles()
        {
            return m_Toggles.Where(x => x.isOn);
        }

        public S_ClassToggle GetFirstActiveToggle()
        {
            IEnumerable<S_ClassToggle> activeToggles = ActiveToggles();
            return activeToggles.Count() > 0 ? activeToggles.First() : null;
        }

        public void SetAllTogglesOff(bool sendCallback = true)
        {
            bool oldAllowSwitchOff = m_AllowSwitchOff;
            m_AllowSwitchOff = true;

            if (sendCallback)
            {
                for (var i = 0; i < m_Toggles.Count; i++)
                    m_Toggles[i].isOn = false;
            }
            else
            {
                for (var i = 0; i < m_Toggles.Count; i++)
                    m_Toggles[i].SetIsOnWithoutNotify(false);
            }

            m_AllowSwitchOff = oldAllowSwitchOff;
        }
    }
}