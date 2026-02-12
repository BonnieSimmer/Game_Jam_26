using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyPad : Interactable
{
    [Header("Nightmare Settings")]
    public string mazeSceneName = "Nour_work/Scenes/Scene_Dream";
    
    [Header("Difficulty Config")]
    public int mazeSize = 21;        
    public int enemyCount = 4;
    public float timeLimit = 120f; 

    public void GoToSleep()
    {
        if (NightmareManager.Instance)
        {
            NightmareManager.Instance.SetLevelData(mazeSize,enemyCount, timeLimit, SceneManager.GetActiveScene().name);
        }
        
        SceneManager.LoadScene(mazeSceneName);
    }

    protected override void Interact()
    {
        GoToSleep();
    }
    
    // [SerializeField] private GameObject _door;
    // private Animator _doorAnimator;
    // private bool _doorOpen;
    //
    // void Start()
    // {
    //     if (_door != null)
    //     {
    //         _doorAnimator = _door.GetComponent<Animator>();
    //     }
    //     else
    //     {
    //         Debug.LogWarning($"Door reference missing on {gameObject.name}!");
    //     }
    // }
    //
    // protected override void Interact()
    // {
    //     if (!_doorAnimator) return;
    //
    //     _doorOpen = !_doorOpen;
    //     _doorAnimator.SetBool("Open", _doorOpen);
    //     promptMessage = _doorOpen ? "Press to Close Door" : "Press to Open Door";
    // }
}