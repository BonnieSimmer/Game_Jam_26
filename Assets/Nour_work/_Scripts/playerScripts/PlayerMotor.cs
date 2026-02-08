using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(InputManager))]
public class PlayerMotor : MonoBehaviour
{
    private CharacterController _controller;
    private InputManager _input;
    private Vector3 _playerVelocity;
    private bool _isGrounded;
    private bool _isCrouching, _lerpCrouch, _isSprinting;
    private float _crouchTimer;

    public float speed = 8f, gravity = -9.8f, jumpHeight = 3f;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<InputManager>();
        
        _input.OnJump += Jump;
        _input.OnCrouch += Crouch;
        _input.OnSprint += Sprint;
    }

    void Update()
    {
        _isGrounded = _controller.isGrounded;
        if (_isGrounded && _playerVelocity.y < 0) _playerVelocity.y = -2f;

        HandleCrouchLerp();
        
        // Process movement
        Vector3 moveInput = _input.GetMovement();
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        _controller.Move(speed * Time.deltaTime * move);

        // Apply gravity
        _playerVelocity.y += gravity * Time.deltaTime;
        _controller.Move(_playerVelocity * Time.deltaTime);
    }

    private void HandleCrouchLerp()
    {
        if (!_lerpCrouch) return;
        _crouchTimer += Time.deltaTime;
        float p = _crouchTimer / 1f;
        _controller.height = Mathf.Lerp(_controller.height, _isCrouching ? 1f : 2f, p * p);
        if (p > 1) _lerpCrouch = false;
    }

    public void Jump()
    {
        if (_isGrounded) _playerVelocity.y = Mathf.Sqrt(jumpHeight * -3f * gravity);
    }

    public void Crouch()
    {
        _isCrouching = !_isCrouching; _crouchTimer = 0; _lerpCrouch = true;
    }

    public void Sprint()
    {
        _isSprinting = !_isSprinting; speed = _isSprinting ? 16f : 8f;
    }
}