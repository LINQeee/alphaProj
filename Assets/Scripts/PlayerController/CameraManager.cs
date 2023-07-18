using System;
using Cinemachine;
using UnityEngine;

public class CameraManager:MonoBehaviour
{
    
    [SerializeField] private float cameraAngleOverride;
    [SerializeField] private CinemachineVirtualCamera cinemachineAimCam;
    [SerializeField] private CinemachineVirtualCamera cinemachineTpCam;
    [SerializeField] private Transform playerCameraFollow;
    [SerializeField] private float topClamp = 70;
    [SerializeField] private float bottomClamp = -30;
    [NonSerialized] public CinemachineVirtualCamera currentCinemachine;
    
    private InputManager _inputManager;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private const float Threshold = 0.01f;
    
    private void Awake()
    {
        _inputManager = GetComponent<InputManager>();
        currentCinemachine = cinemachineTpCam;
        _cinemachineTargetYaw = currentCinemachine.Follow.transform.rotation.eulerAngles.y;
    }

    public void SwitchToAimCam()
    {
        cinemachineAimCam.enabled = true;
        cinemachineTpCam.enabled = false;
    }

    public void SwitchToTpCam()
    {
        cinemachineTpCam.enabled = true;
        cinemachineAimCam.enabled = false;
    }
    
    public void ResetTpCameraTarget()
    {
        cinemachineTpCam.Follow = playerCameraFollow;
    }

    public void SwapTpCameraTarget(Transform newTarget)
    {
        cinemachineTpCam.Follow = newTarget;
    }

    public void CameraRotation()
    {
        if (_inputManager.look.sqrMagnitude >= Threshold)
        {
            _cinemachineTargetYaw += _inputManager.look.x;
            _cinemachineTargetPitch += _inputManager.look.y;
        }

        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);

        currentCinemachine.Follow.transform.rotation =
            Quaternion.Euler(_cinemachineTargetPitch + cameraAngleOverride, _cinemachineTargetYaw, 0);
    }
    
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}