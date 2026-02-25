using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GeometryWarrior
{
    /// <summary>
    /// 主菜单面板
    /// </summary>
    public class MainMenuPanel : MonoBehaviour
    {
        [Header("【显示文本】")]
        [Tooltip("标题Logo图片")]
        [SerializeField] private Image titleImage;
        
        [Tooltip("最高分显示文本")]
        [SerializeField] private TextMeshProUGUI highScoreText;
        
        [Tooltip("能量币显示文本")]
        [SerializeField] private TextMeshProUGUI energyCoinsText;
        
        [Header("【按钮】")]
        [Tooltip("开始游戏按钮")]
        [SerializeField] private Button startGameButton;
        
        [Tooltip("进入家园场景按钮")]
        [SerializeField] private Button homeButton;
        
        [Tooltip("设置按钮")]
        [SerializeField] private Button settingsButton;
        
        [Tooltip("退出游戏按钮")]
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
            
            if (homeButton != null)
                homeButton.onClick.AddListener(OnHomeClicked);
            
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
        
        private void OnHomeClicked()
        {
            Debug.Log("[MainMenuPanel] Home clicked! Entering home scene...");
            
            if (gameManager != null)
            {
                gameManager.EnterHomeScene();
            }
            else
            {
                Debug.LogError("[MainMenuPanel] gameManager is null!");
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
