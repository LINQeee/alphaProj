using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(PlayerController))]
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

    private PlayerController _playerController;
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;
    private float _speed;
    private bool _grounded;
    private float _targetRotation;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private const float TerminalVelocity = 53;
    private float _animationBlend;
    private bool _isJumping;


    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();

        _jumpTimeoutDelta = jumpTimeout;
        _fallTimeoutDelta = fallTimeout;
    }

    public void HandleMovement()
    {
        bool isAiming = _playerController.weaponManager.isAiming;
        
        float targetSpeed = _playerController.staminaProfile.isCanRun && !isAiming ? sprintSpeed : moveSpeed;

        if (_playerController.inputManager.move == Vector2.zero) targetSpeed = 0;

        float currentHorizontalSpeed = new Vector3(_playerController.characterController.velocity.x, 0, _playerController.characterController.velocity.z).magnitude;

        const float speedOffset = 0.1f;

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

        Vector3 inputDirection = isAiming ? Vector3.forward : new Vector3(_playerController.inputManager.move.x, 0, _playerController.inputManager.move.y).normalized;

        if (_playerController.inputManager.move != Vector2.zero || isAiming)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                              _playerController.cameraManager.currentCinemachine.Follow.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                rotationSmoothTime);

            transform.rotation = Quaternion.Euler(0, rotation, 0);
        }

        Vector3 targetDirection = isAiming ?
            transform.rotation * new Vector3(_playerController.inputManager.move.x, 0, _playerController.inputManager.move.y) :
            Quaternion.Euler(0, _targetRotation, 0) * Vector3.forward;

        _playerController.characterController.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                         new Vector3(0, _verticalVelocity, 0) * Time.deltaTime);

        _playerController.animationManager.SetSpeed(_animationBlend);
    }

    public void GroundedCheck()
    {
        var spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset,
            transform.position.z);
        _grounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);

        _playerController.animationManager.SetGrounded(_grounded);
    }

    public void HandleJumpAndGravity()
    {
        if (_grounded)
        {
            _fallTimeoutDelta = fallTimeout;

            _isJumping = false;

            if (_verticalVelocity < 0) _verticalVelocity = -2;

            if (_playerController.inputManager.jumpInput && _jumpTimeoutDelta <= 0 && !_playerController.weaponManager.isAiming && _playerController.staminaProfile.IsCanJump())
            {
                _verticalVelocity = Mathf.Sqrt(jumpHeight * -2 * gravity);

                _playerController.animationManager.AnimateJumping();
                _isJumping = true;
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
                if (!_isJumping) _playerController.animationManager.AnimateFalling();
            }
        }

        if (_verticalVelocity < TerminalVelocity) _verticalVelocity += gravity * Time.deltaTime;
    }

    // ReSharper disable once UnusedMember.Local
    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (!(animationEvent.animatorClipInfo.weight > 0.5f)) return;
        if (footstepAudioClips.Length <= 0) return;
        int index = Random.Range(0, footstepAudioClips.Length);
        AudioSource.PlayClipAtPoint(footstepAudioClips[index], transform.TransformPoint(_playerController.characterController.center),
            footstepAudioVolume);
    }

    // ReSharper disable once UnusedMember.Local
    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(landingAudioClip, transform.TransformPoint(_playerController.characterController.center),
                footstepAudioVolume);
        }
    }
    
    private void FootR(AnimationEvent animationEvent){}
    private void FootL(AnimationEvent animationEvent){}
}