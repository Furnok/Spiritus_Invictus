using Sirenix.OdinInspector;
using System;
using UnityEngine;

[Serializable]
public class S_ClassCameraData
{
    [Title("Camera")]
    public float cameraDistanceMinPlayer;
    [SuffixLabel("s", Overlay = true)]
    public float rotationCameraPlayerDuration;
    [SuffixLabel("s", Overlay = true)]
    public float rotationCameraPlayerDodgeDuration;

    [Title("Switch Shoulder Offset")]
    public Vector3 targetShoulderOffsetPositive;
    public Vector3 targetShoulderOffsetNegative;
    [SuffixLabel("s", Overlay = true)]
    public float switchDurationCamera;

    [Title("Cinematic")]
    [SuffixLabel("s", Overlay = true)]
    public float holdSkipTime;
    [SuffixLabel("s", Overlay = true)]
    public float StartDisplaySkipTime;

    [Title("Player")]
    public float fadeSpeedPlayer;
}