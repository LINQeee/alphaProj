using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

[RequireComponent(typeof(PlayerController)),
 RequireComponent(typeof(NavMeshAgent)),
 RequireComponent(typeof(AnimationManager)),
 RequireComponent(typeof(RigBuilder)),
 RequireComponent(typeof(CameraManager)),
 RequireComponent(typeof(StaminaProfile)),
 RequireComponent(typeof(PlayerLocomotion))
]
public class RiderManager : MonoBehaviour
{
    [NonSerialized] public HorseController CurrentHorse;
    [NonSerialized] public AnimationManager AnimationManager;
    public bool isCanMount { private get; set; }
    public bool isMounted { get; private set; }
    
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private Transform rightHandTarget;
    [SerializeField] private UIManager uiManager;
    
    private PlayerController _playerController;
    private NavMeshAgent _agent;
    private CameraManager _cameraManager;
    private bool _isMountingOrDismounting;
    private CharacterController _characterController;
    private RigBuilder _rigBuilder;
    private StaminaProfile _staminaProfile;
    private PlayerLocomotion _playerLocomotion;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _agent = GetComponent<NavMeshAgent>();
        AnimationManager = GetComponent<AnimationManager>();
        _cameraManager = GetComponent<CameraManager>();
        _characterController = GetComponent<CharacterController>();
        _rigBuilder = GetComponent<RigBuilder>();
        _staminaProfile = GetComponent<StaminaProfile>();
        _playerLocomotion = GetComponent<PlayerLocomotion>();
        _rigBuilder.enabled = false;
    }

    public void MountOrDismount()
    {
        if (_isMountingOrDismounting || _playerLocomotion.IsAiming) return;
        StartCoroutine(!isMounted ? Mount() : Dismount());
    }

    private Vector3 DetectNearestMountPlace(HorseInteractionTransforms interactionTransforms)
    {
        Vector3 leftMountPlace = interactionTransforms.leftMountPlace.position;
        Vector3 rightMountPlace = interactionTransforms.rightMountPlace.position;
        float distanceToLeftPlace = Vector3.Distance(transform.position, leftMountPlace);
        float distanceToRightPlace = Vector3.Distance(transform.position, rightMountPlace);
        return distanceToLeftPlace < distanceToRightPlace ? leftMountPlace : rightMountPlace;
    }

    private IEnumerator Mount()
    {
        if (!isCanMount) yield break;
        _isMountingOrDismounting = true;
        isMounted = true;
        DisablePlayerController();
        GoToNearestMountPlace();
        yield return uiManager.ShowAbility(uiManager.horseMountAbility);
        MovePlayerToRiderPlace();
        EnableHorseController();
        _isMountingOrDismounting = false;
    }

    private void GoToNearestMountPlace()
    {
        Vector3 nearestPlace = DetectNearestMountPlace(CurrentHorse.interactionTransforms);
        if (!IsNearPosition(nearestPlace)) StartCoroutine(MoveAgentToPos(nearestPlace));
    }

    private IEnumerator MoveAgentToPos(Vector3 position)
    {
        _agent.enabled = true;
        _agent.SetDestination(position);
        AnimationManager.SetSpeed(2);
        yield return new WaitUntil(() => IsNearPosition(position));
        AnimationManager.SetSpeed(0);
        _agent.enabled = false;
    }

    private bool IsNearPosition(Vector3 position) => Vector3.Distance(transform.position, position) < 0.1f;

    private void DisablePlayerController()
    {
        _playerController.isEnabledController = false;
        _characterController.enabled = false;
        AnimationManager.SetSpeed(0);
        _staminaProfile.isProfileEnabled = false;
    }

    private void EnableHorseController()
    {
        var horseStaminaProfile = CurrentHorse.GetComponent<StaminaProfile>();
        horseStaminaProfile.ResetProfile();
        horseStaminaProfile.isProfileEnabled = true;
        horseStaminaProfile.inputManager = GetComponent<InputManager>();
        CurrentHorse.CurrentRiderManager = this;
        CurrentHorse.leftHandPair.child = leftHandTarget;
        CurrentHorse.rightHandPair.child = rightHandTarget;
        
        CurrentHorse.IsEnabledController = true;
    }

    private void DisableHorseController()
    {
        CurrentHorse.IsEnabledController = false;
        CurrentHorse.HorseLocomotion.ResetInput();
        CurrentHorse.GetComponent<StaminaProfile>().isProfileEnabled = false;
    }

    private void MovePlayerToRiderPlace()
    {
        transform.SetParent(CurrentHorse.interactionTransforms.riderPlace);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
        AnimationManager.PlayMount();
        _cameraManager.SwapTpCameraTarget(CurrentHorse.cameraFollow);
        _rigBuilder.enabled = true;
    }

    private void ResetPlayerPosition()
    {
        Transform rightMountPlace = CurrentHorse.interactionTransforms.rightMountPlace;
        transform.SetPositionAndRotation(rightMountPlace.position, rightMountPlace.rotation);
        transform.SetParent(null);
        AnimationManager.SetIsMounted(false);
        _rigBuilder.enabled = false;
        _cameraManager.ResetTpCameraTarget();
    }

    private void EnablePlayerController()
    {
        _playerController.isEnabledController = true;
        _characterController.enabled = true;
        _staminaProfile.isProfileEnabled = true;
    }

    private IEnumerator Dismount()
    {
        _isMountingOrDismounting = true;
        isMounted = false;
        AnimationManager.SetIsHorseWalking(false);
        DisableHorseController();
        yield return uiManager.ShowAbility(uiManager.horseDismountAbility);
        ResetPlayerPosition();
        EnablePlayerController();
        _isMountingOrDismounting = false;
    }
}