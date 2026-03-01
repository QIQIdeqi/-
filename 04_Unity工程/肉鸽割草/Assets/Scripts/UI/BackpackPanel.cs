using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FluffyGeometry.UI;

namespace GeometryWarrior
{
    /// <summary>
    /// 背包主界面 - 整合主角装备和家园装扮两个页签
    /// </summary>
    public class BackpackPanel : MonoBehaviour
    {
        public static BackpackPanel Instance { get; private set; }
        
        [Header("【面板引用】")]
        [Tooltip("面板内容")] public RectTransform contentPanel;
        [Tooltip("背景遮罩")] public CanvasGroup overlay;
        [Tooltip("关闭按钮")] public Button closeButton;
        
        [Header("【页签】")]
        [Tooltip("主角装备页签")] public Button playerEquipTab;
        [Tooltip("家园装扮页签")] public Button homeOutfitTab;
        [Tooltip("主角装备页")] public GameObject playerEquipPage;
        [Tooltip("家园装扮页")] public GameObject homeOutfitPage;
        
        [Header("【页签样式】")]
        [Tooltip("选中颜色")] public Color selectedTabColor = new Color(1f, 0.72f, 0.77f); // #FFB7C5
        [Tooltip("未选中颜色")] public Color unselectedTabColor = new Color(1f, 0.96f, 0.97f); // #FFF5F7
        
        [Header("【家园装扮页】")]
        [Tooltip("OutfitPanelNew 预制体")] public GameObject outfitPanelPrefab;
        
        private bool isPlayerEquipActive = true;
        private GameObject currentOutfitPanel;
        
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
            // 绑定按钮
            if (closeButton != null)
                closeButton.onClick.AddListener(Hide);
            
            if (playerEquipTab != null)
                playerEquipTab.onClick.AddListener(() => SwitchTab(true));
            
            if (homeOutfitTab != null)
                homeOutfitTab.onClick.AddListener(() => SwitchTab(false));
            
            // 默认显示主角装备页
            SwitchTab(true);
        }
        
        void OnEnable()
        {
            PlayOpenAnimation();
        }
        
        #region 显示/隐藏
        
        // 背包按钮引用（可选，用于回调）
        private BackpackButton backpackButton;
        
        /// <summary>
        /// 初始化（由BackpackButton调用）
        /// </summary>
        public void Initialize(BackpackButton button)
        {
            backpackButton = button;
        }
        
