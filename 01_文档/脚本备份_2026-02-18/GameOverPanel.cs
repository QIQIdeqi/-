using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GeometryWarrior
{
    /// <summary>
    /// Game Over Panel
    /// </summary>
    public class GameOverPanel : MonoBehaviour
    {
        [Header("[Display Texts]")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI energyCoinsText;
        [SerializeField] private TextMeshProUGUI highScoreText;
        
        [Header("[Buttons]")]
        [SerializeField] private Button reviveButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button menuButton;
        
        [Header("[Ad Hint]")]
        [SerializeField] private TextMeshProUGUI reviveHintText;
        
        private GameManager gameManager;
        
        private void Awake()
        {
            gameManager = GameManager.Instance;
            
            if (reviveButton != null)
                reviveButton.onClick.AddListener(OnReviveButtonClicked);
            
            if (restartButton != null)
                restartButton.onClick.AddListener(OnRestartButtonClicked);
            
            if (menuButton != null)
                menuButton.onClick.AddListener(OnMenuButtonClicked);
        }
        
        private void OnEnable()
        {
            UpdateDisplay();
        }
        
        public void UpdateDisplay()
        {
            if (gameManager == null) return;
            
            if (scoreText != null)
            {
                scoreText.text = $"Score: {gameManager.Score}";
            }
            
            if (timeText != null)
            {
                float time = gameManager.GameTime;
                int minutes = Mathf.FloorToInt(time / 60f);
                int seconds = Mathf.FloorToInt(time % 60f);
                timeText.text = $"Survived: {minutes:00}:{seconds:00}";
            }
            
            if (energyCoinsText != null)
            {
                int earnedCoins = Mathf.RoundToInt(gameManager.Score / 100f);
                energyCoinsText.text = $"Energy: +{earnedCoins}";
            }
            
            // Save disabled temporarily
            // int highScore = PlayerPrefs.GetInt("HighScore", 0);
            // if (gameManager.Score > highScore)
            // {
            //     highScore = gameManager.Score;
            //     PlayerPrefs.SetInt("HighScore", highScore);
            //     PlayerPrefs.Save();
            // }
            
            if (highScoreText != null)
            {
                highScoreText.text = $"High Score: {gameManager.Score}";
            }
        }
        
        private void OnReviveButtonClicked()
        {
            if (gameManager != null)
            {
                gameManager.RevivePlayer();
                Hide();
            }
        }
        
        private void OnRestartButtonClicked()
        {
            if (gameManager != null)
            {
                gameManager.RestartGame();
                Hide();
            }
        }
        
        private void OnMenuButtonClicked()
        {
            if (gameManager != null)
            {
                gameManager.ReturnToMenu();
                Hide();
            }
        }
        
        public void Show()
        {
            gameObject.SetActive(true);
            UpdateDisplay();
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
