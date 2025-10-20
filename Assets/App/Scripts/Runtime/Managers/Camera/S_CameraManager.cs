using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class S_CameraManager : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Camera Main")]
    [SerializeField] private Camera cameraMain;

    [TabGroup("References")]
    [Title("Cinemachine")]
    [SerializeField] private CinemachineCamera cinemachineCameraRail;

    [TabGroup("References")]
    [SerializeField] private CinemachineCamera cinemachineCameraPlayer;

    [TabGroup("References")]
    [SerializeField] private CinemachineThirdPersonFollow cinemachineThirdPersonFollow;

    [TabGroup("References")]
    [SerializeField] private List<CinemachineCamera> cinemachineCameraCinematic;

    [TabGroup("References")]
    [Title("Target")]
    [SerializeField] private CinemachineTargetGroup targetGroup;

    [TabGroup("References")]
    [SerializeField] private GameObject playerPoint;

    [TabGroup("References")]
    [Title("Player")]
    [SerializeField] private Material materialPlayer;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnCameraCinematic rseOnCameraCinematic;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnCinematicFinish rseOnCinematicFinish;

    [TabGroup("Inputs")]
    [SerializeField] private RSO_PlayerIsTargeting rsoPlayerIsTargeting;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnCameraShake rseOnCameraShake;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerCenter rseOnPlayerCenter;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerMove rseOnPlayerMove;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_CameraData ssoCameraData;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnCinematicInputEnabled rseOnCinematicInputEnabled;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnGameInputEnabled rseOnGameInputEnabled;

    private Coroutine shake = null;
    private CinemachineCamera currentCamera = null;
    private Transform playerPos = null;
    private float currentAlpha = 1f;
    private Quaternion startRotation = Quaternion.identity;
    private Quaternion targetRotation = Quaternion.identity;
    private float rotationTimer = 0f;
    private bool isRotating = false;
    private int focus = 2;
    private int focusCinematic = 100;
    private int unFocus = 1;
    private Tween shoulderOffsetTween;
    private float lastDirection = 0f;

    public List<CinemachineCamera> GetListCameraCinematic()
    {
        return cinemachineCameraCinematic;
    }

    private void OnEnable()
    {
        rseOnCameraCinematic.action += SwitchCinematicCamera;
        rsoPlayerIsTargeting.onValueChanged += SwitchCameraTargeting;
        rseOnCameraShake.action += CameraShake;
        rseOnCinematicFinish.action += FinishCinematic;
        rseOnPlayerCenter.action += PlayerPos;
        rseOnPlayerMove.action += InputsMove;

        currentCamera = cinemachineCameraRail;
    }

    private void OnDisable()
    {
        rseOnCameraCinematic.action -= SwitchCinematicCamera;
        rsoPlayerIsTargeting.onValueChanged -= SwitchCameraTargeting;
        rseOnCameraShake.action -= CameraShake;
        rseOnCinematicFinish.action -= FinishCinematic;
        rseOnPlayerCenter.action -= PlayerPos;
        rseOnPlayerMove.action -= InputsMove;
    }

    private void Awake()
    {
        cinemachineThirdPersonFollow.ShoulderOffset = ssoCameraData.Value.targetShoulderOffsetPositive;
    }

    private void Update()
    {
        CamPlayerRotate();

        PlayerHide();
    }

    private void InputsMove(Vector2 move)
    {
        if (rsoPlayerIsTargeting.Value)
        {
            if (move.x > 0 && lastDirection <= 0)
            {
                ChangeShoulderOffset(ssoCameraData.Value.targetShoulderOffsetNegative);
                lastDirection = move.x;
            }
            else if (move.x < 0 && lastDirection >= 0)
            {
                ChangeShoulderOffset(ssoCameraData.Value.targetShoulderOffsetPositive);
                lastDirection = move.x;
            }
        }
    }

    private void ChangeShoulderOffset(Vector3 targetOffset)
    {
        if (cinemachineThirdPersonFollow == null)
            return;

        shoulderOffsetTween?.Kill();

        shoulderOffsetTween = DOTween.To(
            () => cinemachineThirdPersonFollow.ShoulderOffset,
            x => cinemachineThirdPersonFollow.ShoulderOffset = x,
            targetOffset,
            ssoCameraData.Value.switchTimeCamera
        ).SetEase(Ease.Linear);
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
            float t = Mathf.Clamp01(rotationTimer / ssoCameraData.Value.rotationCameraPlayerDuration);
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
        bool shouldHide = distance <= ssoCameraData.Value.cameraDistanceMinPlayer;

        float targetAlpha = shouldHide ? 0f : 1f;

        currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, ssoCameraData.Value.fadeSpeedPlayer * Time.deltaTime);

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
        if (index < 0 || index >= cinemachineCameraCinematic.Count)
        {
            return;
        }

        rseOnCinematicInputEnabled.Call();

        cinemachineCameraRail.Priority = unFocus;
        cinemachineCameraPlayer.Priority = unFocus;
        cinemachineCameraCinematic[index].Priority = focusCinematic;
        currentCamera = cinemachineCameraCinematic[index];
        currentCamera.GetComponent<Animator>().enabled = true;
        currentCamera.GetComponent<Animator>().SetTrigger("Play");
    }

    private void FinishCinematic()
    {
        currentCamera.GetComponent<Animator>().enabled = false;
        currentCamera.Priority = unFocus;

        cinemachineCameraRail.Priority = focus;
        currentCamera = cinemachineCameraRail;

        rseOnGameInputEnabled.Call();
    }

    private void SwitchCameraTargeting(bool value)
    {
        if (value)
        {
            cinemachineCameraRail.Priority = unFocus;
            cinemachineCameraPlayer.Priority = focus;
            currentCamera = cinemachineCameraPlayer;
        }
        else
        {
            cinemachineCameraRail.Priority = focus;
            cinemachineCameraPlayer.Priority = unFocus;
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
}