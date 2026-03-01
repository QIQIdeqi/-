using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GeometryWarrior;

/// <summary>
/// 背包中的家园装扮页签 - 分部件换装界面
/// 替代原有的简单 OutfitPanel
/// </summary>
public class OutfitPanelNew : MonoBehaviour
{
    [Header("【面板动画】")]
    [Tooltip("面板内容容器")] public RectTransform contentPanel;
    [Tooltip("背景遮罩")] public CanvasGroup overlay;
    [Tooltip("关闭按钮")] public Button closeButton;
    
    [Header("【角色预览】")]
    [Tooltip("角色预览Image")] public Image characterPreview;
    [Tooltip("预览区旋转按钮-左")] public Button rotateLeftBtn;
    [Tooltip("预览区旋转按钮-右")] public Button rotateRightBtn;
    
    [Header("【部件分类标签】")]
    [Tooltip("分类标签容器")] public Transform categoryContainer;
    [Tooltip("分类标签预制体")] public GameObject categoryTabPrefab;
    
    [Header("【部件列表】")]
    [Tooltip("部件列表ScrollRect")] public ScrollRect partsScrollRect;
    [Tooltip("部件项预制体")] public GameObject partItemPrefab;
    [Tooltip("部件项容器")] public Transform partsContainer;
    
    [Header("【底部按钮】")]
    [Tooltip("保存按钮")] public Button saveButton;
    [Tooltip("一键卸下按钮")] public Button unequipAllButton;
    [Tooltip("一键换装按钮")] public Button quickEquipButton;
    
    [Header("【样式配置】")]
    [Tooltip("选中的标签颜色")] public Color selectedTabColor = new Color(1f, 0.72f, 0.77f);      // #FFB7C5
    [Tooltip("未选中标签颜色")] public Color unselectedTabColor = new Color(1f, 0.96f, 0.97f);     // #FFF5F7
    [Tooltip("选中的部件项颜色")] public Color selectedItemColor = new Color(0.66f, 0.9f, 0.81f);   // #A8E6CF
    [Tooltip("未选中部件项颜色")] public Color unselectedItemColor = Color.white;
    [Tooltip("锁定部件颜色")] public Color lockedItemColor = new Color(0.8f, 0.8f, 0.8f);
    
    // 数据
    private List<OutfitCategory> categories = new List<OutfitCategory> 
    { 
        OutfitCategory.Bow, 
        OutfitCategory.Hat, 
        OutfitCategory.Glasses,
        OutfitCategory.Scarf,
        OutfitCategory.Backpack
    };
    
    private Dictionary<OutfitCategory, string> categoryNames = new Dictionary<OutfitCategory, string>
    {
        { OutfitCategory.Bow, "🎀 头饰" },
        { OutfitCategory.Hat, "👒 帽子" },
        { OutfitCategory.Glasses, "👓 眼镜" },
        { OutfitCategory.Scarf, "📿 围巾" },
        { OutfitCategory.Backpack, "🎒 背饰" }
    };
    
    private OutfitCategory currentCategory;
    private List<GameObject> categoryTabs = new List<GameObject>();
    private List<GameObject> partItems = new List<GameObject>();
    
    // 当前预览的装备状态
    private Dictionary<OutfitCategory, OutfitPartData> previewEquipState = new Dictionary<OutfitCategory, OutfitPartData>();
    
    // 动画相关
    private bool useDOTween = false;
    
    void Start()
    {
        // 检测是否有 DOTween
        #if DG_TWEENING
        useDOTween = true;
        #endif
        
        closeButton?.onClick.AddListener(ClosePanel);
        saveButton?.onClick.AddListener(SaveOutfit);
        unequipAllButton?.onClick.AddListener(UnequipAll);
        quickEquipButton?.onClick.AddListener(QuickEquipRandom);
        
        rotateLeftBtn?.onClick.AddListener(() => RotateCharacter(-30f));
        rotateRightBtn?.onClick.AddListener(() => RotateCharacter(30f));
        
        InitializeCategoryTabs();
        LoadCurrentEquipState();
    }
    
