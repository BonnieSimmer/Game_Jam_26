using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    private InputManager _input;
    
    public static bool IsPaused;

    void Start()
    {
        _input = Object.FindFirstObjectByType<InputManager>();
        _input.OnPause += TogglePause;
        Resume();
    }

    public void TogglePause()
    {
        if (IsPaused) Resume(); else Pause();
    }

    public void Resume()
    {
        IsPaused = false;
        menuPanel.SetActive(false);
        Time.timeScale = 1f;
        SetCursorState(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Nour_work/Scenes/Scene_MainMenu");
    }

    private void Pause()
    {
        IsPaused = true;
        menuPanel.SetActive(true);
        Time.timeScale = 0f;
        SetCursorState(true);
    }

    private void SetCursorState(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && !IsPaused) SetCursorState(false);
    }
}