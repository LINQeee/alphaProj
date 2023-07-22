using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class InputManager : MonoBehaviour
{
    private PlayerController _playerController;
    private PlayerControls _playerControls;
    public Vector2 move;
    public Vector2 look;
    public bool sprintInput;
    public bool jumpInput;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        if (_playerControls == null)
        {
            _playerControls = new PlayerControls();

            _playerControls.Player.Move.performed += i =>
            {
                move = i.ReadValue<Vector2>();
            };
            _playerControls.Player.Look.performed += i =>
            {
                look = i.ReadValue<Vector2>();
            };

            _playerControls.Player.Sprint.performed += _ => sprintInput = true;
            _playerControls.Player.Sprint.canceled += _ => sprintInput = false;

            _playerControls.Player.Jump.performed += _ => jumpInput = true;
            _playerControls.Player.Jump.canceled += _ => jumpInput = false;

            _playerControls.Player.HorseMount.performed += _ => _playerController.riderManager.MountOrDismount();

            _playerControls.Player.Aim.performed += _ => _playerController.weaponManager.StartAiming();
            _playerControls.Player.Aim.canceled += _ => _playerController.weaponManager.StopAiming();

            _playerControls.Weapons.FirstWeaponSlot.performed += _ => _playerController.weaponManager.EquipOrUnEquipWeapon(0);
        }

        _playerControls.Enable();
    }

    private void OnDisable()
    {
        _playerControls.Disable();
    }
}