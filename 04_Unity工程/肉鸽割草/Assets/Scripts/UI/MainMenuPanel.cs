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
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private TextMeshProUGUI energyCoinsText;
        
        [Header("[Buttons]")]
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;
        
        private GameManager gameManager;
        
        private void Awake()
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
                highScoreText.text = "High Score: 0";
            }
            
            // int energyCoins = PlayerPrefs.GetInt("EnergyCoins", 0);
            if (energyCoinsText != null)
            {
                energyCoinsText.text = "Energy: 0";
            }
        }
        
        private void OnStartGameClicked()
        {
            Hide();
            
            if (gameManager != null)
            {
                gameManager.StartGame();
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
