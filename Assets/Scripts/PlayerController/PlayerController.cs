using UnityEngine;

[RequireComponent(typeof(PlayerLocomotion)), RequireComponent(typeof(CameraManager)), RequireComponent(typeof(StaminaProfile)), RequireComponent(typeof(AnimationManager))]
public class PlayerController : MonoBehaviour
{
    
    public bool isEnabledController = true;
    public bool lockCameraPosition;


    private PlayerLocomotion _playerLocomotion;
    private CameraManager _cameraManager;
    private StaminaProfile _staminaProfile;
    private AnimationManager _animationManager;

    private void Awake()
    {
        _playerLocomotion = GetComponent<PlayerLocomotion>();
        _cameraManager = GetComponent<CameraManager>();
        _staminaProfile = GetComponent<StaminaProfile>();
        _staminaProfile.isProfileEnabled = true;
        _staminaProfile.ResetProfile();
        _staminaProfile.inputManager = GetComponent<InputManager>();
        _animationManager = GetComponent<AnimationManager>();
    }
    
    private void Update()
    {
        if (!isEnabledController) return;
        _playerLocomotion.GroundedCheck();
        _playerLocomotion.HandleJumpAndGravity();
        _playerLocomotion.HandleMovement();
    }

    private void LateUpdate()
    {
        _animationManager.SetAnimatorAxis();
        if (lockCameraPosition) return;
        _cameraManager.CameraRotation();
    }
}