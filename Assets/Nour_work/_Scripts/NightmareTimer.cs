using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 

public class NightmareTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText; 
    [SerializeField]
    private float timeLeft = 120;
    private bool _isRunning = false;
    private string _returnToScene = "";

    void Start()
    {
        if (NightmareManager.Instance)
        {
            timeLeft = NightmareManager.Instance.timeLimitInSeconds;
            _returnToScene = NightmareManager.Instance.returnToSceneName;
        }
        _isRunning = true;
    }

    void Update()
    {
        if (!_isRunning) return;

        timeLeft -= Time.deltaTime;
        
        if (timeLeft < 0)
        {
            LevelComplete(false); 
            return;
        }
        
        if (timerText)
        {
            int minutes = Mathf.FloorToInt(timeLeft / 60F);
            int seconds = Mathf.FloorToInt(timeLeft - minutes * 60);
            timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
        }
    }

    public void LevelComplete(bool won)
    {
        _isRunning = false;

        if (won)
        {
            int currentDay = PlayerPrefs.GetInt("DayNumber", 1);

            if (currentDay == 1)
            {
                PlayerPrefs.SetInt("Trait1", 1);
                Debug.Log("Trait 1 Unlocked!");
            }
            else if (currentDay == 2)
            {
                PlayerPrefs.SetInt("Trait2", 1);
                Debug.Log("Trait 2 Unlocked!");
            }
            else if (currentDay == 3)
            {
                PlayerPrefs.SetInt("Trait3", 1);
                Debug.Log("Trait 3 Unlocked!");
            }

            PlayerPrefs.Save();

            if (SimpleFader.Instance) SimpleFader.Instance.FadeOutAndIn(Color.white ,ReturnToMain);
        }
        else
        {
            if (SimpleFader.Instance) SimpleFader.Instance.FadeOutAndIn(ReturnToMain);
        }

        Invoke(nameof(ReturnToMain), 3f);
    }

    void ReturnToMain()
    {
        int currentDay = PlayerPrefs.GetInt("DayNumber", 1);

        if (currentDay >= 3)
        {
            SceneManager.LoadScene("EndingScene"); 
        }
        else
        {
            if (_returnToScene.Equals("")) return;
            PlayerPrefs.SetInt("DayNumber", currentDay + 1);
            
            GameDataHandler.SaveProgress(_returnToScene);
            SceneManager.LoadScene(_returnToScene);
        }
        
    }
}