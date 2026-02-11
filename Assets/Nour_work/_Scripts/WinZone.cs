using UnityEngine;

public class WinZone : MonoBehaviour
{
    private bool _triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (_triggered) return;

        if (other.CompareTag("Player"))
        {
            _triggered = true;
            Debug.Log("Player reached the center!");
            
            NightmareTimer timer = FindFirstObjectByType<NightmareTimer>();
            if (timer) timer.LevelComplete(true);
        }
    }
}