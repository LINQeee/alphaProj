using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using Utils;

[RequireComponent(typeof(NavMeshAgent))]
public class RiderManager : MonoBehaviour
{
    [NonSerialized] public HorseController currentHorse;
    public bool isCanMount { private get; set; }
    public bool isMounted { get; private set; }
    public bool isMountingOrDismounting { get; private set; }
    
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private Transform rightHandTarget;
    [SerializeField] private UIManager uiManager;
    
    private PlayerController _playerController;
    private NavMeshAgent _agent;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _agent = GetComponent<NavMeshAgent>();
    }

    public void MountOrDismount()
    {
        if (isMountingOrDismounting || _playerController.weaponManager.isAiming) return;
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
        isMountingOrDismounting = true;
        isMounted = true;
        DisablePlayerController();
        GoToNearestMountPlace();
        yield return uiManager.ShowAbility(uiManager.horseMountAbility);
        MovePlayerToRiderPlace();
        EnableHorseController();
        isMountingOrDismounting = false;
    }

    private void GoToNearestMountPlace()
    {
        Vector3 nearestPlace = DetectNearestMountPlace(currentHorse.interactionTransforms);
        if (!IsNearPosition(nearestPlace)) StartCoroutine(MoveAgentToPos(nearestPlace));
    }

    private IEnumerator MoveAgentToPos(Vector3 position)
    {
        _agent.enabled = true;
        _agent.SetDestination(position);
        _playerController.animationManager.SetSpeed(2);
        yield return new WaitUntil(() => IsNearPosition(position));
        _playerController.animationManager.SetSpeed(0);
        _agent.enabled = false;
    }

    private bool IsNearPosition(Vector3 position) => Vector3.Distance(transform.position, position) < 0.1f;

    private void DisablePlayerController()
    {
        _playerController.isEnabledController = false;
        _playerController.characterController.enabled = false;
        _playerController.animationManager.SetSpeed(0);
        _playerController.staminaProfile.isProfileEnabled = false;
    }

    private void EnableHorseController()
    {
        var horseStaminaProfile = currentHorse.GetComponent<StaminaProfile>();
        horseStaminaProfile.ResetProfile();
        horseStaminaProfile.isProfileEnabled = true;
        horseStaminaProfile.inputManager = GetComponent<InputManager>();
        currentHorse.currentPlayerController = _playerController;
        currentHorse.leftHandPair.child = leftHandTarget;
        currentHorse.rightHandPair.child = rightHandTarget;
        
        currentHorse.IsEnabledController = true;
    }

    private void DisableHorseController()
    {
        currentHorse.IsEnabledController = false;
        currentHorse.HorseLocomotion.ResetInput();
        currentHorse.GetComponent<StaminaProfile>().isProfileEnabled = false;
    }

    private void MovePlayerToRiderPlace()
    {
        transform.SetParent(currentHorse.interactionTransforms.riderPlace);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
        _playerController.animationManager.AnimateMount();
        _playerController.cameraManager.SwapTpCameraTarget(currentHorse.cameraFollow);
        _playerController.rigBuilder.EnableLayers();
    }

    private void ResetPlayerPosition()
    {
        Transform rightMountPlace = currentHorse.interactionTransforms.rightMountPlace;
        transform.SetPositionAndRotation(rightMountPlace.position, rightMountPlace.rotation);
        transform.SetParent(null);
        _playerController.animationManager.SetIsMounted(false);
        _playerController.rigBuilder.DisableLayers();
        _playerController.cameraManager.ResetTpCameraTarget();
    }

    private void EnablePlayerController()
    {
        _playerController.isEnabledController = true;
        _playerController.characterController.enabled = true;
        _playerController.staminaProfile.isProfileEnabled = true;
    }

    private IEnumerator Dismount()
    {
        isMountingOrDismounting = true;
        isMounted = false;
        _playerController.animationManager.SetIsHorseWalking(false);
        DisableHorseController();
        yield return uiManager.ShowAbility(uiManager.horseDismountAbility);
        ResetPlayerPosition();
        EnablePlayerController();
        isMountingOrDismounting = false;
    }
}