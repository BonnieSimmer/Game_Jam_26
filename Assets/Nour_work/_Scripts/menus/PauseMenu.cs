using StarterAssets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    private StarterAssetsInputs _input;
    
    public static bool IsPaused = false;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            _input = player.GetComponent<StarterAssetsInputs>();
        }
        
        if (menuPanel) menuPanel.SetActive(false);
        Resume(); 
    }

    void Update()
    {
        if (!_input) return;

        if (_input.pause)
        {
            _input.pause = false; 
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (IsPaused) Resume(); 
        else Pause();
    }

    public void Resume()
    {
        IsPaused = false;
        if (menuPanel) menuPanel.SetActive(false);
        Time.timeScale = 1f;
        AudioListener.pause = false;
        
        SetCursorState(false); 
        
        if (_input) 
        {
            _input.cursorInputForLook = true;
            _input.look = Vector2.zero;
        }
    }

    private void Pause()
    {
        IsPaused = true;
        if (menuPanel) menuPanel.SetActive(true);
        Time.timeScale = 0f;
        AudioListener.pause = true;
        
        SetCursorState(true);
        
        if (_input) 
        {
            _input.cursorInputForLook = false;
            _input.look = Vector2.zero; 
        }
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; 
        IsPaused = false;
        SceneManager.LoadScene("Nour_work/Scenes/Scene_MainMenu");
    }

    private void SetCursorState(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }
}