using UnityEngine;

public class InputManager : MonoBehaviour
{
    private PlayerInput _input;
    private PlayerInput.OnFootActions _onFootActions;
    private PlayerMotor _motor;
    private PlayerLook _look;
    
    private Vector2 _moveInput;
    private Vector2 _lookInput;

    private void Awake()
    {
        _input = new PlayerInput();
        _onFootActions = _input.OnFoot;
        _motor = GetComponent<PlayerMotor>();
        _look = GetComponent<PlayerLook>();

        // Event Subscription
        _onFootActions.Jump.performed += ctx => _motor.Jump();
        _onFootActions.Movement.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _onFootActions.Movement.canceled += ctx => _moveInput = Vector2.zero;
        _onFootActions.Look.performed += ctx => _lookInput = ctx.ReadValue<Vector2>();
        _onFootActions.Crouch.performed += ctx => _motor.Crouch();
        _onFootActions.Sprint.performed += ctx => _motor.Sprint();
    }
    
    private void FixedUpdate() => _motor.ProcessMove(_moveInput);
    
    private void LateUpdate() 
    {
        _look.ProcessLook(_lookInput);
        _lookInput = Vector2.zero; 
    }

    private void OnEnable() => _onFootActions.Enable();
    
    private void OnDisable() => _onFootActions.Disable();
}