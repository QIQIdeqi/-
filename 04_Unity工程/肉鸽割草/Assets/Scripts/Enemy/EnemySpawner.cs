using UnityEngine;
using System.Collections.Generic;

namespace GeometryWarrior
{
    /// <summary>
    /// 单条难度记录数据
    /// </summary>
    [System.Serializable]
    public class DifficultyRecord
    {
        public float gameTime;       // 游戏时间（秒）
        public int level;            // 难度等级
        public float spawnInterval;  // 生成间隔
        public int maxEnemies;       // 最大敌人数量
        public int activeEnemies;    // 当前活跃敌人数
        public float enemyMultiplier; // 敌人属性倍率
        public int enemyMaxHealth;   // 敌人最大血量
        public int enemyAttack;      // 敌人攻击力
        
        /// <summary>
        /// 转换为CSV行
        /// </summary>
        public string ToCsvLine()
        {
            return $"{gameTime:F2},{level},{spawnInterval:F3},{maxEnemies},{activeEnemies},{enemyMultiplier:F2},{enemyMaxHealth},{enemyAttack}";
        }
    }
    
    /// <summary>
    /// 难度历史数据容器（用于JSON序列化）
    /// </summary>
    [System.Serializable]
    public class DifficultyHistoryData
    {
        public string sessionId;           // 游戏会话ID
        public string startTime;           // 开始时间
        public float totalGameTime;        // 总游戏时间
        public List<DifficultyRecord> records = new List<DifficultyRecord>();
    }
    
    /// <summary>
    /// EnemySpawner - Manages enemy spawning within Tile grid bounds
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        [Header("[Spawn Settings]")]
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private float spawnInterval = 1f;
        [SerializeField] private int maxEnemies = 30;
        [SerializeField] private float spawnRadius = 8f;  // 生成半径（从Player开始）
        [SerializeField] private float minSpawnDistance = 5f;  // 最小生成距离
        
        [Header("[Time-Based Difficulty]")]
        [Tooltip("难度增加间隔（秒）")]
        [SerializeField] private float difficultyIncreaseInterval = 5f;
        [Tooltip("每次难度增加的倍率")]
        [SerializeField] private float difficultyMultiplier = 1.2f;
        [Tooltip("最大生成速度（最小间隔）")]
        [SerializeField] private float minSpawnInterval = 0.2f;
        [Tooltip("最大敌人数量上限")]
        [SerializeField] private int maxEnemiesCap = 100;
        
        [Header("[Bounds Settings]")]
        [SerializeField] private bool limitToTileBounds = true;
        
        // 原始值（用于计算）
        private float baseSpawnInterval;
        private int baseMaxEnemies;
        
        private List<EnemyBase> enemyPool = new List<EnemyBase>();
        private List<EnemyBase> activeEnemies = new List<EnemyBase>();
        private float spawnTimer;
        private float difficultyTimer;  // 难度增加计时器
        private float gameTime;         // 游戏进行时间
        private int difficultyLevel = 0; // 当前难度等级
        private Transform player;
        private Bounds spawnBounds;  // 生成边界
        
        // 难度历史记录
        private List<DifficultyRecord> difficultyHistory = new List<DifficultyRecord>();
        
        private void Start()
        {
            // 保存原始值
            baseSpawnInterval = spawnInterval;
            baseMaxEnemies = maxEnemies;
            
            // 初始化会话
            InitializeSession();
            
            // 获取Tile边界
            UpdateSpawnBounds();
        }
        
