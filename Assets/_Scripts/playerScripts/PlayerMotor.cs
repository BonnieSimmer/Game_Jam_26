using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController _controller;
    private Vector3 _playerVelocity;
    private bool _isGrounded;
    private bool _isCrouching;
    private float _crouchTimer;
    private bool _lerpCrouch;
    private bool _isSprinting;
    
    public float speed = 8.0f;
    public float gravity = -9.8f;
    public float jumpHeight = 3.0f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        _isGrounded = _controller.isGrounded;
        if (_lerpCrouch)
        {
            _crouchTimer += Time.deltaTime;
            float p = _crouchTimer / 1;
            p *= p;
            if (_isCrouching)
            {
                _controller.height = Mathf.Lerp(_controller.height, 1, p);
            }
            else
            {
                _controller.height = Mathf.Lerp(_controller.height, 2, p);
            }
            
            if (p > 1)
            {
                _lerpCrouch = false;
                _crouchTimer = 0;
            }
        }
    }
    
    // Receive the inputs from the InputManager.cs and apply them to the player
    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        _controller.Move(speed * Time.deltaTime * transform.TransformDirection(moveDirection));
       
        _playerVelocity.y += gravity * Time.deltaTime;
        if (_isGrounded && _playerVelocity.y < 0) _playerVelocity.y = -2f;
        _controller.Move(_playerVelocity * Time.deltaTime);
    }
    
    public void Jump()
    {
        if (_isGrounded)
        {
            _playerVelocity.y = Mathf.Sqrt(jumpHeight * -3f * gravity);
            if (_isCrouching) Crouch();
        }
    }

    public void Crouch()
    {
        _isCrouching = !_isCrouching;
        _crouchTimer = 0;
        _lerpCrouch = true;
    }

    public void Sprint()
    {
        _isSprinting = !_isSprinting;
        speed = _isSprinting ? 16f : 8f;
    }
}
