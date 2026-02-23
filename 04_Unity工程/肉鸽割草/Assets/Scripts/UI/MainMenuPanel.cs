using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GeometryWarrior
{
    /// <summary>
    /// Main Menu Panel
    /// </summary>
    public class MainMenuPanel : MonoBehaviour
    {
        [Header("[Display Texts]")]
        [SerializeField] private Image titleImage;  // Title changed from Text to Image (Logo)
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private TextMeshProUGUI energyCoinsText;
        
        [Header("[Buttons]")]
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;
        
        private GameManager gameManager;
        
        private void Awake()
        {
            // 在 Start 中获取 GameManager，避免执行顺序问题
        }
        
        private void Start()
        {
            gameManager = GameManager.Instance;
            
            if (startGameButton != null)
                startGameButton.onClick.AddListener(OnStartGameClicked);
            
            if (settingsButton != null)
                settingsButton.onClick.AddListener(OnSettingsClicked);
            
            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuitClicked);
        }
        
        private void OnEnable()
        {
            UpdateDisplay();
        }
        
        private void UpdateDisplay()
        {
            // Save disabled temporarily
            // int highScore = PlayerPrefs.GetInt("HighScore", 0);
            if (highScoreText != null)
            {
                highScoreText.text = "最高分: 0";
            }
            
            // int energyCoins = PlayerPrefs.GetInt("EnergyCoins", 0);
            if (energyCoinsText != null)
            {
                energyCoinsText.text = "能量币: 0";
            }
        }
        
        private void OnStartGameClicked()
        {
            Debug.Log("[MainMenuPanel] Start Game clicked!");
            Hide();
            
            if (gameManager != null)
            {
                Debug.Log("[MainMenuPanel] Calling gameManager.StartGame()");
                gameManager.StartGame();
            }
            else
            {
                Debug.LogError("[MainMenuPanel] gameManager is null! Trying to find...");
                gameManager = GameManager.Instance;
                if (gameManager != null)
                {
                    gameManager.StartGame();
                }
            }
        }
        
        private void OnSettingsClicked()
        {
            Debug.Log("Open Settings");
        }
        
        private void OnQuitClicked()
        {
            if (gameManager != null)
            {
                gameManager.SaveData();
            }
            
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
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
