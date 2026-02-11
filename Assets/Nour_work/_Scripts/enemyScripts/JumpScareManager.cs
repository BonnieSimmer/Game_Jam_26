using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class JumpScareManager : MonoBehaviour
{
    public static JumpScareManager Instance;
    public AudioSource scareSound;
    
    private bool _isScaring = false;

    void Awake() => Instance = this;

    public void TriggerScare(GameObject enemyScareCam, Animator enemyAnimator, System.Action onFinished)
    {
        if (_isScaring) return;
        StartCoroutine(ScareRoutine(enemyScareCam, enemyAnimator, onFinished));
    }

  private IEnumerator ScareRoutine(GameObject scareCam, Animator anim, System.Action onFinished)
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
        // if (anim && anim.gameObject.activeInHierarchy) anim.SetTrigger("JumpScare"); 
        
        if (scareCam)
        {
            var impulse = scareCam.GetComponent<CinemachineImpulseSource>();
            if (impulse) impulse.GenerateImpulseWithForce(1.5f);
        }
        if(scareSound) scareSound.Play();

        yield return new WaitForSeconds(1.5f);

        if (SimpleFader.Instance)
        {
            SimpleFader.Instance.FadeOutAndIn(() => 
            {
                MazeGenerator generator = UnityEngine.Object.FindFirstObjectByType<MazeGenerator>();
                if (generator) generator.ResetGamePositions();

                if (scareCam) scareCam.SetActive(false);
                if (mainCam) mainCam.cullingMask = originalMask;

                if (proximitySensor) proximitySensor.EnableSensor();

                onFinished?.Invoke();
                _isScaring = false;
            });
        }
        else
        {
            if (proximitySensor) proximitySensor.EnableSensor();
            onFinished?.Invoke();
            _isScaring = false;
        }
    }
}