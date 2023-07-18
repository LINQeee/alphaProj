using UnityEngine;

[RequireComponent(typeof(RiderManager)), RequireComponent(typeof(CameraManager)), RequireComponent(typeof(PlayerLocomotion)), RequireComponent(typeof(StaminaProfile)), RequireComponent(typeof(AnimationManager))]
public class InputManager : MonoBehaviour
{
    private PlayerControls _playerControls;
    private RiderManager _riderManager;
    private CameraManager _cameraManager;
    private PlayerLocomotion _playerLocomotion;
    private StaminaProfile _staminaProfile;
    private AnimationManager _animationManager;
    public Vector2 move;
    public Vector2 look;
    public bool sprintInput;
    public bool jumpInput;

    private void Awake()
    {
        _riderManager = GetComponent<RiderManager>();
        _cameraManager = GetComponent<CameraManager>();
        _playerLocomotion = GetComponent<PlayerLocomotion>();
        _staminaProfile = GetComponent<StaminaProfile>();
        _animationManager = GetComponent<AnimationManager>();
    }

    private void OnEnable()
    {
        if (_playerControls == null)
        {
            _playerControls = new PlayerControls();

            _playerControls.Player.Move.performed += i =>
            {
                move = i.ReadValue<Vector2>();
            };
            _playerControls.Player.Look.performed += i =>
            {
                look = i.ReadValue<Vector2>();
            };

            _playerControls.Player.Sprint.performed += _ => sprintInput = true;
            _playerControls.Player.Sprint.canceled += _ => sprintInput = false;

            _playerControls.Player.Jump.performed += _ => jumpInput = true;
            _playerControls.Player.Jump.canceled += _ => jumpInput = false;

            _playerControls.Player.HorseMount.performed += _ => _riderManager.MountOrDismount();

            _playerControls.Player.Aim.performed += _ => StartAiming();
            _playerControls.Player.Aim.canceled += _ => StopAiming();
        }

        _playerControls.Enable();
    }
    
    private void StartAiming()
    {
        if (_riderManager.isMounted) return;
        _cameraManager.SwitchToAimCam();
        _playerLocomotion.IsAiming = true;
        _staminaProfile.isProfileEnabled = false;
        _animationManager.SwapMovementAnimationToAiming();
    }

    private void StopAiming()
    {
        _cameraManager.SwitchToTpCam();
        _playerLocomotion.IsAiming = false;
        _staminaProfile.isProfileEnabled = true;
        _animationManager.SetIsAiming(false);
    }

    private void OnDisable()
    {
        _playerControls.Disable();
    }
}