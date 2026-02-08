using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    private float _xRotation = 0f;
    
    public new Camera camera;
    public float xSensitivity = 0.5f;
    public float ySensitivity = 0.5f;

    public void ProcessLook(Vector2 input)
    {
        float mouseX = input.x * xSensitivity;
        float mouseY = input.y * ySensitivity;
    
        // Calculate camera rotation for looking up and down
        _xRotation -= mouseY; 
        _xRotation = Mathf.Clamp(_xRotation, -80f, 80f);
    
        // Apply rotation to camera
        camera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
    
        // Rotate player body horizontally
        transform.Rotate(Vector3.up * mouseX);
    }
}
