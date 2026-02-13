using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;

public class PlayerAbilities : MonoBehaviour
{
    private ThirdPersonController _controller;
    private StarterAssetsInputs _input;
    private NavMeshPath _navPath;
    private LineRenderer _pathLine;
    private PlayerLogic _playerLogic;


    private float _defaultMoveSpeed;
    private float _defaultSprintSpeed;
    private bool _isPathActive;
    
    private float _freezeCooldownTimer = 0f;
    private float _pathCooldownTimer = 0f;
    private float _currentPathDuration = 0f; 
    
    [Header("Player light level Stats")]
    public int lightHeartLevel = 20;
    
    [Header("UI")]
    public Image freezeCooldownFill; 
    public Image pathCooldownFill;

    [Header("Ability 1: Path of Light")]
    public Transform goalPosition;
    public float lightSpeedPenalty = 2.0f;
    public float maxPathBrightness = 2.0f;
    public float minPathBrightness = 0.2f;
    public float maxDist = 50f;
    
    [Header("Path Cooldowns")]
    public float pathMaxDuration = 5.0f;
    public float pathCooldown;   
    
    [Header("Visuals")]
    public float glowIntensity = 2.0f; 

    [Header("Ability 2: Enemy Freeze")]
    public float freezeDuration = 5.0f;
    public AudioClip freezeSound;
    
    [Header("Freeze Cooldowns")]
    public float freezeCooldown; 

    void Start()
    {
        _controller = GetComponent<ThirdPersonController>();
        _input = GetComponent<StarterAssetsInputs>(); 
        _navPath = new NavMeshPath();
        _pathLine = GetComponentInChildren<LineRenderer>();        
        
        if (_pathLine)
        {
            _pathLine.positionCount = 0;
            _pathLine.enabled = false; 
        }

        if (_controller)
        {
            _defaultMoveSpeed = _controller.MoveSpeed;
            _defaultSprintSpeed = _controller.SprintSpeed;
        }

        if (NightmareManager.Instance)
        {
            freezeCooldown = NightmareManager.Instance.timeLimitInSeconds / 4;
            pathCooldown = NightmareManager.Instance.timeLimitInSeconds / 20;
        }
        
        GameObject goalObj = GameObject.FindGameObjectWithTag("Finish");
        if (goalObj) goalPosition = goalObj.transform;
        
        _isPathActive = false;
        if (freezeCooldownFill) freezeCooldownFill.fillAmount = 0;
        if (pathCooldownFill) pathCooldownFill.fillAmount = 0;
        
        lightHeartLevel = PlayerPrefs.GetInt("lightHeartLevel", 20);
        glowIntensity = lightHeartLevel / 100f;
    }

    void Update()
    {
        if (!_input) return;

        if (PauseMenu.IsPaused) return;
        
        HandleCooldowns();
        HandleLightPath();
        HandleFreeze();
        UpdateUI();
    }

    void HandleCooldowns()
    {
        if (_freezeCooldownTimer > 0)
        {
            _freezeCooldownTimer -= Time.deltaTime;
        }

        if (_pathCooldownTimer > 0)
        {
            _pathCooldownTimer -= Time.deltaTime;
        }
    }

    void HandleLightPath()
    {
        if (!goalPosition)
        {
            GameObject goalObj = GameObject.FindGameObjectWithTag("Finish");
            if (goalObj) goalPosition = goalObj.transform;
        }

        if (_input.path && _pathCooldownTimer <= 0) 
        {
            if (!_isPathActive) ActivatePath();

            _currentPathDuration += Time.deltaTime;

            if (_currentPathDuration >= pathMaxDuration)
            {
                DeactivatePath();
            }
            else
            {
                UpdatePathVisuals();
            }
        }
        else
        {
            if (_isPathActive) 
            {
                DeactivatePath();
            }
        }
    }

