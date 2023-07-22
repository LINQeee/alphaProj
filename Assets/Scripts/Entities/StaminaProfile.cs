using System;
using System.Collections;
using UnityEngine;

public class StaminaProfile : MonoBehaviour
{
    public float maxStamina;
    [SerializeField] private ProgressBar progressBar;
    public bool isProfileEnabled { private get; set; }
    public InputManager inputManager { private get; set; }
    private float _currentStamina;
    [NonSerialized] public bool isCanRun;
    [NonSerialized] private bool _isSprinting;
    [NonSerialized] private bool _isStaminaReloading;

    private void Awake()
    {
        ResetProfile(false);
        if (!gameObject.CompareTag("Player")) return;

        isProfileEnabled = true;
        inputManager = GetComponent<InputManager>();
    }

    private void Update()
    {
        _isSprinting = isProfileEnabled && inputManager.sprintInput && inputManager.move != Vector2.zero;
        CalculateStamina();
    }

    private void LateUpdate()
    {
        try
        {
            progressBar.UpdateBarValue(_currentStamina / maxStamina);
        }
        catch (Exception)
        {
            // ignored
        } //it's normal
    }

    private void CalculateStamina()
    {
        if (_currentStamina < 0.1f && !_isStaminaReloading) StartCoroutine(ReloadStamina());
        else if (_isSprinting && _currentStamina > 0 && !_isStaminaReloading)
        {
            isCanRun = true;
            _currentStamina -= Time.deltaTime / maxStamina * 2;
        }
        else isCanRun = false;

        _currentStamina += _isStaminaReloading || _currentStamina >= maxStamina || _isSprinting
            ? 0
            : Time.deltaTime / maxStamina;
    }

    public void ResetProfile(bool isStaminaProfileEnabled = true)
    {
        _currentStamina = maxStamina;
        isCanRun = false;
        _isStaminaReloading = false;
        StopCoroutine(ReloadStamina());
        isProfileEnabled = isStaminaProfileEnabled;
    }

    public bool IsCanJump()
    {
        if (_currentStamina < Time.deltaTime / maxStamina * 10 || _isStaminaReloading) return false;
        _currentStamina -= Time.deltaTime / maxStamina * 50;
        _currentStamina = Mathf.Clamp(_currentStamina, 0, maxStamina);
        return true;
    }

    private IEnumerator ReloadStamina()
    {
        _isStaminaReloading = true;

        while (_currentStamina <= maxStamina)
        {
            _currentStamina += Time.deltaTime / maxStamina;
            yield return new WaitForEndOfFrame();
        }

        _currentStamina = Mathf.Clamp(_currentStamina, 0, maxStamina);
        _isStaminaReloading = false;
    }
}