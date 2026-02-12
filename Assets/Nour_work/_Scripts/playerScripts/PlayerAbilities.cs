using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;

public class PlayerAbilities : MonoBehaviour
{
    private ThirdPersonController _controller;
    private StarterAssetsInputs _input;
    private NavMeshPath _navPath;
    private LineRenderer _pathLine; 


    // State Variables
    private float _defaultMoveSpeed;
    private float _defaultSprintSpeed;
    private bool _isPathActive;
    
    [Header("Ability 1: Path of Light")]
    public Transform goalPosition;
    public float lightSpeedPenalty = 2.0f;
    public float maxPathBrightness = 2.0f;
    public float minPathBrightness = 0.2f;
    public float maxDist = 50.0f;
    public float lightIntensity = 1.0f;

    [Header("Ability 2: Enemy Freeze")]
    public int freezesRemaining = 3;
    public float freezeDuration = 3.0f;
    public AudioClip freezeSound;
    
    
    void Start()
    {
        _controller = GetComponent<ThirdPersonController>();
        _input = GetComponent<StarterAssetsInputs>(); 
        _navPath = new NavMeshPath();
        _pathLine = GetComponentInChildren<LineRenderer>();        
        _pathLine.positionCount = 0;
        _pathLine.enabled = false; 
        

        if (_controller)
        {
            _defaultMoveSpeed = _controller.MoveSpeed;
            _defaultSprintSpeed = _controller.SprintSpeed;
        }

        if (NightmareManager.Instance)
        {
            freezesRemaining = Mathf.FloorToInt(NightmareManager.Instance.timeLimitInSeconds/40);
        }

        GameObject goalObj = GameObject.FindGameObjectWithTag("Finish");
        if (goalObj) goalPosition = goalObj.transform;
        _isPathActive = false;
    }

    void Update()
    {
        if (!_input) return;

        HandleLightPath();
        HandleFreeze();
    }

    void HandleLightPath()
    {
        if (!goalPosition)
        {
            GameObject goalObj = GameObject.FindGameObjectWithTag("Finish");
            if (goalObj) goalPosition = goalObj.transform;
        }

        if (_input.path) 
        {
            if (!_isPathActive) ActivatePath();
            UpdatePathVisuals();
        }
        else
        {
            if (_isPathActive) DeactivatePath();
        }
    }
    void HandleFreeze()
    {
        if (_input.freezeTriggered)
        {
            _input.ConsumeFreezeInput();

            if (freezesRemaining > 0)
            {
                StartCoroutine(FreezeRoutine());
            }
        }
    }
    
    void ActivatePath()
    {
        _isPathActive = true;
        if (_pathLine)_pathLine.enabled = true;

        if (_controller)
        {
            _controller.MoveSpeed = Mathf.Max(1f, _defaultMoveSpeed - lightSpeedPenalty);
            _controller.SprintSpeed = _controller.MoveSpeed; 
        }
    }

    void DeactivatePath()
    {
        _isPathActive = false;
        if (_pathLine)_pathLine.enabled = false;

        if (_controller)
        {
            _controller.MoveSpeed = _defaultMoveSpeed;
            _controller.SprintSpeed = _defaultSprintSpeed;
        }
    }

 void UpdatePathVisuals()
    {
        if (goalPosition == null || _pathLine == null) return;

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
                
             
                Color brightColor = Color.white * fadeFactor * lightIntensity;
                
                brightColor.a = 1.0f; 

                _pathLine.startColor = brightColor;
                _pathLine.endColor = brightColor;
                
                if (_pathLine.material != null) 
                {
                    _pathLine.material.color = Color.white; 
                }
            }
        }
    }

    IEnumerator FreezeRoutine()
    {
        freezesRemaining--;
        
        // if (freezeSound) AudioSource.PlayClipAtPoint(freezeSound, transform.position);

        var enemies = FindObjectsByType<NavMeshAgent>(FindObjectsSortMode.None);
        List<float> originalSpeeds = new List<float>();

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
}