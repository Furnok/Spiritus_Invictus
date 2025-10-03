using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class S_CameraManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera cameraMain;
    [SerializeField] private Material materialPlayer;
    [SerializeField] private CinemachineCamera cinemachineCameraRail;
    [SerializeField] private CinemachineCamera cinemachineCameraPlayer;
    [SerializeField] private CinemachineTargetGroup targetGroup;
    [SerializeField] private GameObject playerPoint;
    [SerializeField] private List<CinemachineCamera> cinemachineCameraCinematic;

    [Header("Input")]
    [SerializeField] private RSE_OnCameraCinematic rseOnCameraCinematic;
    [SerializeField] private RSE_OnCinematicFinish rseOnCinematicFinish;
    [SerializeField] private RSO_PlayerIsTargeting rsoPlayerIsTargeting;
    [SerializeField] private RSE_OnCameraShake rseOnCameraShake;
    [SerializeField] private RSE_OnPlayerCenter rseOnPlayerCenter;

    [Header("Output")]
    [SerializeField] private SSO_CameraData ssoCameraData;
    [SerializeField] private RSE_OnCinematicInputEnabled rseOnCinematicInputEnabled;
    [SerializeField] private RSE_OnGameInputEnabled rseOnGameInputEnabled;

    private Coroutine shake = null;
    private CinemachineCamera currentCamera = null;
    private Transform playerPos = null;
    private float currentAlpha = 1f;
    private Quaternion startRotation = Quaternion.identity;
    private Quaternion targetRotation = Quaternion.identity;
    private float rotationTimer = 0f;
    private bool isRotating = false;

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

        currentCamera = cinemachineCameraRail;
    }

    private void OnDisable()
    {
        rseOnCameraCinematic.action -= SwitchCinematicCamera;
        rsoPlayerIsTargeting.onValueChanged -= SwitchCameraTargeting;
        rseOnCameraShake.action -= CameraShake;
        rseOnCinematicFinish.action -= FinishCinematic;
        rseOnPlayerCenter.action -= PlayerPos;
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
            float t = Mathf.Clamp01(rotationTimer / ssoCameraData.Value.rotationCameraDuration);
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

        cinemachineCameraRail.Priority = ssoCameraData.Value.cameraUnFocusPriority;
        cinemachineCameraPlayer.Priority = ssoCameraData.Value.cameraUnFocusPriority;
        cinemachineCameraCinematic[index].Priority = ssoCameraData.Value.cameraCinematicPriority;
        currentCamera = cinemachineCameraCinematic[index];
        currentCamera.GetComponent<Animator>().enabled = true;
        currentCamera.GetComponent<Animator>().SetTrigger("Play");
    }

    private void FinishCinematic()
    {
        currentCamera.GetComponent<Animator>().enabled = false;
        currentCamera.Priority = 0;

        cinemachineCameraRail.Priority = ssoCameraData.Value.cameraFocusPriority;
        currentCamera = cinemachineCameraRail;

        rseOnGameInputEnabled.Call();
    }

    private void SwitchCameraTargeting(bool value)
    {
        if (value)
        {
            cinemachineCameraRail.Priority = ssoCameraData.Value.cameraUnFocusPriority;
            cinemachineCameraPlayer.Priority = ssoCameraData.Value.cameraFocusPriority;
            currentCamera = cinemachineCameraPlayer;
        }
        else
        {
            cinemachineCameraRail.Priority = ssoCameraData.Value.cameraFocusPriority;
            cinemachineCameraPlayer.Priority = ssoCameraData.Value.cameraUnFocusPriority;
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