    void HandleFreeze()
    {
        if (_input.freezeTriggered)
        {
            _input.ConsumeFreezeInput();

            if (_freezeCooldownTimer <= 0)
            {
                StartCoroutine(FreezeRoutine());
                _freezeCooldownTimer = freezeCooldown;
            }
        }
    }
    
    void ActivatePath()
    {
        _isPathActive = true;
        if (_pathLine) _pathLine.enabled = true;

        if (_controller)
        {
            _controller.MoveSpeed = Mathf.Max(1f, _defaultMoveSpeed - lightSpeedPenalty);
            _controller.SprintSpeed = _controller.MoveSpeed; 
        }
    }

    void DeactivatePath()
    {
        if (_isPathActive)
        {
            _pathCooldownTimer = pathCooldown;
            _currentPathDuration = 0;         
        }

        _isPathActive = false;
        if (_pathLine) _pathLine.enabled = false;

        if (_controller)
        {
            _controller.MoveSpeed = _defaultMoveSpeed;
            _controller.SprintSpeed = _defaultSprintSpeed;
        }
    }

    void UpdatePathVisuals()
    {
        if (!goalPosition || !_pathLine) return;

        if (NavMesh.CalculatePath(transform.position, goalPosition.position, NavMesh.AllAreas, _navPath))
        {
            if (_navPath.status == NavMeshPathStatus.PathComplete)
            {
                _pathLine.positionCount = _navPath.corners.Length;
                
                Vector3[] liftedCorners = new Vector3[_navPath.corners.Length];
                for (int i = 0; i < _navPath.corners.Length; i++)
                {
                    liftedCorners[i] = _navPath.corners[i] + Vector3.up * 0.1f; 
                }
                _pathLine.SetPositions(liftedCorners);

                float distToGoal = Vector3.Distance(transform.position, goalPosition.position);
                float fadeFactor = Mathf.Lerp(maxPathBrightness, minPathBrightness, distToGoal / maxDist);
                
                Color brightColor = Color.white * fadeFactor * glowIntensity;
                brightColor.a = 1.0f; 

                _pathLine.startColor = brightColor;
                _pathLine.endColor = brightColor;
                
                if (_pathLine.material) 
                {
                    _pathLine.material.color = Color.white; 
                }
            }
        }
    }

    IEnumerator FreezeRoutine()
    {
        if (freezeSound) AudioSource.PlayClipAtPoint(freezeSound, transform.position);

        var enemies = FindObjectsByType<NavMeshAgent>(FindObjectsSortMode.None);
        List<float> originalSpeeds = new List<float>();

        // STOP THEM
        foreach (var agent in enemies)
        {
            originalSpeeds.Add(agent.speed);
            agent.speed = 0; 
            agent.isStopped = true;
            
            Animator anim = agent.GetComponentInChildren<Animator>();
            if (anim) anim.speed = 0; 
        }

        yield return new WaitForSeconds(freezeDuration);

        int i = 0;
        foreach (var agent in enemies)
        {
            if (agent) 
            {
                agent.speed = originalSpeeds[i];
                agent.isStopped = false;
                
                Animator anim = agent.GetComponentInChildren<Animator>();
                if (anim) anim.speed = 1;
            }
            i++;
        }
    }
    
    void UpdateUI()
    {
        if (freezeCooldownFill)
        {
            freezeCooldownFill.fillAmount = _freezeCooldownTimer / freezeCooldown;
        }

        if (pathCooldownFill)
        {
            if (_pathCooldownTimer > 0)
            {
                pathCooldownFill.fillAmount = _pathCooldownTimer / pathCooldown;
                pathCooldownFill.color = Color.darkRed;
            }
            else if (_isPathActive)
            {
                pathCooldownFill.fillAmount = _currentPathDuration / pathMaxDuration;
                pathCooldownFill.color = Color.darkGoldenRod;
            }
            else
            {
                pathCooldownFill.fillAmount = 0;
            }
        }
    }
}