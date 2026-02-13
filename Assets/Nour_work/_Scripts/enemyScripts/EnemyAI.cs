using UnityEngine;
using UnityEngine.AI;
using Unity.Cinemachine;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Header("Targeting")]
    public Transform playerTarget;
    
    [Header("Ranges")]
    public float detectionRadius = 10f;
    public float escapeRadius = 15f; 
    public float catchRadius = 1f; 

    [Header("Wander Settings")]
    public float wanderRadius = 8f;     
    public float wanderTimer = 4f;      

    [Header("Speeds")]
    public float wanderSpeed = 2.0f;
    public float chaseSpeed = 5.0f;

    private NavMeshAgent _agent;
    private float _timer;
    private bool _isChasing = false;
    private bool _hasCaughtPlayer = false; 

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _timer = wanderTimer;
    }

    void Update()
    {
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
            if (playerAnim) playerAnim.SetFloat("Speed", 0f);
        }

        Animator anim = GetComponentInChildren<Animator>();
        var camComponent = GetComponentInChildren<CinemachineCamera>(true);

        if (camComponent && JumpScareManager.Instance)
        {
            JumpScareManager.Instance.TriggerScare(camComponent.gameObject, anim, () => 
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
        if (_timer >= wanderTimer || _agent.remainingDistance < 0.5f)
        {
            Vector3 newPos = GetRandomPoint(transform.position, wanderRadius);
            _agent.SetDestination(newPos);
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
                if (Vector3.Distance(center, hit.position) > 3.0f)
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
}