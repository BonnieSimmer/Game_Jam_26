using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    private float _xRotation = 0f;
    private float _lookWaitTimer = 0.1f; // Wait 0.1 seconds for the mouse to settle
    
    public new Camera camera;
    public float xSensitivity = 0.5f;
    public float ySensitivity = 0.5f;

    public void ProcessLook(Vector2 input)
    {
        if (_lookWaitTimer > 0)
        {
            _lookWaitTimer -= Time.deltaTime;
            return;
        }

        float mouseX = input.x * xSensitivity;
        float mouseY = input.y * ySensitivity;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -80f, 80f);

        camera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}