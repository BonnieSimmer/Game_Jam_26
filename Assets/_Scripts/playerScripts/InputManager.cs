using UnityEngine;
using System;

public class InputManager : MonoBehaviour
{
    private PlayerInput _input;
    public PlayerInput.OnFootActions OnFoot { get; private set; }
    public PlayerInput.SettingsActions Settings { get; private set; }

    public event Action OnJump;
    public event Action OnCrouch;
    public event Action OnSprint;
    public event Action OnInteract;
    public event Action OnPause;

    private void Awake()
    {
        _input = new PlayerInput();
        OnFoot = _input.OnFoot;
        Settings = _input.Settings;

        OnFoot.Jump.performed += _ => OnJump?.Invoke();
        OnFoot.Crouch.performed += _ => OnCrouch?.Invoke();
        OnFoot.Sprint.performed += _ => OnSprint?.Invoke();
        OnFoot.Interact.performed += _ => OnInteract?.Invoke();
        Settings.Pause.performed += _ => OnPause?.Invoke();
    }

    private void OnEnable()
    {
        OnFoot.Enable();
        Settings.Enable();
    }

    private void OnDisable()
    {
        OnFoot.Disable();
        Settings.Disable();
    }

    public Vector2 GetMovement() => OnFoot.Movement.ReadValue<Vector2>();
    public Vector2 GetLook() => OnFoot.Look.ReadValue<Vector2>();
}