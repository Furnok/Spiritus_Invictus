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
    [SerializeField] private CinemachineTargetGroup targetGroupRail;

    [TabGroup("References")]
    [SerializeField] private Transform playerPoint;

    [TabGroup("References")]
    [Title("Player")]
    [SerializeField] private Material materialPlayer;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerMove rseOnPlayerMove;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnNewTargeting rseOnNewTargeting;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerCancelTargeting rseOnPlayerCancelTargeting;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnCameraCinematic rseOnCameraCinematic;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnCinematicFinish rseOnCinematicFinish;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnCameraShake rseOnCameraShake;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerCenter rseOnPlayerCenter;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_CameraData ssoCameraData;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnCinematicInputEnabled rseOnCinematicInputEnabled;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnGameInputEnabled rseOnGameInputEnabled;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_PlayerIsDodging rsoPlayerIsDodging;


    private Coroutine shake = null;
    private CinemachineCamera currentCamera = null;
    private float currentAlpha = 1f;
    private Tween shoulderOffsetTween;
    private float lastDirection = 0f;
    private Transform playerPos = null;
    private Transform currentTargetPos = null;
    private ModeCamera currentMode = ModeCamera.None;
    private int focus = 2;
    private int focusCinematic = 100;
    private int unFocus = 1;
    private Tween playerRotationTween;

    private void Awake()
    {
        currentCamera = cinemachineCameraRail;
        currentMode = ModeCamera.Rail;

        if (cinemachineThirdPersonFollow != null)
        {
            cinemachineThirdPersonFollow.ShoulderOffset = ssoCameraData.Value.targetShoulderOffsetPositive;
        }
    }

    private void OnEnable()
    {
        rseOnPlayerCenter.action += PlayerPos;
        rseOnNewTargeting.action += SwitchCameraMode;
        rseOnPlayerCancelTargeting.action += SwitchCameraMode;
        rseOnCameraCinematic.action += SwitchCinematicCamera;
        rseOnCameraShake.action += CameraShake;
        rseOnPlayerMove.action += InputsMove;
        rseOnCinematicFinish.action += FinishCinematic;
    }

    private void OnDisable()
    {
        rseOnPlayerCenter.action -= PlayerPos;
        rseOnNewTargeting.action -= SwitchCameraMode;
        rseOnPlayerCancelTargeting.action -= SwitchCameraMode;
        rseOnCameraCinematic.action -= SwitchCinematicCamera;
        rseOnCameraShake.action -= CameraShake;
        rseOnPlayerMove.action -= InputsMove;
        rseOnCinematicFinish.action -= FinishCinematic;
    }

    private void Update()
    {
        playerPoint.transform.position = playerPos.position;

        CamPlayerRotate();

        PlayerHide();
    }

    private void PlayerPos(Transform player)
    {
        playerPos = player;
        targetGroupRail.Targets[0].Object = player;
    }

    private void SwitchCameraMode(GameObject target)
    {
        if (currentTargetPos != null)
        {
            currentTargetPos = null;

            cinemachineCameraRail.Priority = focus;
            cinemachineCameraPlayer.Priority = unFocus;
            currentCamera = cinemachineCameraRail;

            currentMode = ModeCamera.Rail;
        }
        else
        {
            currentTargetPos = target.transform;

            cinemachineCameraRail.Priority = unFocus;
            cinemachineCameraPlayer.Priority = focus;
            currentCamera = cinemachineCameraPlayer;

            currentMode = ModeCamera.Player;
        }
    }

    private void SwitchCinematicCamera(int index)
    {
        if (index < 0 || index >= cinemachineCameraCinematic.Count)
        {
            return;
        }

        rseOnCinematicInputEnabled.Call();

        currentTargetPos = null;

        cinemachineCameraRail.Priority = unFocus;
        cinemachineCameraPlayer.Priority = unFocus;
        cinemachineCameraCinematic[index].Priority = focusCinematic;
        currentCamera = cinemachineCameraCinematic[index];
        currentCamera.GetComponent<Animator>().enabled = true;
        currentCamera.GetComponent<Animator>().SetTrigger("Play");

        currentMode = ModeCamera.Cinematic;
    }

    private void FinishCinematic()
    {
        currentCamera.GetComponent<Animator>().enabled = false;
        currentCamera.Priority = unFocus;

        cinemachineCameraRail.Priority = focus;
        currentCamera = cinemachineCameraRail;

        currentMode = ModeCamera.Rail;

        rseOnGameInputEnabled.Call();
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

    private void InputsMove(Vector2 move)
    {
        if (currentMode == ModeCamera.Player)
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
        if (currentMode != ModeCamera.Player)
            return;

        playerRotationTween?.Kill();

        Vector3 directionToTarget = (currentTargetPos.position - playerPos.position).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);

        float angle = Quaternion.Angle(playerPoint.transform.rotation, targetRotation);

        if (angle > 1f)
        {
            float duration = rsoPlayerIsDodging.Value ? ssoCameraData.Value.rotationCameraPlayerDodgeDuration : ssoCameraData.Value.rotationCameraPlayerDuration;

            playerRotationTween = playerPoint.transform.DORotateQuaternion(targetRotation, duration).SetEase(Ease.Linear);
        }
    }

    private void PlayerHide()
    {
        if (currentMode == ModeCamera.Player)
        {
            float distance = Vector3.Distance(cameraMain.transform.position, playerPos.position);
            bool shouldHide = distance <= ssoCameraData.Value.cameraDistanceMinPlayer;

            float targetAlpha = shouldHide ? 0f : 1f;

            currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, ssoCameraData.Value.fadeSpeedPlayer * Time.deltaTime);

            Color color = materialPlayer.color;
            color.a = currentAlpha;
            materialPlayer.color = color;
        }
    }
}