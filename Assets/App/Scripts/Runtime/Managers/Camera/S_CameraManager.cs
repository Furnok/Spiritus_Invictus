using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;

public class S_CameraManager : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Camera Main")]
    [SerializeField] private Camera cameraMain;

    [TabGroup("References")]
    [Title("Cinemachine")]
    [SerializeField] private CinemachineCamera cinemachineCameraIntro;

    [TabGroup("References")]
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
    [SerializeField] private RSE_OnCameraIntro rseOnCameraIntro;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnCameraCinematic rseOnCameraCinematic;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnCinematicFinish rseOnCinematicFinish;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnCameraShake rseOnCameraShake;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerCenter rseOnPlayerCenter;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnSkipInput rseOnSkipInput;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnSkipCancelInput rseOnSkipCancelInput;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnCinematicInputEnabled rseOnCinematicInputEnabled;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnCinematicStart rseOnCinematicStart;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnGameInputEnabled rseOnGameInputEnabled;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnDisplayUIGame rseOnDisplayUIGame;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnDisplaySkip rseOnDisplaySkip;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnSkipHold rseOnSkipHold;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnCancelTargeting rseOnCancelTargeting;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_PlayerIsDodging rsoPlayerIsDodging;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_CameraData ssoCameraData;

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
    private Tween playerRotationTween = null;
    private Coroutine skip = null;
    private bool isSkipping = false;
    private float currentHold = 0f;

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
        rseOnCameraIntro.action += CameraIntro;
        rseOnCameraCinematic.action += SwitchCinematicCamera;
        rseOnCameraShake.action += CameraShake;
        rseOnPlayerMove.action += InputsMove;
        rseOnCinematicFinish.action += FinishCinematic;
        rseOnSkipInput.action += Skip;
        rseOnSkipCancelInput.action += SkipCancel;
    }

    private void OnDisable()
    {
        rseOnPlayerCenter.action -= PlayerPos;
        rseOnNewTargeting.action -= SwitchCameraMode;
        rseOnPlayerCancelTargeting.action -= SwitchCameraMode;
        rseOnCameraIntro.action -= CameraIntro;
        rseOnCameraCinematic.action -= SwitchCinematicCamera;
        rseOnCameraShake.action -= CameraShake;
        rseOnPlayerMove.action -= InputsMove;
        rseOnCinematicFinish.action -= FinishCinematic;
        rseOnSkipInput.action -= Skip;
        rseOnSkipCancelInput.action -= SkipCancel;

        Color color = materialPlayer.color;
        color.a = 1;
        materialPlayer.color = color;
    }

    private void Update()
    {
        playerPoint.transform.position = playerPos.position;

        CamPlayerRotate();

        PlayerHide();

        if (isSkipping)
        {
            currentHold += Time.deltaTime;

            rseOnSkipHold.Call(currentHold);

            if (currentHold >= ssoCameraData.Value.holdSkipTime)
            {
                SkipCinematic();  
            }
        }
    }

    private void Skip()
    {
        isSkipping = true;
        currentHold = 0f;

        rseOnDisplaySkip.Call(true);
        rseOnSkipHold.Call(currentHold);
    }

    private void SkipCancel()
    {
        isSkipping = false;
        currentHold = 0f;

        rseOnSkipHold.Call(currentHold);
    }

    private void SkipCinematic()
    {
        FinishCinematic();
    }

    private void PlayerPos(Transform player)
    {
        playerPos = player;
        targetGroupRail.Targets[0].Object = player;
    }

    private void CameraIntro()
    {
        if (cinemachineCameraIntro.transform.parent.gameObject.activeInHierarchy)
        {
            skip = StartCoroutine(S_Utils.Delay(ssoCameraData.Value.StartDisplaySkipTime, () => rseOnDisplaySkip.Call(true)));

            rseOnDisplayUIGame.Call(false);
            rseOnCinematicStart.Call();
            rseOnCinematicInputEnabled.Call();

            currentCamera = cinemachineCameraIntro;
            cinemachineCameraIntro.GetComponent<Animator>().Rebind();
            cinemachineCameraIntro.GetComponent<Animator>().Update(0f);
            cinemachineCameraIntro.GetComponent<Animator>().enabled = true;
            cinemachineCameraIntro.GetComponent<Animator>().SetTrigger("Play");

            currentMode = ModeCamera.Cinematic;
        }
        else
        {
            rseOnDisplayUIGame.Call(true);
            rseOnGameInputEnabled.Call();
        }
    }

    private void SwitchCameraMode(GameObject target)
    {
        if (currentTargetPos != null)
        {
            currentTargetPos = null;

            cinemachineCameraPlayer.Priority = unFocus;
            cinemachineCameraRail.Priority = focus;

            currentCamera = cinemachineCameraRail;
            currentMode = ModeCamera.Rail;
        }
        else
        {
            currentTargetPos = target.transform;
            cinemachineThirdPersonFollow.ShoulderOffset = ssoCameraData.Value.targetShoulderOffsetPositive;
            lastDirection = 0;

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

        rseOnCancelTargeting.Call();
        skip = StartCoroutine(S_Utils.Delay(ssoCameraData.Value.StartDisplaySkipTime, () => rseOnDisplaySkip.Call(true)));

        rseOnDisplayUIGame.Call(false);
        rseOnCinematicStart.Call();
        rseOnCinematicInputEnabled.Call();

        currentTargetPos = null;

        cinemachineCameraRail.Priority = unFocus;
        cinemachineCameraPlayer.Priority = unFocus;
        cinemachineCameraCinematic[index].Priority = focusCinematic;
        currentCamera = cinemachineCameraCinematic[index];

        currentCamera.GetComponent<Animator>().Rebind();
        currentCamera.GetComponent<Animator>().Update(0f);
        currentCamera.GetComponent<Animator>().enabled = true;
        currentCamera.GetComponent<Animator>().SetTrigger("Play");

        currentMode = ModeCamera.Cinematic;
    }

    private void FinishCinematic()
    {
        currentHold = 0f;
        rseOnSkipHold.Call(currentHold);
        rseOnDisplaySkip.Call(false);
        
        if (skip != null)
        {
            StopCoroutine(skip);
            skip = null;
        }

        currentCamera.GetComponent<Animator>().enabled = false;

        currentCamera.Priority = unFocus;

        cinemachineCameraRail.Priority = focus;
        currentCamera = cinemachineCameraRail;

        currentMode = ModeCamera.Rail;

        isSkipping = false;
        rseOnDisplayUIGame.Call(true);
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
        else
        {
            shoulderOffsetTween?.Kill();
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
            ssoCameraData.Value.switchDurationCamera
        ).SetEase(Ease.Linear);
    }

    private void CamPlayerRotate()
    {
        if (currentMode != ModeCamera.Player)
        {
            playerRotationTween?.Kill();
            return;
        }

        playerRotationTween?.Kill();

        Vector3 directionToTarget = (currentTargetPos.position - playerPos.position).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);

        float angle = Quaternion.Angle(playerPoint.transform.rotation, targetRotation);

        if (angle > 0.5f)
        {
            float duration = rsoPlayerIsDodging.Value ? ssoCameraData.Value.rotationCameraPlayerDodgeDuration : ssoCameraData.Value.rotationCameraPlayerDuration;

            playerRotationTween = playerPoint.transform.DORotateQuaternion(targetRotation, duration).SetEase(Ease.Linear);
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
    
    /*
    private void OnDrawGizmos()
    {
        if (cinemachineCameraRail == null || cinemachineCameraPlayer == null) return;

        // Draw line between cameras
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(cinemachineCameraRail.transform.position, cinemachineCameraPlayer.transform.position);

        // Draw bridge position
        Vector3 bridgePos = Vector3.Lerp(cinemachineCameraRail.transform.position, cinemachineCameraPlayer.transform.position, 0.5f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(bridgePos, 0.2f);

        // Draw arrow toward target if assigned
        if (currentTargetPos != null)
        {
            Handles.color = Color.green;
            Handles.ArrowHandleCap(
                0,
                bridgePos,
                Quaternion.LookRotation(currentTargetPos.position - bridgePos),
                1f,
                EventType.Repaint
            );
        }
    }*/
}