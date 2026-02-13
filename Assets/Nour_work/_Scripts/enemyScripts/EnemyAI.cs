using UnityEngine;
using UnityEngine.AI;
using Unity.Cinemachine;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Header("Targeting")]
    public Transform playerTarget;

    [Header("Ranges")]
    public float detectionRadius = 15f;
    public float escapeRadius = 15f;
    public float catchRadius = 1.5f; 

    [Header("Wander Settings")]
    public float wanderRadius = 8f;
    public float wanderTimer = 4f;

    [Header("Speeds")]
    public float wanderSpeed = 2.0f;
    public float chaseSpeed = 5.335f; 
    
    private NavMeshAgent _agent;
    private Animator _animator;
    private float _timer;
    private bool _isChasing = false;
    private bool _hasCaughtPlayer = false;

    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDMotionSpeed;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();

        AssignAnimationIDs();

        _timer = wanderTimer;
    }

    void Update()
    {
        UpdateAnimations();

        if (!playerTarget || _hasCaughtPlayer) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

        if (distanceToPlayer <= catchRadius)
        {
            TriggerCatchBehavior();
            return;
        }

        if (_isChasing)
        {
            if (distanceToPlayer > escapeRadius) StopChasing();
            else ChasePlayer();
        }
        else
        {
            if (distanceToPlayer < detectionRadius) StartChasing();
            else Wander();
        }
    }

    private void UpdateAnimations()
    {
        if (!_animator) return;
        
        float currentSpeed = _agent.velocity.magnitude;

        _animator.SetFloat(_animIDSpeed, currentSpeed);
        _animator.SetFloat(_animIDMotionSpeed, 1f);
        _animator.SetBool(_animIDGrounded, true); 
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    void TriggerCatchBehavior()
    {
        _hasCaughtPlayer = true;
        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;

        if (playerTarget)
        {
            var tpc = playerTarget.GetComponent<StarterAssets.ThirdPersonController>();
            if (tpc) tpc.enabled = false;

            var playerAnim = playerTarget.GetComponent<Animator>();
            if (playerAnim)
            {
                playerAnim.SetFloat("Speed", 0f);
                playerAnim.SetFloat("MotionSpeed", 0f);
            }
        }
        var camComponent = GetComponentInChildren<CinemachineCamera>(true); 
        var screamSound = GetComponent<AudioSource>();

        if (camComponent && JumpScareManager.Instance)
        {
            JumpScareManager.Instance.TriggerScare(camComponent.gameObject, screamSound, () =>
            {
                var mazeGen = FindFirstObjectByType<MazeGenerator>();
                if (mazeGen) mazeGen.RespawnPlayer();
            });
        }
        else
        {
            var mazeGen = FindFirstObjectByType<MazeGenerator>();
            if (mazeGen) mazeGen.RespawnPlayer();
        }
    }

    void StartChasing()
    {
        _isChasing = true;
        _agent.speed = chaseSpeed;
    }

    void ChasePlayer()
    {
        if (_agent.isOnNavMesh)
            _agent.SetDestination(playerTarget.position);
    }

    void StopChasing()
    {
        _isChasing = false;
        _agent.speed = wanderSpeed;
        _timer = wanderTimer;
    }

    void Wander()
    {
        _timer += Time.deltaTime;
        
        if ((_timer >= wanderTimer || _agent.remainingDistance < 0.5f) && !_agent.pathPending)
        {
            Vector3 newPos = GetRandomPoint(transform.position, wanderRadius);
            if(newPos != transform.position) 
            {
                _agent.SetDestination(newPos);
            }
            _timer = 0;
        }
    }

    Vector3 GetRandomPoint(Vector3 center, float range)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.onUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 5.0f, NavMesh.AllAreas))
            {
                if (Vector3.Distance(center, hit.position) > 2.0f)
                {
                    return hit.position;
                }
            }
        }
        return transform.position;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, catchRadius);
    }
    
    public void OnFootstep(AnimationEvent animationEvent)
    {
    }

    public void OnLand(AnimationEvent animationEvent)
    {
    }
}