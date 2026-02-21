using UnityEngine;

namespace GeometryWarrior
{
    /// <summary>
    /// GameManager - Singleton pattern, manages global game state and UI
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        public enum GameState
        {
            Menu,       // Main menu
            Playing,    // Game in progress
            Paused,     // Paused
            GameOver    // Game over
        }
        
        [Header("[Game State]")]
        [SerializeField] private GameState currentState = GameState.Menu;
        
        [Header("[Player Spawn]")]
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Transform spawnPoint;
        
        [Header("[UI Panels]")]
        [SerializeField] private MainMenuPanel mainMenuPanel;
        [SerializeField] private GameOverPanel gameOverPanel;
        [SerializeField] private GameObject hudCanvas;
        
        [Header("[Game Data]")]
        [SerializeField] private float gameTime = 0f;
        [SerializeField] private int score = 0;
        [SerializeField] private int energyCoins = 0;
        
        public GameState CurrentState => currentState;
        public float GameTime => gameTime;
        public int Score => score;
        public int EnergyCoins => energyCoins;
        
        public System.Action<GameState> OnGameStateChanged;
        public System.Action<float> OnGameTimeUpdated;
        public System.Action<int> OnScoreChanged;
        public System.Action<PlayerController> OnPlayerSpawned;
        
        public PlayerController Player { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            
            LoadData();
        }
        
        private void Start()
        {
            UpdateUIForState();
        }
        
        private void Update()
        {
            if (currentState == GameState.Playing)
            {
                gameTime += Time.deltaTime;
                OnGameTimeUpdated?.Invoke(gameTime);
            }
        }
        
        public void StartGame()
        {
            gameTime = 0f;
            score = 0;
            
            if (Player != null)
            {
                Destroy(Player.gameObject);
            }
            
            // 先初始化TileManager（确定游戏区域）
            InitializeTileManager();
            
            // 再生成Player（在Tile中心出生）
            SpawnPlayer();
            SetGameState(GameState.Playing);
            
            Debug.Log("Game Started!");
        }
        
        /// <summary>
        /// 初始化TileManager，设置游戏区域
        /// </summary>
        private void InitializeTileManager()
        {
            TileManager tileManager = FindObjectOfType<TileManager>();
            if (tileManager != null)
            {
                // 如果有spawnPoint，让Tile中心对准spawnPoint
                // 否则使用原点
                Vector3 centerPos = spawnPoint != null ? spawnPoint.position : Vector3.zero;
                tileManager.CenterOnPosition(centerPos);
                Debug.Log($"GameManager: TileManager已初始化，中心在 {centerPos}");
            }
        }
        
        private void SpawnPlayer()
        {
            if (playerPrefab != null)
            {
                // 获取TileManager的中心点作为出生位置
                Vector3 spawnPos = GetTileGridCenter();
                Bounds bounds = GetTileGridBounds();
                
                GameObject playerObj = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
                Player = playerObj.GetComponent<PlayerController>();
                Player.OnDeath += OnPlayerDeath;
                
                // 设置Player的移动边界
                Player.SetMovementBounds(bounds);
                
                // 设置EnemySpawner的生成边界
                EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
                if (spawner != null)
                {
                    spawner.SetSpawnBounds(bounds);
                }
                
                // Notify systems that player has spawned
                OnPlayerSpawned?.Invoke(Player);
                Debug.Log($"GameManager: Player出生在Tile中心 {spawnPos}, 边界: {bounds}");
            }
        }
        
        /// <summary>
        /// 获取Tile网格的中心点
        /// </summary>
        private Vector3 GetTileGridCenter()
        {
            TileManager tileManager = FindObjectOfType<TileManager>();
            if (tileManager != null)
            {
                return tileManager.GridBounds.center;
            }
            // 如果没有TileManager，使用spawnPoint或原点
            return spawnPoint != null ? spawnPoint.position : Vector3.zero;
        }
        
        /// <summary>
        /// 获取Tile网格的边界
        /// </summary>
        private Bounds GetTileGridBounds()
        {
            TileManager tileManager = FindObjectOfType<TileManager>();
            if (tileManager != null)
            {
                return tileManager.GridBounds;
            }
            // 如果没有TileManager，返回无限边界
            return new Bounds(Vector3.zero, Vector3.one * 1000f);
        }
        
        private void OnPlayerDeath()
        {
            energyCoins += Mathf.RoundToInt(score / 100f);
            SetGameState(GameState.GameOver);
            Debug.Log($"Game Over! Survived: {gameTime:F1}s, Score: {score}, Energy Coins: {energyCoins}");
        }
        
        public void PauseGame()
        {
            if (currentState == GameState.Playing)
            {
                SetGameState(GameState.Paused);
                Time.timeScale = 0f;
            }
        }
        
        public void ResumeGame()
        {
            if (currentState == GameState.Paused)
            {
                SetGameState(GameState.Playing);
                Time.timeScale = 1f;
            }
        }
        
        public void RestartGame()
        {
            Time.timeScale = 1f;
            
            EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
            if (spawner != null)
            {
                spawner.ClearAllEnemies();
            }
            
            StartGame();
        }
        
        public void ReturnToMenu()
        {
            Time.timeScale = 1f;
            
            if (Player != null)
            {
                Destroy(Player.gameObject);
                Player = null;
            }
            
            EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
            if (spawner != null)
            {
                spawner.ClearAllEnemies();
            }
            
            SetGameState(GameState.Menu);
        }
        
        private void SetGameState(GameState newState)
        {
            currentState = newState;
            OnGameStateChanged?.Invoke(currentState);
            UpdateUIForState();
        }
        
        private void UpdateUIForState()
        {
            if (mainMenuPanel != null)
            {
                if (currentState == GameState.Menu)
                    mainMenuPanel.Show();
                else
                    mainMenuPanel.Hide();
            }
            
            if (hudCanvas != null)
            {
                hudCanvas.SetActive(currentState == GameState.Playing);
            }
            
            if (gameOverPanel != null)
            {
                if (currentState == GameState.GameOver)
                    gameOverPanel.Show();
                else
                    gameOverPanel.Hide();
            }
        }
        
        public void AddScore(int points)
        {
            score += points;
            OnScoreChanged?.Invoke(score);
        }
        
        public void RevivePlayer()
        {
            if (Player != null && Player.IsDead)
            {
                Player.Revive();
                SetGameState(GameState.Playing);
            }
        }
        
        public void SaveData()
        {
            // Temporarily disabled for WeChat export
            // PlayerPrefs.SetInt("EnergyCoins", energyCoins);
            // PlayerPrefs.Save();
        }
        
        public void LoadData()
        {
            // Temporarily disabled for WeChat export
            // energyCoins = PlayerPrefs.GetInt("EnergyCoins", 0);
            energyCoins = 0;
        }
        
        private void OnApplicationQuit()
        {
            SaveData();
        }
    }
}