    void OnEnable()
    {
        PlayOpenAnimation();
    }
    
    #region 动画（兼容DOTween和原生动画）
    
    private void PlayOpenAnimation()
    {
        if (useDOTween)
        {
            PlayOpenAnimationDOTween();
        }
        else
        {
            PlayOpenAnimationNative();
        }
    }
    
    private void PlayOpenAnimationDOTween()
    {
        #if DG_TWEENING
        // 重置状态
        contentPanel.anchoredPosition = new Vector2(0, -Screen.height);
        overlay.alpha = 0f;
        
        // 背景遮罩渐显
        DG.Tweening.DOTween.To(() => overlay.alpha, x => overlay.alpha = x, 0.6f, 0.3f)
            .SetEase(DG.Tweening.Ease.OutQuad);
        
        // 面板从底部滑入 + 弹性
        DG.Tweening.DOTween.To(() => contentPanel.anchoredPosition.y, 
            y => contentPanel.anchoredPosition = new Vector2(0, y), 0, 0.5f)
            .SetEase(DG.Tweening.Ease.OutBack)
            .OnComplete(() => {
                StartCoroutine(AnimateTabsEntry());
            });
        #endif
    }
    
    private void PlayOpenAnimationNative()
    {
        // 原生动画简化版
        contentPanel.anchoredPosition = Vector2.zero;
        overlay.alpha = 0.6f;
        StartCoroutine(AnimateTabsEntryNative());
    }
    
    private IEnumerator AnimateTabsEntry()
    {
        for (int i = 0; i < categoryTabs.Count; i++)
        {
            categoryTabs[i].transform.localScale = Vector3.zero;
            
            #if DG_TWEENING
            categoryTabs[i].transform.DOScale(1f, 0.3f)
                .SetEase(DG.Tweening.Ease.OutBack);
            #else
            categoryTabs[i].transform.localScale = Vector3.one;
            #endif
            
            yield return new WaitForSeconds(0.05f);
        }
        
        RefreshPartsList();
    }
    
    private IEnumerator AnimateTabsEntryNative()
    {
        for (int i = 0; i < categoryTabs.Count; i++)
        {
            categoryTabs[i].transform.localScale = Vector3.zero;
            yield return new WaitForSeconds(0.05f);
            
            // 简单的缩放动画
            float timer = 0;
            while (timer < 0.3f)
            {
                timer += Time.deltaTime;
                float t = timer / 0.3f;
                float scale = Mathf.Lerp(0, 1, Mathf.Sin(t * Mathf.PI * 0.5f));
                categoryTabs[i].transform.localScale = Vector3.one * scale;
                yield return null;
            }
            categoryTabs[i].transform.localScale = Vector3.one;
        }
        
        RefreshPartsList();
    }
    
    private void PlayCloseAnimation(System.Action onComplete)
    {
        if (useDOTween)
        {
            #if DG_TWEENING
            DG.Tweening.DOTween.To(() => contentPanel.anchoredPosition.y, 
                y => contentPanel.anchoredPosition = new Vector2(0, y), -Screen.height, 0.3f)
                .SetEase(DG.Tweening.Ease.InQuad);
            
            DG.Tweening.DOTween.To(() => overlay.alpha, x => overlay.alpha = x, 0f, 0.3f)
                .SetEase(DG.Tweening.Ease.InQuad)
                .OnComplete(() => onComplete?.Invoke());
            #endif
        }
        else
        {
            // 原生关闭动画
            StartCoroutine(CloseAnimationNative(onComplete));
        }
    }
    
