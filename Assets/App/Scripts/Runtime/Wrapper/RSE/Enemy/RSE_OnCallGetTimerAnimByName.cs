using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(fileName = "RSE_OnCallGetTimerAnimByName", menuName = "Data/RSE/Enemy/OnCallGetTimerAnimByName")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "OnCallGetTimerAnimByName", message: "Get timer animation by [Name]", category: "Events", id: "6ed3f6dc35d517aedd38436137c1ca59")]
public sealed partial class RSE_OnCallGetTimerAnimByName : EventChannel<string> { }

