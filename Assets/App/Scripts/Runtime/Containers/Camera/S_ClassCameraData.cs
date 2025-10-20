using Sirenix.OdinInspector;
using System;
using Unity.Cinemachine;
using UnityEngine;

[Serializable]
public class S_ClassCameraData
{
    [Title("Camera")]
    public float cameraDistanceMinPlayer;
    public float rotationCameraPlayerDuration;

    [Title("Switch Shoulder Offset")]
    public Vector3 targetShoulderOffsetPositive;
    public Vector3 targetShoulderOffsetNegative;
    public float switchTimeCamera;

    [Title("Player")]
    public float fadeSpeedPlayer;
}