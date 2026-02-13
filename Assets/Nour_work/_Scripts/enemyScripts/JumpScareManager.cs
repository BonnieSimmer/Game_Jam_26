using System.Collections;
using Unity.Cinemachine; 
using UnityEngine;

public class JumpScareManager : MonoBehaviour
{
    public static JumpScareManager Instance;
    
    [Header("Settings")]
    public float scareDuration = 4.0f;
    
    [Header("Shake Settings")]
    [Range(0.1f, 5f)] public float shakeIntensity = 0.5f; 
    [Range(0.1f, 2f)] public float shakeRotation = 0.5f; 
    
    private bool _isScaring = false;

    void Awake() => Instance = this;

    public void TriggerScare(GameObject enemyScareCam, AudioSource scareSound, System.Action onFinished)
    {
        if (_isScaring) return;
        StartCoroutine(ScareRoutine(enemyScareCam, scareSound, onFinished));
    }

    private IEnumerator ScareRoutine(GameObject scareCam, AudioSource scareSound, System.Action onFinished)
    {
        _isScaring = true; 
        
        var proximitySensor = FindFirstObjectByType<EnemyProximitySensor>();
        if (proximitySensor) proximitySensor.ResetAndDisable();

        Camera mainCam = Camera.main; 
        int originalMask = 0;
        if (mainCam)
        {
            originalMask = mainCam.cullingMask;
            int playerLayerIndex = LayerMask.NameToLayer("Player");
            if (playerLayerIndex != -1) mainCam.cullingMask &= ~(1 << playerLayerIndex);
        }
        
        if (scareCam) scareCam.SetActive(true);
        if(scareSound) scareSound.Play();

        Coroutine shakeCoroutine = StartCoroutine(ManualShake(scareCam.transform, scareDuration, shakeIntensity, shakeRotation));

        yield return new WaitForSeconds(scareDuration); 
        
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);

        if (SimpleFader.Instance)
        {
            SimpleFader.Instance.FadeOutAndIn(() => 
            {
                CleanupAndFinish(scareCam, mainCam, originalMask, proximitySensor, onFinished);
            });
        }
        else
        {
            CleanupAndFinish(scareCam, mainCam, originalMask, proximitySensor, onFinished);
        }
    }

    private IEnumerator ManualShake(Transform camTransform, float duration, float intensity, float rotIntensity)
    {
        Vector3 originalPos = camTransform.localPosition;
        Quaternion originalRot = camTransform.localRotation;
        
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;
            float z = Random.Range(-1f, 1f) * intensity;

            float rotX = Random.Range(-1f, 1f) * rotIntensity;
            float rotY = Random.Range(-1f, 1f) * rotIntensity;
            float rotZ = Random.Range(-1f, 1f) * rotIntensity;

            camTransform.localPosition = originalPos + new Vector3(x, y, z);
            camTransform.localRotation = originalRot * Quaternion.Euler(rotX, rotY, rotZ);

            elapsed += Time.deltaTime;
            yield return null; 
        }

        camTransform.localPosition = originalPos;
        camTransform.localRotation = originalRot;
    }

    private void CleanupAndFinish(GameObject scareCam, Camera mainCam, int mask, EnemyProximitySensor sensor, System.Action onFinished)
    {
        MazeGenerator generator = UnityEngine.Object.FindFirstObjectByType<MazeGenerator>();
        if (generator) generator.ResetGamePositions();

        if (scareCam) scareCam.SetActive(false);
        if (mainCam) mainCam.cullingMask = mask;
        
        if (sensor) sensor.EnableSensor();
        
        onFinished?.Invoke();
        _isScaring = false;
    }
}