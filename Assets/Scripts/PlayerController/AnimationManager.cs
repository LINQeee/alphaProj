using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class AnimationManager : MonoBehaviour
{
    [SerializeField] private List<AnimSet> playerAnimSets = new();
    private AnimSet _currentAnimSet;
    private Animator _animator;
    private PlayerController _playerController;

    #region Ids

    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
    private static readonly int IsMounted = Animator.StringToHash("IsMounted");
    private static readonly int IsHorseSprinting = Animator.StringToHash("IsHorseSprinting");
    private static readonly int IsHorseWalking = Animator.StringToHash("IsHorseWalking");
    private static readonly int AxisX = Animator.StringToHash("AxisX");
    private static readonly int AxisY = Animator.StringToHash("AxisY");
    private static readonly int IsAiming = Animator.StringToHash("IsAiming");
    private static readonly int CurrentAnimSetId = Animator.StringToHash("CurrentAnimSetId");

    #endregion

    private float _animAxisX;
    private float _animAxisY;
    public int axisLerpTime = 8;
   

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerController = GetComponent<PlayerController>();
        AnimSetService.AnimSetTypeUniqueCheck(playerAnimSets);
        ChangeAnimSetWithType(AnimSet.AnimSetTypes.Unarmed);
        AnimateBaseLayer();
    }

    public void ChangeAnimSetWithType(AnimSet.AnimSetTypes type)
    {
        _currentAnimSet = AnimSetService.GetAnimSetByType(playerAnimSets, type);
        SetCurrentAnimSetId((int)_currentAnimSet.animSetType);
        AnimateBaseLayer();
    }

    public void StartAnimateAiming()
    {
        SetIsAiming(true);
        AnimateAimLayer();
    }
    public void StopAnimateAiming()
    {
        SetIsAiming(false);
    }
    
    public void SetSpeed(float value) => _animator.SetFloat(Speed, value);
    public void SetGrounded(bool value) => _animator.SetBool(IsGrounded, value);
    public void SetIsMounted(bool value) => _animator.SetBool(IsMounted, value);
    public void SetIsHorseSprinting(bool value) => _animator.SetBool(IsHorseSprinting, value);
    public void SetIsHorseWalking(bool value) => _animator.SetBool(IsHorseWalking, value);
    private void SetIsAiming(bool value) => _animator.SetBool(IsAiming, value);
    private void SetCurrentAnimSetId(int value) => _animator.SetInteger(CurrentAnimSetId, value);
    
    #region animateActionsMethods
    public void AnimateJumping() => _animator.CrossFade(_currentAnimSet.jumpAnimName, 0.2f);
    public void AnimateFalling() => _animator.CrossFade(_currentAnimSet.inAirAnimName, 0.2f);
    public void AnimateMount()
    {
        SetIsMounted(true);
        _animator.CrossFade("Mount", 0.05f);
    }
    private void AnimateBaseLayer() => _animator.CrossFade(_currentAnimSet.baseLayerAnimName, 0.05f);
    private void AnimateAimLayer() => _animator.CrossFade(_currentAnimSet.aimLayerAnimName, 0.1f);
    #endregion

    public void SetAnimatorAxis()
    {
        SetAxis(_playerController.inputManager.move.y, ref _animAxisY, AxisY);
        SetAxis(_playerController.inputManager.move.x, ref _animAxisX, AxisX);
    }
    private void SetAxis(float value, ref float animAxis, int animId)
    {
        animAxis = Mathf.Lerp(animAxis, value, Time.deltaTime * axisLerpTime);
        _animator.SetFloat(animId, animAxis);
    }
}