using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GeometryWarrior
{
    /// <summary>
    /// Game Over / Pause Panel - 支持死亡和暂停两种模式
    /// </summary>
    public class GameOverPanel : MonoBehaviour
    {
        public enum PanelMode
        {
            GameOver,   // 死亡模式
            Paused      // 暂停模式
        }
        
        [Header("[标题]")]
        [SerializeField] private TextMeshProUGUI titleText;      // 标题（游戏结束/游戏暂停）
        
        [Header("[显示文本]")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI energyCoinsText;
        [SerializeField] private TextMeshProUGUI highScoreText;
        
        [Header("[按钮]")]
        [SerializeField] private Button reviveButton;            // 复活/继续按钮
        [SerializeField] private TextMeshProUGUI reviveButtonText; // 按钮文字
        [SerializeField] private Button restartButton;           // 重新开始按钮
        [SerializeField] private Button menuButton;              // 返回菜单按钮
        
        [Header("[提示文本]")]
        [SerializeField] private TextMeshProUGUI reviveHintText; // 复活提示（看广告等）
        
        private GameManager gameManager;
        private PanelMode currentMode = PanelMode.GameOver;
        
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
        
        /// <summary>
        /// 显示面板（根据模式显示不同内容）
        /// </summary>
        /// <param name="mode">PanelMode.GameOver 或 PanelMode.Paused</param>
        public void Show(PanelMode mode = PanelMode.GameOver)
        {
            currentMode = mode;
            SetupForMode(mode);
            gameObject.SetActive(true);
            UpdateDisplay();
        }
        
        /// <summary>
        /// 根据模式设置面板
        /// </summary>
        private void SetupForMode(PanelMode mode)
        {
            // 设置标题
            if (titleText != null)
            {
                titleText.text = mode == PanelMode.GameOver ? "游戏结束" : "游戏暂停";
            }
            
            // 设置复活/继续按钮文字
            if (reviveButtonText != null)
            {
                reviveButtonText.text = mode == PanelMode.GameOver ? "复活（看广告）" : "继续游戏";
            }
            
            // 设置提示文字
            if (reviveHintText != null)
            {
                if (mode == PanelMode.GameOver)
                {
                    reviveHintText.text = "观看广告即可复活继续游戏";
                    reviveHintText.gameObject.SetActive(true);
                }
                else
                {
                    reviveHintText.gameObject.SetActive(false); // 暂停时不显示复活提示
                }
            }
            
            // 暂停时隐藏重新开始按钮（可选，也可以保留）
            if (restartButton != null)
            {
                // 暂停时也可以重新开始，所以这里不做处理
                // 如果你想暂停时隐藏重新开始，取消下面注释：
                // restartButton.gameObject.SetActive(mode == PanelMode.GameOver);
            }
        }
        
        /// <summary>
        /// 更新显示数据
        /// </summary>
        public void UpdateDisplay()
        {
            if (gameManager == null) return;
            
            if (scoreText != null)
            {
                scoreText.text = $"得分: {gameManager.Score}";
            }
            
            if (timeText != null)
            {
                float time = gameManager.GameTime;
                int minutes = Mathf.FloorToInt(time / 60f);
                int seconds = Mathf.FloorToInt(time % 60f);
                timeText.text = $"存活时间: {minutes:00}:{seconds:00}";
            }
            
            if (energyCoinsText != null)
            {
                int earnedCoins = Mathf.RoundToInt(gameManager.Score / 100f);
                energyCoinsText.text = $"获得能量币: +{earnedCoins}";
            }
            
            if (highScoreText != null)
            {
                highScoreText.text = $"本局得分: {gameManager.Score}";
            }
        }
        
        /// <summary>
        /// 复活/继续按钮点击
        /// </summary>
        private void OnReviveButtonClicked()
        {
            if (gameManager == null) return;
            
            if (currentMode == PanelMode.GameOver)
            {
                // 死亡模式：复活玩家
                gameManager.RevivePlayer();
            }
            else
            {
                // 暂停模式：继续游戏
                gameManager.ResumeGame();
            }
            
            Hide();
        }
        
        /// <summary>
        /// 重新开始按钮点击
        /// </summary>
        private void OnRestartButtonClicked()
        {
            if (gameManager != null)
            {
                // 无论暂停还是死亡，重新开始都重置游戏
                gameManager.RestartGame();
                Hide();
            }
        }
        
        /// <summary>
        /// 返回菜单按钮点击
        /// </summary>
        private void OnMenuButtonClicked()
        {
            if (gameManager != null)
            {
                gameManager.ReturnToMenu();
                Hide();
            }
        }
        
        /// <summary>
        /// 隐藏面板
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        /// <summary>
        /// 获取当前模式
        /// </summary>
        public PanelMode GetCurrentMode()
        {
            return currentMode;
        }
    }
}
