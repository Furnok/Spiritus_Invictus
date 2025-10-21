using Sirenix.OdinInspector;
using System;
using UnityEngine;

[Serializable]
public class S_ClassCameraData
{
    [Title("Camera")]
    public float cameraDistanceMinPlayer;
    public float rotationCameraPlayerDuration;
    public float rotationCameraPlayerDodgeDuration;

    [Title("Switch Shoulder Offset")]
    public Vector3 targetShoulderOffsetPositive;
    public Vector3 targetShoulderOffsetNegative;
    public float switchTimeCamera;

    [Title("Player")]
    public float fadeSpeedPlayer;
}