    private IEnumerator CloseAnimationNative(System.Action onComplete)
    {
        float timer = 0;
        Vector2 startPos = contentPanel.anchoredPosition;
        float startAlpha = overlay.alpha;
        
        while (timer < 0.3f)
        {
            timer += Time.deltaTime;
            float t = timer / 0.3f;
            
            contentPanel.anchoredPosition = Vector2.Lerp(startPos, new Vector2(0, -Screen.height), t);
            overlay.alpha = Mathf.Lerp(startAlpha, 0, t);
            
            yield return null;
        }
        
        onComplete?.Invoke();
    }
    
    #endregion
    
    #region 分类标签
    
    private void InitializeCategoryTabs()
    {
        // 清除旧标签
        foreach (var tab in categoryTabs)
        {
            if (tab != null) Destroy(tab);
        }
        categoryTabs.Clear();
        
        // 创建新标签
        for (int i = 0; i < categories.Count; i++)
        {
            var category = categories[i];
            var tabObj = Instantiate(categoryTabPrefab, categoryContainer);
            categoryTabs.Add(tabObj);
            
            // 设置标签文本
            var text = tabObj.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null) text.text = categoryNames[category];
            
            // 设置按钮事件
            var button = tabObj.GetComponent<Button>();
            if (button != null)
            {
                var captureCategory = category;
                button.onClick.AddListener(() => SelectCategory(captureCategory));
            }
            
            // 初始状态
            UpdateTabVisual(tabObj, i == 0);
        }
        
