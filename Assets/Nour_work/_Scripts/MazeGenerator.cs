using UnityEngine;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.Behavior;

public class MazeGenerator : MonoBehaviour
{
    private Vector3 _startPosition;
    private CharacterController _playerController;
    private List<GameObject> _activeEnemies = new List<GameObject>();

    private int _size = 21; 
    
    [Header("Visuals")]
    public float spacing = 3.0f;
    public float minHeight = -0.2f;
    public float maxHeight = 0.2f;

    [Header("Prefabs")]
    public GameObject floorPrefab;
    public GameObject goalPrefab;
    public GameObject playerInstance;
    
    [Header("AI & Spawning")]
    public GameObject enemyPrefab;
    public int numberOfEnemies = 3;

    private int[,] _maze;
    private Vector2Int _center;
    private List<Vector2Int> _corners;

    void Start()
    {
        if (NightmareManager.Instance)
        {
            _size = NightmareManager.Instance.mazeSize;
            numberOfEnemies = NightmareManager.Instance.enemyCount;
        }
        else
        {
            _size = 21; 
            Debug.LogWarning("No NightmareManager found. Using default size 21.");
        }
        if (_size % 2 == 0) _size++;
        
        if (playerInstance) _playerController = playerInstance.GetComponent<CharacterController>();
        
        GenerateMaze();
        DrawMaze();
    }

    void GenerateMaze()
    {
        _maze = new int[_size,_size];
        
        for (int x = 0; x <_size; x++)
        for (int y = 0; y <_size; y++)
            _maze[x, y] = 1;
        
        _center = new Vector2Int(_size / 2,_size / 2);
        _corners = new List<Vector2Int> {
            new (1, 1),
            new (1,_size - 2),
            new (_size - 2, 1),
            new (_size - 2,_size - 2)
        };

        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        stack.Push(_center);
        _maze[_center.x, _center.y] = 0;
        HashSet<Vector2Int> visited = new HashSet<Vector2Int> { _center };
        
        while (stack.Count > 0)
        {
            Vector2Int current = stack.Peek();
            List<Vector2Int> neighbors = GetUnvisitedNeighbors(current, visited);
            
            if (neighbors.Count > 0)
            {
                Vector2Int next = neighbors[Random.Range(0, neighbors.Count)];
                _maze[(current.x + next.x) / 2, (current.y + next.y) / 2] = 0;
                _maze[next.x, next.y] = 0;
                visited.Add(next);
                stack.Push(next);
            }
            else
            {
                stack.Pop();
            }
        }
        foreach (Vector2Int corner in _corners) {
            _maze[corner.x, corner.y] = 0;
            if (corner.x == 1) _maze[2, corner.y] = 0;
            else _maze[_size - 3, corner.y] = 0;
        }
    }
    
    List<Vector2Int> GetUnvisitedNeighbors(Vector2Int p, HashSet<Vector2Int> visited)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        Vector2Int[] directions = { new (0, 2), new (0, -2), new (2, 0), new (-2, 0) };
        
