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
            
            SpawnPlayer();
            SetGameState(GameState.Playing);
            
            Debug.Log("Game Started!");
        }
        
        private void SpawnPlayer()
        {
            if (playerPrefab != null)
            {
                Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : Vector3.zero;
                GameObject playerObj = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
                Player = playerObj.GetComponent<PlayerController>();
                Player.OnDeath += OnPlayerDeath;
                
                // Notify systems that player has spawned
                OnPlayerSpawned?.Invoke(Player);
                Debug.Log($"GameManager: Player spawned, notified {OnPlayerSpawned?.GetInvocationList().Length ?? 0} subscribers");
            }
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
