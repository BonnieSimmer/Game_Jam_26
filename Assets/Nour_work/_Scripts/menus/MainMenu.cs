using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Buttons")]
    public Button continueButton;
    public Button newGameButton;

    [Header("Defaults")]
    public string firstLevelName = "Shihab_work/Scenes/IndoorsScene"; 

    void Start()
    {
        if (continueButton)
        {
            bool hasSave = GameDataHandler.HasSave();
            continueButton.interactable = hasSave;
            
            var colors = continueButton.colors;
            colors.disabledColor = new Color(1, 1, 1, 0.3f);
            continueButton.colors = colors;
        }
    }

    public void OnNewGameClicked()
    {
        GameDataHandler.DeleteSave();
        
        PlayerPrefs.SetInt("DayNumber", 1);
        PlayerPrefs.SetInt("lightHeartLevel", 20);
        PlayerPrefs.Save();

        SceneManager.LoadScene(firstLevelName);
    }

    public void OnContinueClicked()
    {
        if (GameDataHandler.HasSave())
        {
            string sceneToLoad = GameDataHandler.GetSavedScene();
            SceneManager.LoadScene(sceneToLoad);
        }
    }
    
    public void OnQuitClicked()
    {
        Application.Quit();
    }
}