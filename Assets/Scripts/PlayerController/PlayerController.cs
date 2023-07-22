using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Utils;

[RequireComponent(typeof(PlayerLocomotion)), RequireComponent(typeof(CameraManager)),
 RequireComponent(typeof(StaminaProfile)), RequireComponent(typeof(AnimationManager)),
 RequireComponent(typeof(RiderManager)), RequireComponent(typeof(InputManager)),
 RequireComponent(typeof(WeaponManager)), RequireComponent(typeof(RigBuilder))]
public class PlayerController : MonoBehaviour
{
    public bool isEnabledController = true;
    public bool lockCameraPosition;


    [NonSerialized] public PlayerLocomotion playerLocomotion;
    [NonSerialized] public CameraManager cameraManager;
    [NonSerialized] public StaminaProfile staminaProfile;
    [NonSerialized] public AnimationManager animationManager;
    [NonSerialized] public InputManager inputManager;
    [NonSerialized] public RigBuilder rigBuilder;
    [NonSerialized] public RiderManager riderManager;
    [NonSerialized] public WeaponManager weaponManager;
    [NonSerialized] public CharacterController characterController;

    private void Awake()
    {
        playerLocomotion = GetComponent<PlayerLocomotion>();
        cameraManager = GetComponent<CameraManager>();
        staminaProfile = GetComponent<StaminaProfile>();
        inputManager = GetComponent<InputManager>();
        animationManager = GetComponent<AnimationManager>();
        rigBuilder = GetComponent<RigBuilder>();
        rigBuilder.DisableLayers();
        riderManager = GetComponent<RiderManager>();
        weaponManager = GetComponent<WeaponManager>();
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (!isEnabledController) return;
        playerLocomotion.GroundedCheck();
        playerLocomotion.HandleJumpAndGravity();
        playerLocomotion.HandleMovement();
    }

    private void LateUpdate()
    {
        animationManager.SetAnimatorAxis();
        if (lockCameraPosition) return;
        cameraManager.CameraRotation();
    }
}