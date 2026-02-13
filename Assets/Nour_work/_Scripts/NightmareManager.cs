using UnityEngine;

public class NightmareManager : MonoBehaviour
{
    public static NightmareManager Instance;

    [Header("Current Level Data")]
    public int mazeSize = 21;       
    public int enemyCount = 3;      
    public float timeLimitInSeconds = 120f;   
    public string returnToSceneName = "Shihab_Work/Scenes/IndoorsScene";
    
    [Header("Difficulty Scaling")]
    public int baseMazeSize = 9;
    public float baseTime = 60f;

    void Awake()
    {
        Instance = this;
        CalculateDifficulty(); 
    }
    
    public void CalculateDifficulty()
    {
        int dayNumber = PlayerPrefs.GetInt("DayNumber", 1);
        dayNumber = Mathf.Max(1, dayNumber);
        
        mazeSize = baseMazeSize + (dayNumber * 3);
        
        if (mazeSize % 2 == 0) mazeSize++; 

        enemyCount = dayNumber;

        timeLimitInSeconds = baseTime * dayNumber;
        
        Debug.Log($"Nightmare Generated for Day {dayNumber}: Size {mazeSize}, Enemies {enemyCount}, Time {timeLimitInSeconds}s");
    }
}