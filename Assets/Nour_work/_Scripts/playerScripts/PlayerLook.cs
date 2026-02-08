using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    private InputManager _input;
    private float _xRotation = 0f;
    
    public Camera playerCamera;
    public float xSensitivity = 30f, ySensitivity = 30f;

    void Start() => _input = GetComponent<InputManager>();

    void LateUpdate()
    {
        if (Time.timeScale == 0 || !playerCamera) return;

        Vector2 lookInput = _input.GetLook();
        float mouseX = lookInput.x * xSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * ySensitivity * Time.deltaTime;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -80f, 80f);

        playerCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
    }
}