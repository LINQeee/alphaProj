using System;
using UnityEngine;

[RequireComponent(typeof(HorseAnimationManager)), RequireComponent(typeof(HorseLocomotion)), RequireComponent(typeof(StaminaProfile))]
public class HorseController : MonoBehaviour
{
    public HorseInteractionTransforms interactionTransforms;
    public Transform cameraFollow;
    public TransformPair leftHandPair;
    public TransformPair rightHandPair;
    [NonSerialized] public RiderManager CurrentRiderManager;
    public float maxStamina;
    [NonSerialized] public bool IsEnabledController;
    [NonSerialized] public HorseLocomotion HorseLocomotion;
    [NonSerialized] public HorseAnimationManager HorseAnimationManager;
    private StaminaProfile _staminaProfile;

    private void Awake()
    {
        HorseAnimationManager = GetComponent<HorseAnimationManager>();
        HorseLocomotion = GetComponent<HorseLocomotion>();
        _staminaProfile = GetComponent<StaminaProfile>();
    }

    private void Update()
    {
        AnimateHorse();
        if (!IsEnabledController) return;
        HorseLocomotion.CalcInputAndHandleJumping();
        UpdateHandsIK();
        AnimateRider();
    }

    private void UpdateHandsIK()
    {
        leftHandPair.SyncPair();
        rightHandPair.SyncPair();
    }

    private void FixedUpdate()
    {
        if (!IsEnabledController) return;
        HorseLocomotion.HandleMovement();
    }

    private void AnimateHorse()
    {
        HorseAnimationManager.SetMovementAxis(HorseLocomotion.HorseInput.x, HorseLocomotion.HorseInput.y);

        HorseAnimationManager.SetIsSprintingRelativeToController(IsEnabledController, _staminaProfile);

        HorseAnimationManager.AnimateFallingIfNotJumping(HorseLocomotion.IsJumping, HorseLocomotion.IsGrounded());
    }

    private void AnimateRider()
    {
        CurrentRiderManager.AnimationManager.SetIsHorseWalking(HorseLocomotion.HorseInput.y > 0);
        CurrentRiderManager.AnimationManager.SetIsHorseSprinting(_staminaProfile.IsCanRun);
    }
}