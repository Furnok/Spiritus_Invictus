using Sirenix.OdinInspector;
using System;
using UnityEngine;

[Serializable]
public class S_ClassCameraData
{
    [Title("Camera")]
    public float cameraDistanceMinPlayer = 0;
    [SuffixLabel("s", Overlay = true)]
    public float rotationCameraPlayerDuration = 0;
    [SuffixLabel("s", Overlay = true)]
    public float rotationCameraPlayerDodgeDuration = 0;
    [SuffixLabel("°", Overlay = true)]
    public float minVerticalCameraPlayer = 0;
    [SuffixLabel("°", Overlay = true)]
    public float maxVerticalCameraPlayer = 0;

    [Title("Switch Shoulder Offset")]
    public Vector3 targetShoulderOffsetPositive = Vector3.zero;
    public Vector3 targetShoulderOffsetNegative = Vector3.zero;
    [SuffixLabel("s", Overlay = true)]
    public float switchDurationCamera = 0;

    [Title("Cinematic")]
    [SuffixLabel("s", Overlay = true)]
    public float holdSkipTime = 0;
    [SuffixLabel("s", Overlay = true)]
    public float StartDisplaySkipTime = 0;

    [Title("Player")]
    public float fadeSpeedPlayer = 0;
}