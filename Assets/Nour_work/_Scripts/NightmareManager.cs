using UnityEngine;

public class NightmareManager : MonoBehaviour
{
    public static NightmareManager Instance;

    public int mazeSize = 21;   // Must be odd (if you forget I have a fail safe no worry)  
    public int enemyCount = 3;      
    public float timeLimitInSeconds = 120f;   
    public string returnToSceneName = "Nour_work/Scenes/Scene_Level_01";

    public bool playerGotTrait = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Pass from daytime to this function
    public void SetLevelData(int size, int enemies, float time, string sceneName)
    {
        mazeSize = size;
        enemyCount = enemies;
        timeLimitInSeconds = time;
        returnToSceneName = sceneName;
    }
}