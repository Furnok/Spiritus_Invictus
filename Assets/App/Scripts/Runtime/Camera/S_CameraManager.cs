using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Splines;

public class S_CameraManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int cameraFocusPriority;
    [SerializeField] private int cameraUnFocusPriority;
    [SerializeField] private int cameraCinematicPriority;
    [SerializeField] private float cameraDistanceMin;
    [SerializeField] private float fadeSpeed;
    [SerializeField] private float rotationDuration;

    //[SerializeField] private float[] fovPerKnot;
    //[SerializeField] private float transitionSpeed;

    [Header("References")]
    [SerializeField] private Camera cameraMain;
    [SerializeField] private Material materialPlayer;
    [SerializeField] private CinemachineCamera cinemachineCameraRail;
    [SerializeField] private CinemachineCamera cinemachineCameraPlayer;
    [SerializeField] private List<CinemachineCamera> cinemachineCameraCinematic;
    [SerializeField] private CinemachineTargetGroup targetGroup;
    [SerializeField] private GameObject playerPoint;

    //[SerializeField] private CinemachineSplineDolly splineDolly;

    [Header("Input")]
    [SerializeField] private RSE_CameraCinematic rseCameraCinematic;
    [SerializeField] private RSE_CinematicFinish rseCinematicFinish;
    [SerializeField] private RSO_PlayerIsTargeting rsoplayerIsTargeting;
    [SerializeField] private RSE_CameraShake rseCameraShake;
    [SerializeField] private RSE_OnPlayerCameraLook rseOnPlayerCameraLook;
    
    private Coroutine shake = null;

    private CinemachineCamera currentCamera = null;
    private CinemachineCamera oldCamera = null;

    private Transform playerPos = null;

    private float currentAlpha = 1f;
    private Quaternion startRotation;
    private Quaternion targetRotation;
    private float rotationTimer = 0f;
    private bool isRotating = false;

    private void OnEnable()
    {
        rseCameraCinematic.action += SwitchCinematicCamera;
        rsoplayerIsTargeting.onValueChanged += SwitchCameraTargeting;
        rseCameraShake.action += CameraShake;
        rseCinematicFinish.action += FinishCinematic;
        rseOnPlayerCameraLook.action += PlayerPos;

        currentCamera = cinemachineCameraRail;
    }

    private void OnDisable()
    {
        rseCameraCinematic.action -= SwitchCinematicCamera;
        rsoplayerIsTargeting.onValueChanged -= SwitchCameraTargeting;
        rseCameraShake.action -= CameraShake;
        rseCinematicFinish.action -= FinishCinematic;
        rseOnPlayerCameraLook.action -= PlayerPos;
    }

    private void Update()
    {
        CamPlayerRotate();

        PlayerHide();
    }

    private void CamPlayerRotate()
    {
        playerPoint.transform.position = playerPos.position;

        if (Quaternion.Angle(playerPoint.transform.rotation, playerPos.rotation) > 1f)
        {
            startRotation = playerPoint.transform.rotation;
            targetRotation = playerPos.rotation;
            rotationTimer = 0f;
            isRotating = true;
        }

        if (isRotating)
        {
            rotationTimer += Time.deltaTime;
            float t = Mathf.Clamp01(rotationTimer / rotationDuration);
            playerPoint.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            if (t >= 1f)
            {
                isRotating = false;
            }
        }
    }

    private void PlayerHide()
    {
        float distance = Vector3.Distance(cameraMain.transform.position, playerPos.position);
        bool shouldHide = distance <= cameraDistanceMin;

        float targetAlpha = shouldHide ? 0f : 1f;

        currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, fadeSpeed * Time.deltaTime);

        Color color = materialPlayer.color;
        color.a = currentAlpha;
        materialPlayer.color = color;
    }

    private void PlayerPos(Transform player)
    {
        playerPos = player;
        targetGroup.Targets[0].Object = player;
    }

    private void SwitchCinematicCamera(int index)
    {
        oldCamera = currentCamera;

        if (index < 0 || index >= cinemachineCameraCinematic.Count)
        {
            return;
        }

        cinemachineCameraRail.Priority = cameraUnFocusPriority;
        cinemachineCameraPlayer.Priority = cameraUnFocusPriority;
        cinemachineCameraCinematic[index].Priority = cameraCinematicPriority;
        currentCamera = cinemachineCameraCinematic[index];
        currentCamera.GetComponent<Animator>().enabled = true;
        currentCamera.GetComponent<Animator>().SetTrigger("Play");
    }

    private void FinishCinematic()
    {
        if (oldCamera != null)
        {
            oldCamera.Priority = cameraFocusPriority;
            currentCamera.GetComponent<Animator>().enabled = false;
            currentCamera.Priority = 0;
            currentCamera = oldCamera;
            oldCamera = null;
        }
    }

    private void SwitchCameraTargeting(bool value)
    {
        if (value)
        {
            cinemachineCameraRail.Priority = cameraUnFocusPriority;
            cinemachineCameraPlayer.Priority = cameraFocusPriority;
            currentCamera = cinemachineCameraPlayer;
        }
        else
        {
            cinemachineCameraRail.Priority = cameraFocusPriority;
            cinemachineCameraPlayer.Priority = cameraUnFocusPriority;
            currentCamera = cinemachineCameraRail;
        }
    }

    private void CameraShake(S_ClassCameraShake classCameraShake)
    {
        CinemachineBasicMultiChannelPerlin cam = currentCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();

        if (cam != null)
        {
            if (shake != null)
            {
                StopCoroutine(shake);

                cam.AmplitudeGain = 0;
                cam.FrequencyGain = 0;
                shake = null;
            }

            cam.AmplitudeGain = classCameraShake.amplitude;
            cam.FrequencyGain = classCameraShake.frequency;

            shake = StartCoroutine(S_Utils.Delay(classCameraShake.duration, () =>
            {
                cam.AmplitudeGain = 0;
                cam.FrequencyGain = 0;
            }));
        }
    }

    /*

    private void OnEnable()
    {
        rseCameraShake.action += CameraShake;
    }

    private void OnDisable()
    {
        rseCameraShake.action -= CameraShake;
    }

    private void Start()
    {
        allVCams = FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None);
    }

    private void LateUpdate()
    {
        FOV();
    }

    private void FOV()
    {
        if (cinemachineCameraRail == null || splineDolly == null)
            return;

        var spline = splineDolly.Spline;
        if (spline == null)
            return;

        int knotCount = fovPerKnot.Length;

        if (fovPerKnot == null || fovPerKnot.Length < knotCount)
        {
            return;
        }

        float posKnotUnits = splineDolly.CameraPosition;

        if (splineDolly.PositionUnits != PathIndexUnit.Knot)
        {
            float normalized = 0f;

            if (splineDolly.PositionUnits == PathIndexUnit.Normalized)
            {
                normalized = posKnotUnits;
            }
            else if (splineDolly.PositionUnits == PathIndexUnit.Distance)
            {
                float totalLength = spline.CalculateLength();
                normalized = Mathf.Clamp01(posKnotUnits / totalLength);
            }

            posKnotUnits = normalized * (knotCount - 1);
        }

        int indexA = Mathf.FloorToInt(posKnotUnits);
        int indexB = Mathf.Min(indexA + 1, knotCount - 1);

        float t = posKnotUnits - indexA;

        float fovA = fovPerKnot[indexA];
        float fovB = fovPerKnot[indexB];

        float fov = Mathf.Lerp(fovA, fovB, t);

        cinemachineCameraRail.Lens.FieldOfView = fov;
    }
    */
}