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
        [SerializeField] private Text titleTextLegacy;           // 普通Text备选
        
        [Header("[显示文本]")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private Text scoreTextLegacy;           // 普通Text备选
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private Text timeTextLegacy;            // 普通Text备选
        [SerializeField] private TextMeshProUGUI energyCoinsText;
        [SerializeField] private Text energyCoinsTextLegacy;     // 普通Text备选
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private Text highScoreTextLegacy;       // 普通Text备选
        
        [Header("[按钮]")]
        [SerializeField] private Button reviveButton;            // 复活/继续按钮
        [SerializeField] private TextMeshProUGUI reviveButtonText; // 按钮文字
        [SerializeField] private Text reviveButtonTextLegacy;    // 普通Text备选
        [SerializeField] private Button restartButton;           // 重新开始按钮
        [SerializeField] private Button menuButton;              // 返回菜单按钮
        
        [Header("[提示文本]")]
        [SerializeField] private TextMeshProUGUI reviveHintText; // 复活提示（看广告等）
        [SerializeField] private Text reviveHintTextLegacy;      // 普通Text备选
        
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
            string titleStr = mode == PanelMode.GameOver ? "游戏结束" : "游戏暂停";
            if (titleText != null) titleText.text = titleStr;
            if (titleTextLegacy != null) titleTextLegacy.text = titleStr;
            
            // 设置复活/继续按钮文字
            string buttonStr = mode == PanelMode.GameOver ? "复活（看广告）" : "继续游戏";
            if (reviveButtonText != null) reviveButtonText.text = buttonStr;
            if (reviveButtonTextLegacy != null) reviveButtonTextLegacy.text = buttonStr;
            
            // 设置提示文字
            if (mode == PanelMode.GameOver)
            {
                string hintStr = "观看广告即可复活继续游戏";
                if (reviveHintText != null)
                {
                    reviveHintText.text = hintStr;
                    reviveHintText.gameObject.SetActive(true);
                }
                if (reviveHintTextLegacy != null)
                {
                    reviveHintTextLegacy.text = hintStr;
                    reviveHintTextLegacy.gameObject.SetActive(true);
                }
            }
            else
            {
                if (reviveHintText != null) reviveHintText.gameObject.SetActive(false);
                if (reviveHintTextLegacy != null) reviveHintTextLegacy.gameObject.SetActive(false);
            }
        }
        
        /// <summary>
        /// 更新显示数据
        /// </summary>
        public void UpdateDisplay()
        {
            if (gameManager == null) return;
            
            string scoreStr = $"得分: {gameManager.Score}";
            if (scoreText != null) scoreText.text = scoreStr;
            if (scoreTextLegacy != null) scoreTextLegacy.text = scoreStr;
            
            float time = gameManager.GameTime;
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            string timeStr = $"存活时间: {minutes:00}:{seconds:00}";
            if (timeText != null) timeText.text = timeStr;
            if (timeTextLegacy != null) timeTextLegacy.text = timeStr;
            
            int earnedCoins = Mathf.RoundToInt(gameManager.Score / 100f);
            string coinsStr = $"获得能量币: +{earnedCoins}";
            if (energyCoinsText != null) energyCoinsText.text = coinsStr;
            if (energyCoinsTextLegacy != null) energyCoinsTextLegacy.text = coinsStr;
            
            string highScoreStr = $"本局得分: {gameManager.Score}";
            if (highScoreText != null) highScoreText.text = highScoreStr;
            if (highScoreTextLegacy != null) highScoreTextLegacy.text = highScoreStr;
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
