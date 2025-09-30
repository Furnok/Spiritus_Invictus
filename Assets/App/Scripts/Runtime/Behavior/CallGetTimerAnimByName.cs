using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/CallGetTimerAnimByName")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "CallGetTimerAnimByName", message: "Get timer animatio by [Name]", category: "Events", id: "6ed3f6dc35d517aedd38436137c1ca59")]
public sealed partial class CallGetTimerAnimByName : EventChannel<string> { }

