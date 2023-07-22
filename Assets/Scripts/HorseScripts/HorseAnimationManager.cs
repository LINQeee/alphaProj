using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HorseAnimationManager : MonoBehaviour
{
    private Animator _animator;
    private static readonly int AnimIdRotationInt = Animator.StringToHash("RotationInt");
    private static readonly int AnimIdMovementInt = Animator.StringToHash("MovementInt");
    private static readonly int AnimIdIsSprinting = Animator.StringToHash("IsSprinting");
    private static readonly int AnimIdIsFalling = Animator.StringToHash("IsFalling");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void AnimateJumpingRelativeToSpeed(bool isSprinting)
    {
        _animator.CrossFade(isSprinting ? "SprintJump" : "Jump", 0.2f);
    }

    public void SetMovementAxis(float rotationInput, float movementInput)
    {
        SetRotationInt((int)Mathf.Round(rotationInput));
        SetMovementInt((int)Mathf.Round(movementInput));
    }

    public void SetIsSprintingRelativeToController(bool isControllerEnabled, StaminaProfile staminaProfile)
    {
            SetIsSprinting(isControllerEnabled && staminaProfile.isCanRun);
    }

    public void AnimateFallingIfNotJumping(bool isJumping, bool isGrounded)
    {
        if (isJumping) return;

        switch (isGrounded)
        {
            case false when !GetIsFalling():
                SetIsFalling(true);
                _animator.CrossFade("StartFalling", 0.2f);
                break;
            case true:
                SetIsFalling(false);
                break;
        }
    }

    private void SetRotationInt(int value) => _animator.SetInteger(AnimIdRotationInt, value);

    private void SetMovementInt(int value) => _animator.SetInteger(AnimIdMovementInt, value);

    private void SetIsSprinting(bool value) => _animator.SetBool(AnimIdIsSprinting, value);

    private void SetIsFalling(bool value) => _animator.SetBool(AnimIdIsFalling, value);

    private bool GetIsFalling() => _animator.GetBool(AnimIdIsFalling);
}