using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Scenes/Scene_MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
