using UnityEngine;

public class CursorController : MonoBehaviour
{
    private OtherInput _input;
    private OtherInput.SettingsActions _settingsActions;
    
    private void Awake()
    {
        _input = new OtherInput();
        _settingsActions = _input.Settings;
        
        // Lock the cursor to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.Locked;

        // Make the cursor invisible
        Cursor.visible = false;
        
        _settingsActions.Pause.performed += ctx => GetCursor(); 
    }
    
    public void GetCursor()
    {
        // Unlock and show the cursor when the Escape key is pressed
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}