using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GeometryWarrior
{
    /// <summary>
    /// 家园场景HUD - 显示摇杆、背包按钮、提示信息等
    /// </summary>
    public class HomeHUD : MonoBehaviour
    {
        [Header("【摇杆】")]
        [Tooltip("虚拟摇杆")] public Joystick joystick;
        
        [Header("【按钮】")]
        [Tooltip("背包按钮")] public Button backpackButton;
        [Tooltip("设置按钮")] public Button settingsButton;
        // 返回按钮已移除 - 通过门离开家园
        
        [Header("【提示区域】")]
        [Tooltip("NPC对话提示")] public GameObject npcHintPanel;
        [Tooltip("NPC提示文字")] public TextMeshProUGUI npcHintText;
        [Tooltip("离开提示")] public GameObject exitHintPanel;
        [Tooltip("离开提示文字")] public TextMeshProUGUI exitHintText;
        
        [Header("【Toast提示】")]
        [Tooltip("Toast容器")] public Transform toastContainer;
        [Tooltip("Toast预制体")] public GameObject toastPrefab;
        
        // 单例
        public static HomeHUD Instance { get; private set; }
        
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        
        void Start()
        {
            // 绑定按钮事件
            if (backpackButton != null)
                backpackButton.onClick.AddListener(OnBackpackClick);
            
            if (settingsButton != null)
                settingsButton.onClick.AddListener(OnSettingsClick);
            
            // 默认隐藏提示
            HideNPCHint();
            HideExitHint();
        }
        
        #region 按钮事件
        
        private void OnBackpackClick()
        {
            // 打开背包界面
            Debug.Log("[HomeHUD] 打开背包");
            
            // 播放按钮动效
            UIAnimationHelper.PlayFluffyButtonClick(backpackButton.transform);
            
            // 调用背包系统
            ShowBackpackPanel();
        }
        
        /// <summary>
        /// 显示背包面板
        /// </summary>
        private void ShowBackpackPanel()
        {
            // 查找场景中的 BackpackPanel
            BackpackPanel backpackPanel = FindObjectOfType<BackpackPanel>(true);
            
            if (backpackPanel != null)
            {
                backpackPanel.Show();
                Debug.Log("[HomeHUD] 已显示背包面板");
            }
            else
            {
                Debug.LogWarning("[HomeHUD] 未找到 BackpackPanel，请确保场景中已添加");
                ShowToast("❌ 背包系统未配置");
            }
        }
        
        private void OnSettingsClick()
        {
            // 打开设置
            Debug.Log("[HomeHUD] 打开设置");
            UIAnimationHelper.PlayFluffyButtonClick(settingsButton.transform);
        }
        
        #endregion
        
        #region NPC提示
        
        /// <summary>
        /// 显示NPC对话提示
        /// </summary>
        public void ShowNPCHint(string npcName = "绵绵")
        {
            if (npcHintPanel != null)
            {
                npcHintPanel.SetActive(true);
                if (npcHintText != null)
                    npcHintText.text = $"💬 点击与{npcName}对话";
                
                // 播放弹出动画
                PlayHintAnimation(npcHintPanel.transform);
            }
        }
        
        /// <summary>
        /// 隐藏NPC提示
        /// </summary>
        public void HideNPCHint()
        {
            if (npcHintPanel != null)
                npcHintPanel.SetActive(false);
        }
        
        #endregion
        
        #region 离开提示
        
        /// <summary>
        /// 显示离开提示
        /// </summary>
        public void ShowExitHint()
        {
            if (exitHintPanel != null)
            {
                exitHintPanel.SetActive(true);
                if (exitHintText != null)
                    exitHintText.text = "🚪 点击离开家园";
                
                PlayHintAnimation(exitHintPanel.transform);
            }
        }
        
        /// <summary>
        /// 隐藏离开提示
        /// </summary>
        public void HideExitHint()
        {
            if (exitHintPanel != null)
                exitHintPanel.SetActive(false);
        }
        
        #endregion
        
        #region Toast提示
        
        /// <summary>
        /// 显示Toast提示
        /// </summary>
        public void ShowToast(string message, float duration = 2f)
        {
            if (toastPrefab != null && toastContainer != null)
            {
                GameObject toast = Instantiate(toastPrefab, toastContainer);
                
                TextMeshProUGUI text = toast.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                    text.text = message;
                
                // 播放动画
                UIAnimationHelper.PlayToastPopup(toast.transform, message);
                
                // 自动销毁
                Destroy(toast, duration + 0.5f);
            }
            else
            {
                Debug.Log($"[Toast] {message}");
            }
        }
        
        #endregion
        
        #region 动画
        
        private void PlayHintAnimation(Transform hintTransform)
        {
            // 使用DOTween或原生动画
            hintTransform.localScale = Vector3.zero;
            
            #if DG_TWEENING
            hintTransform.DOScale(1f, 0.3f).SetEase(DG.Tweening.Ease.OutBack);
            #else
            StartCoroutine(ScaleAnimationNative(hintTransform));
            #endif
        }
        
        private System.Collections.IEnumerator ScaleAnimationNative(Transform target)
        {
            float timer = 0;
            while (timer < 0.3f)
            {
                timer += Time.deltaTime;
                float t = timer / 0.3f;
                float scale = Mathf.Sin(t * Mathf.PI * 0.5f) * 1.1f;
                if (scale > 1) scale = 1;
                target.localScale = Vector3.one * scale;
                yield return null;
            }
            target.localScale = Vector3.one;
        }
        
        #endregion
    }
}
