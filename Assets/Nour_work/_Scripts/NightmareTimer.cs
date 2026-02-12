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
        
        if (NightmareManager.Instance) 
            NightmareManager.Instance.playerGotTrait = won;

        if (won)
        {
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
        if (_returnToScene.Equals("")) return;
        SceneManager.LoadScene(_returnToScene);
    }
}