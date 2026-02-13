using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class EnemyProximitySensor : MonoBehaviour
{
   [Header("Volume Settings")]
    public Volume globalVolume;              
    public float maxIntensity = 0.8f;       
    
    [Header("Detection Settings")]
    public float maxDistance = 20.0f;       
    public float minDistance = 3.0f;       

    [Header("Pulse Effect (Optional)")]
    public bool usePulse = true;
    public float pulseSpeed = 2.0f;
    
    [Header("Whisper Effect (Optional)")]
    public AudioSource whisperSound;
    public float maxWhisperVolume = 0.8f;

    private Vignette _vignette;

    void Start()
    {
        if (globalVolume.profile.TryGet(out Vignette v))
        {
            _vignette = v;
        }
        whisperSound = GetComponent<AudioSource>();
        if (whisperSound) whisperSound.volume = 0f;;
    }

    void Update()
    {
        if (!_vignette) return;

        float closestDistance = Mathf.Infinity;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Entity");

        foreach (GameObject enemy in enemies)
        {
            float d = Vector3.Distance(transform.position, enemy.transform.position);
            if (d < closestDistance) closestDistance = d;
        }

        // 0 = Far, 0.5 = Close
        float factor = 0f;
        if (closestDistance < maxDistance)
        {
            factor = 1.0f - Mathf.InverseLerp(minDistance, maxDistance, closestDistance);
        }

        float finalIntensity = factor * maxIntensity;
        float whisperVolume = factor * maxWhisperVolume; 
        
        if (usePulse && factor > 0.1f)
        {
            float currentPulse = Mathf.Sin(Time.time * pulseSpeed * (1 + factor * 2)); 
            finalIntensity += currentPulse * 0.05f; 
        }

        _vignette.intensity.value = Mathf.Clamp(finalIntensity, 0f, 0.5f);
        whisperSound.volume = Mathf.Clamp(whisperVolume, 0f, 0.2f);
    }
    
    public void ResetAndDisable()
    {
        if (_vignette) _vignette.intensity.value = 0f;
        if (whisperSound) whisperSound.volume = 0f;
        this.enabled = false; 
    }

    public void EnableSensor()
    {
        this.enabled = true;
    }
}