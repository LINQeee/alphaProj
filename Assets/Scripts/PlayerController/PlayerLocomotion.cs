using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(StaminaProfile)), RequireComponent(typeof(CharacterController)),
 RequireComponent(typeof(InputManager)), RequireComponent(typeof(AnimationManager)), RequireComponent(typeof(CameraManager))]
public class PlayerLocomotion : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4;
    [SerializeField] private float sprintSpeed = 5.335f;
    [SerializeField] private float speedChangeRate = 10;
    [SerializeField] private float rotationSmoothTime = 0.12f;
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float gravity = -15;
    [SerializeField] private float groundedOffset = -0.14f;
    [SerializeField] private float groundedRadius = 0.28f;
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private float jumpTimeout = 0.5f;
    [SerializeField] private float fallTimeout = 0.2f;
    [SerializeField] private AudioClip landingAudioClip;
    [SerializeField] private AudioClip[] footstepAudioClips;
    [SerializeField] [Range(0, 1)] private float footstepAudioVolume;
    [NonSerialized] public bool IsAiming;

    private CameraManager _cameraManager;
    private StaminaProfile _staminaProfile;
    private CharacterController _controller;
    private InputManager _inputManager;
    private AnimationManager _animationManager;
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;
    private float _speed;
    private bool _grounded;
    private float _targetRotation;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private const float TerminalVelocity = 53;
    private float _animationBlend;


    private void Awake()
    {
        _staminaProfile = GetComponent<StaminaProfile>();
        _controller = GetComponent<CharacterController>();
        _inputManager = GetComponent<InputManager>();
        _animationManager = GetComponent<AnimationManager>();
        _cameraManager = GetComponent<CameraManager>();

        _jumpTimeoutDelta = jumpTimeout;
        _fallTimeoutDelta = fallTimeout;
    }

    public void HandleMovement()
    {
        float targetSpeed = _staminaProfile.IsCanRun && !IsAiming ? sprintSpeed : moveSpeed;

        if (_inputManager.move == Vector2.zero) targetSpeed = 0;

        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;

        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * speedChangeRate);

            _speed = Mathf.Round(_speed * 1000) / 1000;
        }
        else
        {
            _speed = targetSpeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0;

        Vector3 inputDirection = IsAiming ? Vector3.forward : new Vector3(_inputManager.move.x, 0, _inputManager.move.y).normalized;

        if (_inputManager.move != Vector2.zero || IsAiming)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                              _cameraManager.currentCinemachine.Follow.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                rotationSmoothTime);

            transform.rotation = Quaternion.Euler(0, rotation, 0);
        }

        Vector3 targetDirection = IsAiming ?
            transform.rotation * new Vector3(_inputManager.move.x, 0, _inputManager.move.y) :
            Quaternion.Euler(0, _targetRotation, 0) * Vector3.forward;

        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                         new Vector3(0, _verticalVelocity, 0) * Time.deltaTime);

        _animationManager.SetSpeed(_animationBlend);
    }

    public void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset,
            transform.position.z);
        _grounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);

        _animationManager.SetGrounded(_grounded);
    }

    public void HandleJumpAndGravity()
    {
        if (_grounded)
        {
            _fallTimeoutDelta = fallTimeout;

            _animationManager.SetJump(false);

            if (_verticalVelocity < 0) _verticalVelocity = -2;

            if (_inputManager.jumpInput && _jumpTimeoutDelta <= 0 && !IsAiming && _staminaProfile.IsCanJump())
            {
                _verticalVelocity = Mathf.Sqrt(jumpHeight * -2 * gravity);

                _animationManager.PlayJump();
                _animationManager.SetJump(true);
            }

            if (_jumpTimeoutDelta >= 0)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            _jumpTimeoutDelta = jumpTimeout;

            if (_fallTimeoutDelta >= 0)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                if (!_animationManager.GetJump()) _animationManager.PlayInAir();
            }
        }

        if (_verticalVelocity < TerminalVelocity) _verticalVelocity += gravity * Time.deltaTime;
    }

    // ReSharper disable once UnusedMember.Local
    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (!(animationEvent.animatorClipInfo.weight > 0.5f)) return;
        if (footstepAudioClips.Length <= 0) return;
        var index = Random.Range(0, footstepAudioClips.Length);
        AudioSource.PlayClipAtPoint(footstepAudioClips[index], transform.TransformPoint(_controller.center),
            footstepAudioVolume);
    }

    // ReSharper disable once UnusedMember.Local
    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(landingAudioClip, transform.TransformPoint(_controller.center),
                footstepAudioVolume);
        }
    }
    
    private void FootR(AnimationEvent animationEvent){}
    private void FootL(AnimationEvent animationEvent){}
}