using Sirenix.OdinInspector;
using System;
using UnityEngine;

[Serializable]
public class S_ClassTargetsDebug
{
    [Title("General")]
    public Color gizmoColor = Color.white;

    public Color gizmoTargetColor = Color.white;

    public Color gizmoPreTargetColor = Color.white;

    public LayerMask obstacleMask = 0;

    public float gizmoRadius = 0;

    public float gizmoTargetRadius = 0;

    public float gizmoPreTargetRadius = 0;

    public float gizmoHeightOffset = 0;
}