        private void Update()
        {
            // 如果 player 为空，尝试重新查找
            if (player == null)
            {
                player = FindObjectOfType<PlayerController>()?.transform;
                if (player == null) return;
                
                // 找到Player后更新边界
                UpdateSpawnBounds();
            }
            
            // 更新游戏时间和难度
            gameTime += Time.deltaTime;
            difficultyTimer += Time.deltaTime;
            
            // 每 difficultyIncreaseInterval 秒增加一次难度
            if (difficultyTimer >= difficultyIncreaseInterval)
            {
                IncreaseDifficulty();
                difficultyTimer = 0f;
            }
            
            // Spawn enemies
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0 && activeEnemies.Count < maxEnemies)
            {
                SpawnEnemy();
                spawnTimer = spawnInterval;
            }
        }
        
        /// <summary>
        /// 增加难度：生成速度加快，最大敌人数量增加
        /// </summary>
        private void IncreaseDifficulty()
        {
            difficultyLevel++;
            
            // 生成间隔缩短 (除以倍率)
            spawnInterval /= difficultyMultiplier;
            spawnInterval = Mathf.Max(minSpawnInterval, spawnInterval);
            
            // 最大敌人数量增加 (乘以倍率)
            maxEnemies = Mathf.RoundToInt(baseMaxEnemies * Mathf.Pow(difficultyMultiplier, difficultyLevel));
            maxEnemies = Mathf.Min(maxEnemies, maxEnemiesCap);
            
            // 记录到历史
            RecordDifficulty();
            
            // 自动保存到本地
            if (autoSaveToLocal)
            {
                SaveToLocal();
            }
            
            Debug.Log($"[难度提升] 等级:{difficultyLevel} 时间:{gameTime:F1}s 生成间隔:{spawnInterval:F2}s 最大敌人:{maxEnemies}");
        }
        
        /// <summary>
        /// 记录当前难度状态到历史
        /// </summary>
        private void RecordDifficulty()
        {
            // 计算当前敌人属性
            float multiplier = Mathf.Pow(difficultyMultiplier, difficultyLevel);
            int baseHealth = 30; // 基础血量，应与EnemyBase一致
            int baseAttack = 10; // 基础攻击，应与EnemyBase一致
            
            DifficultyRecord record = new DifficultyRecord
            {
                gameTime = gameTime,
                level = difficultyLevel,
                spawnInterval = spawnInterval,
                maxEnemies = maxEnemies,
                activeEnemies = activeEnemies.Count,
                enemyMultiplier = multiplier,
                enemyMaxHealth = Mathf.RoundToInt(baseHealth * multiplier),
                enemyAttack = Mathf.RoundToInt(baseAttack * multiplier)
            };
            
            difficultyHistory.Add(record);
        }
        
        /// <summary>
        /// 获取难度历史记录（供外部查询）
        /// </summary>
        public List<DifficultyRecord> GetDifficultyHistory()
        {
            return new List<DifficultyRecord>(difficultyHistory);
        }
        
        /// <summary>
        /// 导出难度历史到控制台
        /// </summary>
        [ContextMenu("导出难度历史")]
        public void ExportDifficultyHistory()
        {
            Debug.Log("========== 难度递增历史记录 ==========");
            Debug.Log("  时间      等级    生成间隔      最大敌人    活跃敌人");
            Debug.Log("----------------------------------------");
            
            foreach (var record in difficultyHistory)
            {
                Debug.Log($"{record.gameTime:F1}s    {record.level,-4} {record.spawnInterval:F2}s      {record.maxEnemies,-6} {record.activeEnemies,-6}");
            }
            
            Debug.Log($"\n总计记录: {difficultyHistory.Count} 条");
            Debug.Log("=======================================");
            
            // 自动保存到本地
            SaveToLocal();
        }
        
        #region 本地文件保存
        
        [Header("[本地保存设置]")]
        [Tooltip("保存文件夹名称")]
        [SerializeField] private string saveFolderName = "GameLogs";
        [Tooltip("是否自动保存到本地")]
        [SerializeField] private bool autoSaveToLocal = true;
        [Tooltip("保存格式: csv 或 json")]
        [SerializeField] private SaveFormat saveFormat = SaveFormat.CSV;
        
        public enum SaveFormat { CSV, JSON }
        
        private string sessionId;
        private string startTime;
        
        /// <summary>
        /// 获取保存文件夹路径
        /// </summary>
        private string GetSaveFolderPath()
        {
            // 使用游戏目录下的文件夹
            string path = System.IO.Path.Combine(Application.dataPath, "..", saveFolderName);
            path = System.IO.Path.GetFullPath(path); // 转换为绝对路径
            return path;
        }
        
        /// <summary>
        /// 确保文件夹存在
        /// </summary>
        private void EnsureFolderExists()
        {
            string folderPath = GetSaveFolderPath();
            if (!System.IO.Directory.Exists(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
                Debug.Log($"[难度记录] 创建保存文件夹: {folderPath}");
            }
        }
        
        /// <summary>
        /// 初始化会话信息
        /// </summary>
        private void InitializeSession()
        {
            sessionId = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
            startTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        
        /// <summary>
        /// 保存到本地文件
        /// </summary>
        [ContextMenu("保存到本地")]
        public void SaveToLocal()
        {
            if (difficultyHistory.Count == 0)
            {
                Debug.LogWarning("[难度记录] 没有记录可保存");
                return;
            }
            
            EnsureFolderExists();
            
            if (string.IsNullOrEmpty(sessionId))
            {
                InitializeSession();
            }
            
            string filePath;
            if (saveFormat == SaveFormat.CSV)
            {
                filePath = SaveToCsv();
            }
            else
            {
                filePath = SaveToJson();
            }
            
            Debug.Log($"[难度记录] 已保存到: {filePath}");
        }
        
        /// <summary>
        /// 保存为CSV格式
        /// </summary>
        private string SaveToCsv()
        {
            string fileName = $"Difficulty_{sessionId}.csv";
            string filePath = System.IO.Path.Combine(GetSaveFolderPath(), fileName);
            
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            
            // 头部信息
            sb.AppendLine("# 几何战士 - 难度递增记录");
            sb.AppendLine($"# 会话ID: {sessionId}");
            sb.AppendLine($"# 开始时间: {startTime}");
            sb.AppendLine($"# 总游戏时间: {gameTime:F2}秒");
            sb.AppendLine($"# 最终难度等级: {difficultyLevel}");
            sb.AppendLine();
            
            // CSV表头
            sb.AppendLine("GameTime,Level,SpawnInterval,MaxEnemies,ActiveEnemies,EnemyMultiplier,EnemyMaxHealth,EnemyAttack");
            
            // 数据行
            foreach (var record in difficultyHistory)
            {
                sb.AppendLine(record.ToCsvLine());
            }
            
            System.IO.File.WriteAllText(filePath, sb.ToString(), System.Text.Encoding.UTF8);
            return filePath;
        }
        
        /// <summary>
        /// 保存为JSON格式
        /// </summary>
        private string SaveToJson()
        {
            string fileName = $"Difficulty_{sessionId}.json";
            string filePath = System.IO.Path.Combine(GetSaveFolderPath(), fileName);
            
            DifficultyHistoryData data = new DifficultyHistoryData
            {
                sessionId = sessionId,
                startTime = startTime,
                totalGameTime = gameTime,
                records = new List<DifficultyRecord>(difficultyHistory)
            };
            
            string json = JsonUtility.ToJson(data, true);
            System.IO.File.WriteAllText(filePath, json, System.Text.Encoding.UTF8);
            return filePath;
        }
        
        /// <summary>
        /// 打开保存文件夹
        /// </summary>
        [ContextMenu("打开保存文件夹")]
        public void OpenSaveFolder()
        {
            EnsureFolderExists();
            string folderPath = GetSaveFolderPath();
            
            // 使用系统默认方式打开文件夹
            System.Diagnostics.Process.Start("explorer.exe", folderPath);
        }
        
        #endregion
        
        /// <summary>
        /// 从TileManager获取生成边界
        /// </summary>
        private void UpdateSpawnBounds()
        {
            if (!limitToTileBounds) return;
            
            TileManager tileManager = FindObjectOfType<TileManager>();
            if (tileManager != null)
            {
                spawnBounds = tileManager.GridBounds;
                Debug.Log($"EnemySpawner: 生成边界已设置 - {spawnBounds}");
            }
        }
        
        private void SpawnEnemy()
        {
            if (enemyPrefab == null)
            {
                Debug.LogError("SpawnEnemy: Enemy Prefab 为空！");
                return;
            }
            
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
                // 设置敌人难度（根据当前难度等级计算实际倍率）
                float actualMultiplier = Mathf.Pow(difficultyMultiplier, difficultyLevel);
                enemy.SetDifficultyLevel(difficultyLevel, actualMultiplier);
                
                enemy.OnDeathEvent += OnEnemyDeath;
                activeEnemies.Add(enemy);
            }
        }
        
        private Vector2 GetSpawnPosition()
        {
            Vector2 playerPos = player.position;
            Vector2 spawnPos = playerPos;
            int maxAttempts = 20;
            
            for (int i = 0; i < maxAttempts; i++)
            {
                // 在Player周围的环形区域随机生成
                float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                float distance = Random.Range(minSpawnDistance, spawnRadius);
                
                spawnPos = playerPos + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
                
                // 检查是否在边界内
                if (IsValidSpawnPosition(spawnPos))
                {
                    return spawnPos;
                }
            }
            
            // 如果随机位置都不合法，尝试在边界上找位置
            return GetSpawnPositionOnBoundary();
        }
        
        /// <summary>
        /// 检查位置是否在有效生成区域内
        /// </summary>
        private bool IsValidSpawnPosition(Vector2 pos)
        {
            if (!limitToTileBounds || spawnBounds.size == Vector3.zero)
            {
                return true;  // 不限制边界
            }
            
            // 检查是否在Tile边界内
            if (pos.x < spawnBounds.min.x || pos.x > spawnBounds.max.x)
                return false;
            if (pos.y < spawnBounds.min.y || pos.y > spawnBounds.max.y)
                return false;
            
            return true;
        }
        
        /// <summary>
        /// 当Player靠近边缘时，在边界上找生成位置
        /// </summary>
        private Vector2 GetSpawnPositionOnBoundary()
        {
            if (!limitToTileBounds || spawnBounds.size == Vector3.zero)
            {
                // 不限制边界，在Player周围生成
                float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                return (Vector2)player.position + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnRadius;
            }
            
            // 根据Player位置，在相对的边界区域生成
            Vector2 playerPos = player.position;
            Vector2 center = spawnBounds.center;
            
            // 计算Player相对于中心的方向
            Vector2 toPlayer = (playerPos - center).normalized;
            
            // 如果Player在边缘，在对侧生成更多敌人
            Vector2 spawnCenter = playerPos + toPlayer * spawnRadius;
            
            // 限制在边界内
            spawnCenter.x = Mathf.Clamp(spawnCenter.x, spawnBounds.min.x, spawnBounds.max.x);
            spawnCenter.y = Mathf.Clamp(spawnCenter.y, spawnBounds.min.y, spawnBounds.max.y);
            
            // 在限制后的位置周围随机偏移
            float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float distance = Random.Range(0f, spawnRadius);
            Vector2 spawnPos = spawnCenter + new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)) * distance;
            
            // 最后确保在边界内
            spawnPos.x = Mathf.Clamp(spawnPos.x, spawnBounds.min.x, spawnBounds.max.x);
            spawnPos.y = Mathf.Clamp(spawnPos.y, spawnBounds.min.y, spawnBounds.max.y);
            
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
            
            // Add score
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(10);
            }
            
            // Return to pool
            if (!enemyPool.Contains(enemy))
            {
                enemyPool.Add(enemy);
            }
            
            enemy.gameObject.SetActive(false);
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
            
            // 重置难度
            ResetDifficulty();
        }
        
        /// <summary>
        /// 重置难度到初始值
        /// </summary>
        private void ResetDifficulty()
        {
            spawnInterval = baseSpawnInterval;
            maxEnemies = baseMaxEnemies;
            difficultyLevel = 0;
            difficultyTimer = 0f;
            gameTime = 0f;
            difficultyHistory.Clear();
            Debug.Log("[难度重置] 已恢复到初始值，历史记录已清空");
        }
        
        public int GetActiveEnemyCount()
        {
            return activeEnemies.Count;
        }
        
        /// <summary>
        /// 设置生成边界（供外部调用）
        /// </summary>
        public void SetSpawnBounds(Bounds bounds)
        {
            spawnBounds = bounds;
            limitToTileBounds = true;
        }
    }
}