        foreach (Vector2Int dir in directions)
        {
            Vector2Int next = p + dir;
            if (next.x > 0 && next.x <_size - 1 && next.y > 0 && next.y <_size - 1 && !visited.Contains(next))
            {
                neighbors.Add(next);
            }
        }
        return neighbors;
    }

    void DrawMaze()
    {
        SpawnGridGeometry();
        SpawnBackgroundMonoliths(100);
        SpawnFloatingDebris(50);

        BakeNavMesh();

        SpawnGoal();
        
        ResetGamePositions();

        StaticBatchingUtility.Combine(this.gameObject);
    }
    
    public void ResetGamePositions()
    { 
        foreach (GameObject enemy in _activeEnemies)
        {
            if (enemy) Destroy(enemy);
        }
        _activeEnemies.Clear();

        float spawnY = (minHeight + maxHeight) / 2f;
        int playerCornerIndex = Random.Range(0, 4);

        List<Vector3> validEnemySpawnPoints = new List<Vector3>();

        for (int i = 0; i < _corners.Count; i++)
        {
            Vector3 cornerPos = new Vector3(_corners[i].x * spacing, spawnY + 3.0f, _corners[i].y * spacing);

            if (i == playerCornerIndex)
            {
                _startPosition = cornerPos;
                TeleportPlayer(_startPosition);
            }
            else
            {
                validEnemySpawnPoints.Add(cornerPos);
            }
        }

        int enemiesToSpawn = numberOfEnemies;
        if (NightmareManager.Instance)
        {
            enemiesToSpawn = NightmareManager.Instance.enemyCount;
        }

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Vector3 spawnBase = validEnemySpawnPoints[i % validEnemySpawnPoints.Count];
            
            Vector3 randomOffset = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f));

            GameObject enemy = Instantiate(enemyPrefab, spawnBase + randomOffset, Quaternion.identity);
            _activeEnemies.Add(enemy);

            if (enemy.TryGetComponent<EnemyAI>(out var ai))
            {
                if (playerInstance)
                {
                    ai.playerTarget = playerInstance.transform;
                }
            }
        }
    }
    
    void TeleportPlayer(Vector3 pos)
    {
        if (_playerController) _playerController.enabled = false;
        playerInstance.transform.position = pos;
        if (_playerController) _playerController.enabled = true;
        
        var tpc = playerInstance.GetComponent<StarterAssets.ThirdPersonController>();
        if (tpc) 
        {
            tpc.enabled = true;
            playerInstance.transform.rotation = Quaternion.identity; 
        }
    }

    public void RespawnPlayer()
    {
        ResetGamePositions();
    }

    void SpawnGridGeometry()
    {
        for (int x = 0; x <_size; x++)
        {
            for (int y = 0; y <_size; y++)
            {
                if (_maze[x, y] == 0)
                {
                    float pillarHeight = Random.Range(10f, 20f);
                    float surfaceLevel = Random.Range(minHeight, maxHeight);
                    Vector3 pos = new Vector3(x * spacing, surfaceLevel - (pillarHeight / 2f), y * spacing);
                    
                    GameObject tile = Instantiate(floorPrefab, pos, Quaternion.identity, transform);
                    tile.isStatic = true;
                    tile.transform.localScale = new Vector3(spacing, pillarHeight, spacing);
                    if (tile.GetComponent<Collider>() == null) tile.AddComponent<BoxCollider>();
                }
                else
                {
                    GameObject invisoWall = new GameObject("InvisibleWall");
                    invisoWall.transform.position = new Vector3(x * spacing, 5f, y * spacing);
                    invisoWall.transform.parent = this.transform;

                    BoxCollider col = invisoWall.AddComponent<BoxCollider>();
                    col.size = new Vector3(spacing, 30f, spacing);
                    invisoWall.isStatic = true;
                }
            }
        }
    }

    void SpawnBackgroundMonoliths(int count)
    {
        for (int i = 0; i < count; i++)
        {
            float rx = Random.Range(-_size,_size * 2) * spacing;
            float rz = Random.Range(-_size,_size * 2) * spacing;
            
            int gridX = Mathf.RoundToInt(rx / spacing);
            int gridZ = Mathf.RoundToInt(rz / spacing);
            bool isInsideGrid = gridX >= 0 && gridX <_size && gridZ >= 0 && gridZ <_size;
            
            if (!isInsideGrid || _maze[gridX, gridZ] == 1)
            {
                float pHeight = Random.Range(15f, 40f); 
                float pOffset = Random.Range(-10f, -2f); 
                Vector3 pPos = new Vector3(rx, pOffset - (pHeight / 2f), rz);
                
                GameObject bgPillar = Instantiate(floorPrefab, pPos, Quaternion.identity, transform);
                float pWidth = Random.Range(spacing * 0.5f, spacing * 1.5f);
                bgPillar.transform.localScale = new Vector3(pWidth, pHeight, pWidth);
            }
        }
    }

    void SpawnFloatingDebris(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(-_size,_size * spacing), 
                Random.Range(5, 25), 
                Random.Range(-_size,_size * spacing)
            );
            GameObject bit = Instantiate(floorPrefab, randomPos, Random.rotation);
            bit.transform.localScale = Vector3.one * Random.Range(0.2f, 1.2f);
        }
    }

    void SpawnGoal()
    {
        float spawnY = (minHeight + maxHeight) / 2f;
        Vector3 goalPos = new Vector3(_center.x * spacing, spawnY + 1.5f, _center.y * spacing);
        if (goalPrefab)
        {
            GameObject goal = Instantiate(goalPrefab, goalPos, Quaternion.identity);
            playerInstance.GetComponent<PlayerAbilities>().goalPosition = goal.transform;
        }
    }

    void BakeNavMesh()
    {
        NavMeshSurface surface = GetComponent<NavMeshSurface>();
        if (surface != null) surface.BuildNavMesh();
    }
}