using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(fileName = "RSE_CallGetTimerAnimByName", menuName = "Data/RSE/Enemy/CallGetTimerAnimByName")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "CallGetTimerAnimByName", message: "Get timer animatio by [Name]", category: "Events", id: "6ed3f6dc35d517aedd38436137c1ca59")]
public sealed partial class RSE_CallGetTimerAnimByName : EventChannel<string> { }

