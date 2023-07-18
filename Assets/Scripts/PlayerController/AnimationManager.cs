using System;
using UnityEngine;

[RequireComponent(typeof(InputManager))]
public class AnimationManager : MonoBehaviour
{
    private Animator _animator;
    private InputManager _inputManager;
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
    private static readonly int FreeFall = Animator.StringToHash("FreeFall");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int IsMounted = Animator.StringToHash("IsMounted");
    private static readonly int IsHorseSprinting = Animator.StringToHash("IsHorseSprinting");
    private static readonly int IsHorseWalking = Animator.StringToHash("IsHorseWalking");
    private static readonly int AxisX = Animator.StringToHash("AxisX");
    private static readonly int AxisY = Animator.StringToHash("AxisY");
    private float _animAxisX;
    private float _animAxisY;
    public int transitionSpeed;
    private static readonly int IsAiming = Animator.StringToHash("IsAiming");


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _inputManager = GetComponent<InputManager>();
    }

    public void SetSpeed(float value) => _animator.SetFloat(Speed, value);
    public void SetGrounded(bool value) => _animator.SetBool(IsGrounded, value);
    public void SetJump(bool value) => _animator.SetBool(Jump, value);
    public bool GetJump() => _animator.GetBool(Jump);
    public void SetIsMounted(bool value) => _animator.SetBool(IsMounted, value);
    public void SetIsHorseSprinting(bool value) => _animator.SetBool(IsHorseSprinting, value);
    public void SetIsHorseWalking(bool value) => _animator.SetBool(IsHorseWalking, value);
    public void SetIsAiming(bool value) => _animator.SetBool(IsAiming, value);

    public void PlayMount()
    {
        SetIsMounted(true);
        _animator.CrossFade("Mount", 0.05f);
    }

    public void PlayJump()
    {
        _animator.CrossFade("JumpStart", 0.2f);
    }

    public void SwapMovementAnimationToAiming()
    {
        SetIsAiming(true);
        _animator.CrossFade("BackIdleForward", 0.2f);
    }

    public void PlayInAir()
    {
        _animator.CrossFade("InAir", 0.2f);
    }

    public void SetAnimatorAxis()
    {
        SetAxis(_inputManager.move.y, ref _animAxisY, AxisY);
        SetAxis(_inputManager.move.x, ref _animAxisX, AxisX);
    }

    private void SetAxis(float value, ref float animAxis, int animId)
    {
        animAxis = Mathf.Lerp(animAxis, value, Time.deltaTime * transitionSpeed);
        _animator.SetFloat(animId, animAxis);
    }
}