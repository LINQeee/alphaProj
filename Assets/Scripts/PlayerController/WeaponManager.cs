using System.Collections.Generic;
using UnityEngine;
using Utils;

[RequireComponent(typeof(PlayerController))]
public class WeaponManager : MonoBehaviour
{
    [SerializeField] private List<Weapon> playerWeapons;
    [SerializeField] private Transform leftHandTarget;
    public bool isAiming;

    private PlayerController _playerController;
    private bool _isWeaponEquip;
    private Weapon _currentWeapon;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    private void LateUpdate()
    {
        if (_isWeaponEquip) _currentWeapon.SyncWeaponIK(leftHandTarget);
    }
    
    public void EquipOrUnEquipWeapon(int weaponIndex)
    {
        if (_isWeaponEquip) UnEquipWeapon();
        else EquipWeapon(playerWeapons[weaponIndex]);
    }

    private void EquipWeapon(Weapon weapon)
    {
        weapon.gameObject.SetActive(true);
        _currentWeapon = weapon;
        _isWeaponEquip = true;
        _playerController.animationManager.ChangeAnimSetWithType(_currentWeapon.parentAnimType);
        _playerController.rigBuilder.SetLayerByIndex(0, true);
    }

    private void UnEquipWeapon()
    {
        if (isAiming) return;
        _currentWeapon.gameObject.SetActive(false);
        _isWeaponEquip = false;
        _currentWeapon = null;
        _playerController.rigBuilder.SetLayerByIndex(0, false);
        _playerController.animationManager.ChangeAnimSetWithType(AnimSet.AnimSetTypes.Unarmed);
    }
    
    public void StartAiming()
    {
        if (_playerController.riderManager.isMounted || _playerController.riderManager.isMountingOrDismounting) return;
        _playerController.cameraManager.SwitchToAimCam();
        isAiming = true;
        _playerController.staminaProfile.isProfileEnabled = false;
        EquipOrUnEquipWeapon(0);
        _playerController.animationManager.StartAnimateAiming();
    }
    
    public void StopAiming()
    {
        _playerController.cameraManager.SwitchToTpCam();
        isAiming = false;
        _playerController.staminaProfile.isProfileEnabled = true;
        _playerController.animationManager.StopAnimateAiming();
    }
}
