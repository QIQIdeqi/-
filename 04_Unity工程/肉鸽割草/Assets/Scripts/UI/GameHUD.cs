using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GeometryWarrior
{
    /// <summary>
    /// Game HUD - Displays health, exp, score, and time
    /// </summary>
    public class GameHUD : MonoBehaviour
    {
        [Header("[Health]")]
        [SerializeField] private Slider healthSlider;
        [SerializeField] private TextMeshProUGUI healthText;
        
        [Header("[Experience]")]
        [SerializeField] private Slider expSlider;
        [SerializeField] private TextMeshProUGUI levelText;
        
        [Header("[Score & Time]")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI timeText;
        
        private PlayerController player;
        private GameManager gameManager;
        
        private void Start()
        {
            gameManager = GameManager.Instance;
            
            if (gameManager != null)
            {
                gameManager.OnScoreChanged += UpdateScore;
                gameManager.OnGameTimeUpdated += UpdateTime;
            }
        }
        
        private void Update()
        {
            // Find player if not found
            if (player == null)
            {
                player = FindObjectOfType<PlayerController>();
                if (player != null)
                {
                    player.OnHealthChanged += UpdateHealth;
                    player.OnExpChanged += UpdateExp;
                    player.OnLevelUp += UpdateLevel;
                    
                    // Initial update
                    UpdateHealth(player.CurrentHealth);
                    UpdateExp(player.CurrentExp, player.ExpToNextLevel);
                    UpdateLevel(player.CurrentLevel);
                }
            }
        }
        
        private void UpdateHealth(int health)
        {
            if (player == null) return;
            
            if (healthSlider != null)
            {
                healthSlider.maxValue = player.MaxHealth;
                healthSlider.value = health;
            }
            
            if (healthText != null)
            {
                healthText.text = $"{health}/{player.MaxHealth}";
            }
        }
        
        private void UpdateExp(int currentExp, int expToNext)
        {
            if (expSlider != null)
            {
                expSlider.maxValue = expToNext;
                expSlider.value = currentExp;
            }
        }
        
        private void UpdateLevel(int level)
        {
            if (levelText != null)
            {
                levelText.text = $"Lv.{level}";
            }
        }
        
        private void UpdateScore(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = $"分数:{score}";
            }
        }
        
        private void UpdateTime(float time)
        {
            if (timeText != null)
            {
                int minutes = Mathf.FloorToInt(time / 60f);
                int seconds = Mathf.FloorToInt(time % 60f);
                timeText.text = $"{minutes:00}:{seconds:00}";
            }
        }
        
        private void OnDestroy()
        {
            if (player != null)
            {
                player.OnHealthChanged -= UpdateHealth;
                player.OnExpChanged -= UpdateExp;
                player.OnLevelUp -= UpdateLevel;
            }
            
            if (gameManager != null)
            {
                gameManager.OnScoreChanged -= UpdateScore;
                gameManager.OnGameTimeUpdated -= UpdateTime;
            }
        }
    }
}