        /// <summary>
        /// 显示背包界面
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }
        
        /// <summary>
        /// 显示背包界面（指定默认页签）
        /// </summary>
        public void Show(int defaultTab)
        {
            gameObject.SetActive(true);
            // defaultTab: 0=主角装备, 1=家园装扮
            SwitchTab(defaultTab == 0);
        }
        
        /// <summary>
        /// 隐藏背包界面
        /// </summary>
        public void Hide()
        {
            PlayCloseAnimation(() => {
                gameObject.SetActive(false);
                // 通知背包按钮
                if (backpackButton != null)
                {
                    backpackButton.OnPanelClosed();
                }
            });
        }
        
        /// <summary>
        /// 重新打开（用于家具编辑后返回）
        /// </summary>
        public void Reopen()
        {
            // 直接显示，不播放动画
            gameObject.SetActive(true);
            // 确保在家园装扮页
            SwitchTab(false);
        }
        
        #endregion
        
        #region 页签切换
        
        /// <summary>
        /// 切换页签
        /// </summary>
        private void SwitchTab(bool showPlayerEquip)
        {
            if (isPlayerEquipActive == showPlayerEquip) return;
            
            isPlayerEquipActive = showPlayerEquip;
            
            // 更新页签视觉
            UpdateTabVisual(playerEquipTab, showPlayerEquip);
            UpdateTabVisual(homeOutfitTab, !showPlayerEquip);
            
            // 切换页面显示
            if (playerEquipPage != null)
                playerEquipPage.SetActive(showPlayerEquip);
            
            if (homeOutfitPage != null)
            {
                homeOutfitPage.SetActive(!showPlayerEquip);
                
                // 如果切换到家园装扮页，确保OutfitPanel已创建
                if (!showPlayerEquip && currentOutfitPanel == null && outfitPanelPrefab != null)
                {
                    CreateOutfitPanel();
                }
            }
            
            // 播放切换动效
            PlayTabSwitchAnimation();
        }
        
        private void UpdateTabVisual(Button tabButton, bool isSelected)
        {
            if (tabButton == null) return;
            
            Image image = tabButton.GetComponent<Image>();
            if (image != null)
            {
                image.color = isSelected ? selectedTabColor : unselectedTabColor;
            }
            
            // 缩放效果
            #if DG_TWEENING
            tabButton.transform.DOScale(isSelected ? 1.1f : 1f, 0.2f)
                .SetEase(DG.Tweening.Ease.OutQuad);
            #else
            tabButton.transform.localScale = isSelected ? Vector3.one * 1.1f : Vector3.one;
            #endif
        }
        
        private void CreateOutfitPanel()
        {
            if (outfitPanelPrefab == null || homeOutfitPage == null) return;
            
            currentOutfitPanel = Instantiate(outfitPanelPrefab, homeOutfitPage.transform);
            
            // 配置RectTransform填满父物体
            RectTransform rect = currentOutfitPanel.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            currentOutfitPanel.SetActive(true);
        }
        
        #endregion
        
        #region 动画
        
        private void PlayOpenAnimation()
        {
            if (contentPanel == null || overlay == null) return;
            
            #if DG_TWEENING
            // DOTween动画
            contentPanel.anchoredPosition = new Vector2(0, -Screen.height);
            overlay.alpha = 0f;
            
            overlay.DOFade(0.6f, 0.3f).SetEase(DG.Tweening.Ease.OutQuad);
            contentPanel.DOAnchorPosY(0, 0.5f)
                .SetEase(DG.Tweening.Ease.OutBack)
                .OnComplete(() => {
                    // 页签依次弹出
                    PlayTabsEntryAnimation();
                });
            #else
            // 原生动画
            StartCoroutine(OpenAnimationNative());
            #endif
        }
        
        private System.Collections.IEnumerator OpenAnimationNative()
        {
            contentPanel.anchoredPosition = new Vector2(0, -Screen.height);
            overlay.alpha = 0f;
            
            float timer = 0;
            float duration = 0.5f;
            
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = timer / duration;
                
                // 弹性缓出
                float elastic = Mathf.Sin(t * Mathf.PI * (0.2f + 2.5f * t * t * t)) * Mathf.Pow(1f - t, 2.2f) + t;
                
                contentPanel.anchoredPosition = new Vector2(0, Mathf.Lerp(-Screen.height, 0, elastic));
                overlay.alpha = Mathf.Lerp(0, 0.6f, t);
                
                yield return null;
            }
            
            contentPanel.anchoredPosition = Vector2.zero;
            overlay.alpha = 0.6f;
            
            PlayTabsEntryAnimation();
        }
        
        private void PlayTabsEntryAnimation()
        {
            Button[] tabs = new Button[] { playerEquipTab, homeOutfitTab };
            
            for (int i = 0; i < tabs.Length; i++)
            {
                if (tabs[i] == null) continue;
                
                #if DG_TWEENING
                tabs[i].transform.localScale = Vector3.zero;
                tabs[i].transform.DOScale(1f, 0.3f)
                    .SetEase(DG.Tweening.Ease.OutBack)
                    .SetDelay(i * 0.1f);
                #else
                StartCoroutine(TabEntryNative(tabs[i].transform, i * 0.1f));
                #endif
            }
        }
        
        private System.Collections.IEnumerator TabEntryNative(Transform tab, float delay)
        {
            tab.localScale = Vector3.zero;
            yield return new WaitForSeconds(delay);
            
            float timer = 0;
            float duration = 0.3f;
            
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = timer / duration;
                float scale = Mathf.Sin(t * Mathf.PI * 0.5f) * 1.1f;
                if (scale > 1) scale = 1;
                tab.localScale = Vector3.one * scale;
                yield return null;
            }
            
            tab.localScale = Vector3.one;
        }
        
        private void PlayTabSwitchAnimation()
        {
            // 页面切换动效
            GameObject activePage = isPlayerEquipActive ? playerEquipPage : homeOutfitPage;
            if (activePage != null)
            {
                activePage.transform.localScale = Vector3.one * 0.95f;
                
                #if DG_TWEENING
                activePage.transform.DOScale(1f, 0.3f)
                    .SetEase(DG.Tweening.Ease.OutBack);
                #else
                StartCoroutine(PageSwitchNative(activePage.transform));
                #endif
            }
        }
        
        private System.Collections.IEnumerator PageSwitchNative(Transform page)
        {
            float timer = 0;
            float duration = 0.3f;
            Vector3 startScale = Vector3.one * 0.95f;
            
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = timer / duration;
                float scale = Mathf.Lerp(0.95f, 1f, Mathf.Sin(t * Mathf.PI * 0.5f));
                page.localScale = Vector3.one * scale;
                yield return null;
            }
            
            page.localScale = Vector3.one;
        }
        
        private void PlayCloseAnimation(System.Action onComplete)
        {
            if (contentPanel == null || overlay == null)
            {
                onComplete?.Invoke();
                return;
            }
            
            #if DG_TWEENING
            contentPanel.DOAnchorPosY(-Screen.height, 0.3f)
                .SetEase(DG.Tweening.Ease.InQuad);
            
            overlay.DOFade(0f, 0.3f)
                .SetEase(DG.Tweening.Ease.InQuad)
                .OnComplete(() => onComplete?.Invoke());
            #else
            StartCoroutine(CloseAnimationNative(onComplete));
            #endif
        }
        
        private System.Collections.IEnumerator CloseAnimationNative(System.Action onComplete)
        {
            float timer = 0;
            float duration = 0.3f;
            Vector2 startPos = contentPanel.anchoredPosition;
            float startAlpha = overlay.alpha;
            
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = timer / duration;
                
                contentPanel.anchoredPosition = Vector2.Lerp(startPos, new Vector2(0, -Screen.height), t * t);
                overlay.alpha = Mathf.Lerp(startAlpha, 0, t);
                
                yield return null;
            }
            
            onComplete?.Invoke();
        }
        
        #endregion
    }
}