        // 默认选中第一个
        if (categories.Count > 0) SelectCategory(categories[0]);
    }
    
    private void SelectCategory(OutfitCategory category)
    {
        currentCategory = category;
        
        // 更新标签视觉
        for (int i = 0; i < categoryTabs.Count; i++)
        {
            UpdateTabVisual(categoryTabs[i], categories[i] == category);
        }
        
        // 刷新部件列表
        RefreshPartsList();
    }
    
    private void UpdateTabVisual(GameObject tabObj, bool isSelected)
    {
        var image = tabObj.GetComponent<Image>();
        if (image != null)
        {
            image.color = isSelected ? selectedTabColor : unselectedTabColor;
            
            // 选中时放大，未选中恢复
            if (useDOTween)
            {
                #if DG_TWEENING
                tabObj.transform.DOScale(isSelected ? 1.1f : 1f, 0.2f)
                    .SetEase(DG.Tweening.Ease.OutQuad);
                #endif
            }
            else
            {
                tabObj.transform.localScale = isSelected ? Vector3.one * 1.1f : Vector3.one;
            }
        }
        
        // 边框效果
        var outline = tabObj.GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = isSelected;
        }
    }
    
    #endregion
    
    #region 部件列表
    
    private void RefreshPartsList()
    {
        // 清除旧项
        foreach (var item in partItems)
        {
            if (item != null) Destroy(item);
        }
        partItems.Clear();
        
        // 获取该类型的所有部件
        var parts = OutfitManager.Instance?.GetPartsByCategory(currentCategory);
        if (parts == null) return;
        
        for (int i = 0; i < parts.Count; i++)
        {
            CreatePartItem(parts[i], i);
        }
    }
    
    private void CreatePartItem(OutfitPartData part, int index)
    {
        var itemObj = Instantiate(partItemPrefab, partsContainer);
        partItems.Add(itemObj);
        
        // 设置图标
        var iconImage = itemObj.transform.Find("Icon")?.GetComponent<Image>();
        if (iconImage != null && part.icon != null)
        {
            iconImage.sprite = part.icon;
        }
        
        // 设置名称 - 使用 partName 字段
        var nameText = itemObj.transform.Find("Name")?.GetComponent<TextMeshProUGUI>();
        if (nameText != null) nameText.text = part.partName;
        
        // 检查是否已装备
        bool isEquipped = previewEquipState.ContainsKey(currentCategory) 
            && previewEquipState[currentCategory] == part;
        
        // 检查是否解锁
        bool isUnlocked = OutfitManager.Instance?.IsPartUnlocked(part) ?? false;
        
        // 设置状态标签
        var statusObj = itemObj.transform.Find("Status")?.gameObject;
        var statusText = statusObj?.GetComponent<TextMeshProUGUI>();
        if (statusText != null)
        {
            if (isEquipped)
            {
                statusText.text = "✅ 已装备";
                statusObj.SetActive(true);
            }
            else if (!isUnlocked)
            {
                statusText.text = "🔒 未解锁";
                statusObj.SetActive(true);
            }
            else
            {
                statusObj.SetActive(false);
            }
        }
        
        // 设置背景色
        var bgImage = itemObj.GetComponent<Image>();
        if (bgImage != null)
        {
            if (isEquipped)
                bgImage.color = selectedItemColor;
            else if (!isUnlocked)
                bgImage.color = lockedItemColor;
            else
                bgImage.color = unselectedItemColor;
        }
        
        // 点击事件
        var button = itemObj.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => OnPartItemClick(part, itemObj));
        }
        
        // 入场动画
        StartCoroutine(ItemEntryAnimation(itemObj, index));
    }
    
    private IEnumerator ItemEntryAnimation(GameObject itemObj, int index)
    {
        itemObj.transform.localScale = Vector3.zero;
        yield return new WaitForSeconds(index * 0.03f);
        
        if (useDOTween)
        {
            #if DG_TWEENING
            itemObj.transform.DOScale(1f, 0.3f)
                .SetEase(DG.Tweening.Ease.OutBack);
            #endif
        }
        else
        {
            // 原生缩放动画
            float timer = 0;
            while (timer < 0.3f)
            {
                timer += Time.deltaTime;
                float t = timer / 0.3f;
                float scale = Mathf.Lerp(0, 1, Mathf.Sin(t * Mathf.PI * 0.5f) * 1.2f);
                if (scale > 1) scale = 1;
                itemObj.transform.localScale = Vector3.one * scale;
                yield return null;
            }
            itemObj.transform.localScale = Vector3.one;
        }
    }
    
    private void OnPartItemClick(OutfitPartData part, GameObject itemObj)
    {
        // 检查是否解锁
        if (!(OutfitManager.Instance?.IsPartUnlocked(part) ?? false))
        {
            // 播放锁定提示动画 - 抖动
            StartCoroutine(ShakeAnimation(itemObj));
            return;
        }
        
        // 切换装备状态
        if (previewEquipState.ContainsKey(currentCategory) && previewEquipState[currentCategory] == part)
        {
            // 如果已装备，则卸下
            previewEquipState.Remove(currentCategory);
        }
        else
        {
            // 装备
            previewEquipState[currentCategory] = part;
        }
        
        // 播放点击动效
        StartCoroutine(PunchScaleAnimation(itemObj));
        
        // 刷新列表显示
        RefreshPartsList();
        
        // 更新预览
        UpdateCharacterPreview();
    }
    
    private IEnumerator ShakeAnimation(GameObject target)
    {
        Vector3 originalPos = target.transform.position;
        float timer = 0;
        
        while (timer < 0.3f)
        {
            timer += Time.deltaTime;
            float offset = Mathf.Sin(timer * 50) * 5f;
            target.transform.position = originalPos + Vector3.right * offset;
            yield return null;
        }
        
        target.transform.position = originalPos;
    }
    
    private IEnumerator PunchScaleAnimation(GameObject target)
    {
        Vector3 originalScale = target.transform.localScale;
        float timer = 0;
        
        while (timer < 0.2f)
        {
            timer += Time.deltaTime;
            float t = timer / 0.2f;
            float scale = 1 + Mathf.Sin(t * Mathf.PI) * 0.1f;
            target.transform.localScale = originalScale * scale;
            yield return null;
        }
        
        target.transform.localScale = originalScale;
    }
    
    #endregion
    
    #region 角色预览
    
    private void UpdateCharacterPreview()
    {
        // 这里根据 previewEquipState 更新角色预览图
        // 可以生成一个组合后的精灵，或者使用PlayerOutfitApplier的逻辑
        
        // 通知外部更新
        OnPreviewChanged?.Invoke(previewEquipState);
    }
    
    private void RotateCharacter(float angle)
    {
        // 旋转预览角色
        if (characterPreview != null)
        {
            if (useDOTween)
            {
                #if DG_TWEENING
                characterPreview.transform.DORotate(new Vector3(0, 0, angle), 0.3f, DG.Tweening.RotateMode.LocalAxisAdd)
                    .SetEase(DG.Tweening.Ease.OutQuad);
                #endif
            }
            else
            {
                characterPreview.transform.Rotate(0, 0, angle);
            }
        }
    }
    
    public delegate void PreviewChangedHandler(Dictionary<OutfitCategory, OutfitPartData> state);
    public event PreviewChangedHandler OnPreviewChanged;
    
    #endregion
    
    #region 功能按钮
    
    private void LoadCurrentEquipState()
    {
        // 从 OutfitManager 加载当前装备状态到预览
        previewEquipState.Clear();
        
        if (OutfitManager.Instance != null)
        {
            foreach (var category in categories)
            {
                var equipped = OutfitManager.Instance.GetEquippedPart(category);
                if (equipped != null)
                {
                    previewEquipState[category] = equipped;
                }
            }
        }
        
        UpdateCharacterPreview();
    }
    
    private void SaveOutfit()
    {
        // 保存预览状态到 OutfitManager
        if (OutfitManager.Instance != null)
        {
            foreach (var kvp in previewEquipState)
            {
                OutfitManager.Instance.EquipPart(kvp.Value);
            }
            
            // 卸下未在预览中的部件
            foreach (var category in categories)
            {
                if (!previewEquipState.ContainsKey(category))
                {
                    OutfitManager.Instance.UnequipPart(category);
                }
            }
        }
        
        // 播放保存成功动效
        StartCoroutine(ButtonPunchAnimation(saveButton.transform));
        ShowToast("✨ 装扮已保存！");
    }
    
    private void UnequipAll()
    {
        previewEquipState.Clear();
        RefreshPartsList();
        UpdateCharacterPreview();
        
        // 播放卸下动效
        StartCoroutine(ButtonPunchAnimation(unequipAllButton.transform));
    }
    
    private void QuickEquipRandom()
    {
        // 一键随机装备（只选已解锁的）
        foreach (var category in categories)
        {
            var parts = OutfitManager.Instance?.GetPartsByCategory(category);
            if (parts != null && parts.Count > 0)
            {
                // 过滤已解锁的
                var unlocked = parts.FindAll(p => OutfitManager.Instance.IsPartUnlocked(p));
                if (unlocked.Count > 0)
                {
                    // 随机选择一个
                    var randomPart = unlocked[Random.Range(0, unlocked.Count)];
                    previewEquipState[category] = randomPart;
                }
            }
        }
        
        RefreshPartsList();
        UpdateCharacterPreview();
        
        // 播放动效
        StartCoroutine(ButtonPunchAnimation(quickEquipButton.transform));
        ShowToast("🎲 随机换装完成！");
    }
    
    private IEnumerator ButtonPunchAnimation(Transform button)
    {
        Vector3 originalScale = button.localScale;
        float timer = 0;
        
        while (timer < 0.3f)
        {
            timer += Time.deltaTime;
            float t = timer / 0.3f;
            float scale = 1 + Mathf.Sin(t * Mathf.PI * 2) * 0.15f;
            button.localScale = originalScale * scale;
            yield return null;
        }
        
        button.localScale = originalScale;
    }
    
    private void ClosePanel()
    {
        PlayCloseAnimation(() => {
            gameObject.SetActive(false);
        });
    }
    
    #endregion
    
    #region 工具方法
    
    private void ShowToast(string message)
    {
        Debug.Log(message);
        // 可以实现一个Toast系统
    }
    
    #endregion
}
