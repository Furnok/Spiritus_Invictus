using Sirenix.OdinInspector;
using System;
using UnityEditor;
using UnityEngine;

namespace BT.ScriptablesObject
{
    public class RuntimeScriptableObject<T> : ScriptableObject
    {
        public event Action<T> onValueChanged;

        [SerializeField] private T _value = default;

        [HideInInspector, NonSerialized] private T _initialValue = default;

        public T Value
        {
            get => _value;
            set
            {
                _value = value;

                onValueChanged?.Invoke(_value);
            }
        }

        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying)
                _initialValue = _value;
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                ResetValue();
            }
        }

        private void ResetValue()
        {
            onValueChanged = null;
            _value = _initialValue;
        }

        [Button("Call Event")]
        private void InvokeOnValueChanged()
        {
            onValueChanged?.Invoke(_value);
        }
        #endif
    }
}
