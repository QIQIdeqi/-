using UnityEngine;
using System.Collections.Generic;

namespace GeometryWarrior
{
    /// <summary>
    /// EnemySpawner - Manages enemy spawning with object pooling and wave system
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        [Header("[Spawn Settings]")]
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private float spawnInterval = 1f;
        [SerializeField] private int maxEnemies = 30;
        [SerializeField] private float spawnRadius = 15f;
        [SerializeField] private float minSpawnDistance = 10f;
        
        [Header("[Wave Settings]")]
        [SerializeField] private float waveDuration = 30f;
        [SerializeField] private float difficultyMultiplier = 1.1f;
        
        private List<EnemyBase> enemyPool = new List<EnemyBase>();
        private List<EnemyBase> activeEnemies = new List<EnemyBase>();
        private float spawnTimer;
        private float waveTimer;
        private int currentWave = 1;
        private Transform player;
        
        private void Start()
        {
            player = FindObjectOfType<PlayerController>()?.transform;
        }
        
        private void Update()
        {
            if (player == null) return;
            
            // Spawn enemies
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0 && activeEnemies.Count < maxEnemies)
            {
                SpawnEnemy();
                spawnTimer = spawnInterval;
            }
            
            // Wave system
            waveTimer += Time.deltaTime;
            if (waveTimer >= waveDuration)
            {
                NextWave();
            }
        }
        
        private void SpawnEnemy()
        {
            Vector2 spawnPos = GetSpawnPosition();
            
            // Try to get from pool
            EnemyBase enemy = GetEnemyFromPool();
            
            if (enemy == null)
            {
                // Create new enemy
                GameObject enemyObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
                enemy = enemyObj.GetComponent<EnemyBase>();
            }
            else
            {
                // Reset pooled enemy
                enemy.transform.position = spawnPos;
                enemy.gameObject.SetActive(true);
            }
            
            if (enemy != null)
            {
                enemy.OnDeathEvent += OnEnemyDeath;
                activeEnemies.Add(enemy);
            }
        }
        
        private Vector2 GetSpawnPosition()
        {
            Vector2 spawnPos;
            int attempts = 0;
            
            do
            {
                float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                float distance = Random.Range(minSpawnDistance, spawnRadius);
                spawnPos = (Vector2)player.position + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
                attempts++;
            }
            while (attempts < 10);
            
            return spawnPos;
        }
        
        private EnemyBase GetEnemyFromPool()
        {
            foreach (EnemyBase enemy in enemyPool)
            {
                if (!enemy.gameObject.activeInHierarchy)
                {
                    return enemy;
                }
            }
            return null;
        }
        
        private void OnEnemyDeath(EnemyBase enemy)
        {
            enemy.OnDeathEvent -= OnEnemyDeath;
            activeEnemies.Remove(enemy);
            
            // Return to pool
            if (!enemyPool.Contains(enemy))
            {
                enemyPool.Add(enemy);
            }
            
            enemy.gameObject.SetActive(false);
        }
        
        private void NextWave()
        {
            currentWave++;
            waveTimer = 0f;
            
            // Increase difficulty
            spawnInterval /= difficultyMultiplier;
            spawnInterval = Mathf.Max(0.3f, spawnInterval);
            
            Debug.Log($"Wave {currentWave} Started!");
        }
        
        public void ClearAllEnemies()
        {
            foreach (EnemyBase enemy in activeEnemies)
            {
                if (enemy != null)
                {
                    enemy.OnDeathEvent -= OnEnemyDeath;
                    enemy.gameObject.SetActive(false);
                }
            }
            activeEnemies.Clear();
        }
        
        public int GetActiveEnemyCount()
        {
            return activeEnemies.Count;
        }
    }
}
