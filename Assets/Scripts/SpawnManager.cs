using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform enemyContainer;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private int maxActiveEnemies = 6;
    [SerializeField] private int minActiveEnemies = 4;
    [SerializeField] private float spawnInterval = 3f;

    private Queue<GameObject> pooledEnemies = new Queue<GameObject>();
    private int activeEnemyCount = 0;
    private float spawnTimer = 0f;

    void Awake()
    {
        // CHANGED: Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        InitializePool();
    }

    void Update()
    {
        // CHANGED: Auto-spawn logic
        if (activeEnemyCount < maxActiveEnemies)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnInterval && activeEnemyCount < minActiveEnemies)
            {
                SpawnEnemy();
                spawnTimer = 0f;
            }
        }
    }

    void InitializePool()
    {
        // CHANGED: Pre-create pooled enemies
        for (int i = 0; i < poolSize; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab, enemyContainer);
            enemy.SetActive(false);
            pooledEnemies.Enqueue(enemy);
        }
    }

    public GameObject SpawnEnemy()
    {
        // CHANGED: Reuse from pool or create new
        GameObject enemy;

        if (pooledEnemies.Count > 0)
        {
            enemy = pooledEnemies.Dequeue();
        }
        else
        {
            enemy = Instantiate(enemyPrefab, enemyContainer);
        }

        enemy.transform.position = startPoint.position;
        enemy.SetActive(true);
        activeEnemyCount++;
        return enemy;
    }

    public void ReturnEnemyToPool(GameObject enemy)
    {
        // CHANGED: Return to pool instead of destroy
        enemy.SetActive(false);
        pooledEnemies.Enqueue(enemy);
        activeEnemyCount--;
    }

    public int GetActiveEnemyCount() => activeEnemyCount;
}