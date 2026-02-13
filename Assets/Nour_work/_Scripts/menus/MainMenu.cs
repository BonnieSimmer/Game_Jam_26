using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Shihab_Work/Scenes/IndoorsScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
