using UnityEngine;

public class InputManager : MonoBehaviour
{
    private PlayerInput _input;
    private PlayerMotor _motor;
    private PlayerLook _look;
    
    private Vector2 _moveInput;
    private Vector2 _lookInput;
    
    public PlayerInput.OnFootActions OnFootActions;


    private void Awake()
    {
        _input = new PlayerInput();
        OnFootActions = _input.OnFoot;
        _motor = GetComponent<PlayerMotor>();
        _look = GetComponent<PlayerLook>();
        
        ToggleCursor(false);

        // Event Subscription
        OnFootActions.Jump.performed += ctx => _motor.Jump();
        OnFootActions.Movement.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        OnFootActions.Movement.canceled += ctx => _moveInput = Vector2.zero;
        OnFootActions.Look.performed += ctx => _lookInput = ctx.ReadValue<Vector2>();
        OnFootActions.Crouch.performed += ctx => _motor.Crouch();
        OnFootActions.Sprint.performed += ctx => _motor.Sprint();
        _input.Settings.Pause.performed += ctx => ToggleCursor(true);
    }
    
    
    private void Update() 
    {
        _motor.ProcessMove(_moveInput);
        _look.ProcessLook(_lookInput);
        _lookInput = Vector2.zero; 
    }

    private void OnEnable() => OnFootActions.Enable();
    
    private void OnDisable() => OnFootActions.Disable();
    
    public void ToggleCursor(bool isPaused)
    {
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            ToggleCursor(false); 
        }
    }
}