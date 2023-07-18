using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(HorseAnimationManager)), RequireComponent(typeof(StaminaProfile))]
public class HorseLocomotion : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float turnSpeed = 1f;
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private float groundedOffsetY = 1f;
    [SerializeField] private float groundedOffsetZ = -0.5f;

    private Rigidbody _rb;
    private HorseAnimationManager _animationManager;
    private StaminaProfile _staminaProfile;
    [SerializeField] private InputManager inputManager;
    [NonSerialized] public bool IsJumping;
    [SerializeField] private Transform cameraTransform;
    [NonSerialized] public Vector2 HorseInput;

    private bool _isSprinting;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _animationManager = GetComponent<HorseAnimationManager>();
        _staminaProfile = GetComponent<StaminaProfile>();
    }

    public void ResetInput()
    {
        _isSprinting = false;
        HorseInput = Vector2.zero;
    }

    public void CalcInputAndHandleJumping()
    {
        UpdateInputAndIsSprinting();
        JumpHorse();
    }

    public void HandleMovement()
    {
        RotateHorse();
        MoveHorseIfVerticalInputIsPositive();
    }

    public bool IsGrounded()
    {
        Vector3 spherePosition = transform.position;
        spherePosition.y -= groundedOffsetY;
        spherePosition.z -= groundedOffsetZ;
        return Physics.CheckSphere(spherePosition, 0.6f, groundLayers, QueryTriggerInteraction.Ignore);
    }

    private void JumpHorse()
    {
        if (!inputManager.jumpInput || IsJumping || HorseInput.y == 0 ||
            !_staminaProfile.IsCanJump()) return;
        IsJumping = true;
        _animationManager.AnimateJumpingRelativeToSpeed(_isSprinting);
    }

    private void UpdateInputAndIsSprinting()
    {
        float horizontal = inputManager.move.x;
        float vertical = inputManager.move.y;
        Sides cameraPosRelativeToHorsePos = ObjPosRelativeToCamPos();

        HorseInput = IsBackwardInput(vertical, horizontal) switch
        {
            false when IsJumping => new Vector2(horizontal, 1),
            true when cameraPosRelativeToHorsePos == Sides.Forward => new Vector2(0, 1),
            true when cameraPosRelativeToHorsePos == Sides.Left => new Vector2(-1, 1),
            true when cameraPosRelativeToHorsePos == Sides.Right => new Vector2(1, 1),
            false when horizontal != 0 => new Vector2(horizontal, 1),
            false when horizontal == 0 => new Vector2(horizontal, vertical),
            _ => Vector2.zero
        };

        _isSprinting = inputManager.sprintInput switch
        {
            true when HorseInput.y != 0 => true,
            false => false,
            _ => _isSprinting
        };
    }

    private static bool IsBackwardInput(float vertical, float horizontal) => horizontal == 0 && vertical < 0;

    private Sides ObjPosRelativeToCamPos()
    {
        float dotResult = Vector3.Dot(transform.right, cameraTransform.forward);
        return dotResult switch
        {
            < 0.1f and > -0.1f => Sides.Forward,
            > 0 => Sides.Left,
            _ => Sides.Right
        };
    }

    private void RotateHorse()
    {
        if (HorseInput is { x: 0, y: < 0 }) HorseInput.x = 1;
        Vector3 newRotation = Vector3.zero;
        newRotation.y = HorseInput.x * turnSpeed;
        transform.Rotate(newRotation);
    }

    private void MoveHorseIfVerticalInputIsPositive()
    {
        if (HorseInput.y < 0) return;

        var newVelocity = new Vector3(0, _rb.velocity.y,
            _staminaProfile.IsCanRun
                ? HorseInput.y * runSpeed
                : HorseInput.y * moveSpeed);
        _rb.velocity = transform.TransformVector(newVelocity);
    }

    private enum Sides
    {
        Left,
        Forward,
        Right
    }
}