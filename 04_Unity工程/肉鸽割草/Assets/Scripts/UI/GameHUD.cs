using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GeometryWarrior
{
    /// <summary>
    /// Game HUD - Displays health, exp, score, and time
    /// 支持 TextMeshPro 和 普通 UI Text
    /// </summary>
    public class GameHUD : MonoBehaviour
    {
        [Header("[Health]")]
        [SerializeField] private Slider healthSlider;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private Text healthTextLegacy; // 普通Text备选
        
        [Header("[Experience]")]
        [SerializeField] private Slider expSlider;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private Text levelTextLegacy; // 普通Text备选
        
        [Header("[Score & Time]")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private Text scoreTextLegacy; // 普通Text备选
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private Text timeTextLegacy; // 普通Text备选
        
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
            
            string healthStr = $"{health}/{player.MaxHealth}";
            if (healthText != null)
                healthText.text = healthStr;
            if (healthTextLegacy != null)
                healthTextLegacy.text = healthStr;
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
            string levelStr = $"Lv.{level}";
            if (levelText != null)
                levelText.text = levelStr;
            if (levelTextLegacy != null)
                levelTextLegacy.text = levelStr;
        }
        
        private void UpdateScore(int score)
        {
            string scoreStr = $"分数:{score}";
            if (scoreText != null)
                scoreText.text = scoreStr;
            if (scoreTextLegacy != null)
                scoreTextLegacy.text = scoreStr;
        }
        
        private void UpdateTime(float time)
        {
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            string timeStr = $"{minutes:00}:{seconds:00}";
            
            if (timeText != null)
                timeText.text = timeStr;
            if (timeTextLegacy != null)
                timeTextLegacy.text = timeStr;
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
