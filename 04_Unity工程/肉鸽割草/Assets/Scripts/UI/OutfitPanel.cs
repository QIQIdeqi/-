using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GeometryWarrior
{
    /// <summary>
    /// 装扮界面 - 分部件换装系统
    /// </summary>
    public class OutfitPanel : MonoBehaviour
    {
        [Header("【主面板】")]
        [Tooltip("装扮界面的主面板GameObject")]
        [SerializeField] private GameObject panel;
        
        [Tooltip("关闭装扮界面的按钮")]
        [SerializeField] private Button closeButton;
        
        [Header("【分类标签】")]
        [Tooltip("放置分类标签的容器（如Horizontal Layout Group）")]
        [SerializeField] private Transform categoryTabContainer;
        
        [Tooltip("分类标签按钮预制体")]
        [SerializeField] private GameObject categoryTabPrefab;
        
        [Header("【部件列表】")]
        [Tooltip("放置部件列表项的容器（如Grid Layout Group）")]
        [SerializeField] private Transform partListContainer;
        
        [Tooltip("部件列表项预制体")]
        [SerializeField] private GameObject partItemPrefab;
        
        [Header("【预览区域】")]
        [Tooltip("玩家预览图（显示角色基础形象）")]
        [SerializeField] private Image playerPreviewImage;
        
        [Tooltip("放置预览部件的容器")]
        [SerializeField] private Transform previewPartsContainer;
        
        [Tooltip("预览部件预制体（用于显示装备效果）")]
        [SerializeField] private GameObject previewPartPrefab;
        
        [Header("【当前选择信息】")]
        [Tooltip("显示选中部件名称的文本")]
        [SerializeField] private TextMeshProUGUI selectedPartNameText;
        
        [Tooltip("显示选中部件描述的文本")]
        [SerializeField] private TextMeshProUGUI selectedPartDescText;
        
        [Tooltip("装备选中部件的按钮")]
        [SerializeField] private Button equipButton;
        
        [Tooltip("卸下当前类别部件的按钮")]
        [SerializeField] private Button unequipButton;
        
        // 当前选中的类别
        private OutfitCategory currentCategory = OutfitCategory.Bow;
        
        // 当前选中的部件
        private OutfitPartData selectedPart;
        
        // 生成的UI项列表
        private List<OutfitItemUI> outfitItems = new List<OutfitItemUI>();
        private List<GameObject> categoryTabs = new List<GameObject>();
        private Dictionary<OutfitCategory, GameObject> previewParts = new Dictionary<OutfitCategory, GameObject>();
        
        // 是否可见
        public bool IsVisible => panel != null && panel.activeSelf;
        
        private void Start()
        {
            // 绑定按钮事件
            if (closeButton != null)
                closeButton.onClick.AddListener(Hide);
            
            if (equipButton != null)
                equipButton.onClick.AddListener(OnEquipClick);
            
            if (unequipButton != null)
                unequipButton.onClick.AddListener(OnUnequipClick);
            
            // 初始隐藏
            Hide();
        }
        
        /// <summary>
        /// 显示装扮界面
        /// </summary>
        public void Show()
        {
            panel.SetActive(true);
            
            // 创建分类标签
            CreateCategoryTabs();
            
            // 刷新部件列表
            RefreshPartList(currentCategory);
            
            // 更新预览
            UpdatePreview();
            
            // 暂停玩家移动（可选）
            var player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.enabled = false;
            }
        }
        
        /// <summary>
        /// 隐藏装扮界面
        /// </summary>
        public void Hide()
        {
            panel.SetActive(false);
            
            // 恢复玩家移动
            var player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.enabled = true;
            }
            
            // 通知NPC界面已关闭
            var npc = FindObjectOfType<HomeNPC>();
            if (npc != null)
            {
                npc.OnOutfitPanelClosed();
            }
        }
        
        /// <summary>
        /// 创建分类标签
        /// </summary>
        private void CreateCategoryTabs()
        {
            // 清除旧标签
            foreach (var tab in categoryTabs)
            {
                Destroy(tab);
            }
            categoryTabs.Clear();
            
            // 创建新标签
            var categories = System.Enum.GetValues(typeof(OutfitCategory));
            foreach (OutfitCategory category in categories)
            {
                GameObject tabObj = Instantiate(categoryTabPrefab, categoryTabContainer);
                
                // 设置标签文本
                TextMeshProUGUI text = tabObj.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                {
                    text.text = GetCategoryName(category);
                }
                
                // 绑定点击事件
                Button button = tabObj.GetComponent<Button>();
                if (button != null)
                {
                    OutfitCategory cat = category; // 捕获变量
                    button.onClick.AddListener(() => OnCategoryTabClick(cat));
                }
                
                // 设置选中状态
                SetTabSelected(tabObj, category == currentCategory);
                
                categoryTabs.Add(tabObj);
            }
        }
        
        /// <summary>
        /// 点击分类标签
        /// </summary>
        private void OnCategoryTabClick(OutfitCategory category)
        {
            currentCategory = category;
            
            // 更新标签选中状态
            for (int i = 0; i < categoryTabs.Count; i++)
            {
                SetTabSelected(categoryTabs[i], i == (int)category);
            }
            
            // 刷新部件列表
            RefreshPartList(category);
        }
        
        /// <summary>
        /// 设置标签选中状态
        /// </summary>
        private void SetTabSelected(GameObject tab, bool selected)
        {
            Image image = tab.GetComponent<Image>();
            if (image != null)
            {
                image.color = selected ? Color.yellow : Color.white;
            }
        }
        
        /// <summary>
        /// 刷新部件列表
        /// </summary>
        private void RefreshPartList(OutfitCategory category)
        {
            // 清除旧列表
            foreach (var item in outfitItems)
            {
                if (item != null)
                    Destroy(item.gameObject);
            }
            outfitItems.Clear();
            
            if (OutfitManager.Instance == null) return;
            
            // 获取该类别下的所有部件
            var parts = OutfitManager.Instance.GetPartsByCategory(category);
            
            // 创建列表项
            foreach (var part in parts)
            {
                if (part == null) continue;
                
                GameObject itemObj = Instantiate(partItemPrefab, partListContainer);
                
                OutfitItemUI itemUI = itemObj.GetComponent<OutfitItemUI>();
                if (itemUI != null)
                {
                    bool isUnlocked = OutfitManager.Instance.IsPartUnlocked(part);
                    bool isEquipped = OutfitManager.Instance.GetEquippedPart(category) == part;
                    
                    itemUI.Setup(part, isUnlocked, isEquipped, OnPartItemClick);
                    outfitItems.Add(itemUI);
                }
            }
        }
        
        /// <summary>
        /// 点击部件项
        /// </summary>
        private void OnPartItemClick(OutfitPartData part)
        {
            selectedPart = part;
            
            // 更新选中信息
            if (selectedPartNameText != null)
                selectedPartNameText.text = part.partName;
            
            if (selectedPartDescText != null)
                selectedPartDescText.text = part.description;
            
            // 更新按钮状态
            bool isUnlocked = OutfitManager.Instance.IsPartUnlocked(part);
            bool isEquipped = OutfitManager.Instance.GetEquippedPart(part.category) == part;
            
            if (equipButton != null)
            {
                equipButton.gameObject.SetActive(isUnlocked && !isEquipped);
            }
            
            if (unequipButton != null)
            {
                unequipButton.gameObject.SetActive(isEquipped);
            }
        }
        
        /// <summary>
        /// 点击装备按钮
        /// </summary>
        private void OnEquipClick()
        {
            if (selectedPart == null || OutfitManager.Instance == null) return;
            
            OutfitManager.Instance.EquipPart(selectedPart);
            
            // 刷新显示
            RefreshPartList(currentCategory);
            UpdatePreview();
            
            // 更新按钮状态
            if (equipButton != null)
                equipButton.gameObject.SetActive(false);
            
            if (unequipButton != null)
                unequipButton.gameObject.SetActive(true);
        }
        
        /// <summary>
        /// 点击卸下按钮
        /// </summary>
        private void OnUnequipClick()
        {
            if (selectedPart == null || OutfitManager.Instance == null) return;
            
            OutfitManager.Instance.UnequipPart(selectedPart.category);
            
            // 刷新显示
            RefreshPartList(currentCategory);
            UpdatePreview();
            
            // 更新按钮状态
            if (equipButton != null)
                equipButton.gameObject.SetActive(true);
            
            if (unequipButton != null)
                unequipButton.gameObject.SetActive(false);
        }
        
        /// <summary>
        /// 更新预览显示
        /// </summary>
        private void UpdatePreview()
        {
            // 清除旧预览
            foreach (var kvp in previewParts)
            {
                Destroy(kvp.Value);
            }
            previewParts.Clear();
            
            if (OutfitManager.Instance == null) return;
            
            // 创建新预览
            var equippedParts = OutfitManager.Instance.GetAllEquippedParts();
            foreach (var kvp in equippedParts)
            {
                OutfitPartData part = kvp.Value;
                if (part != null && part.partSprite != null)
                {
                    GameObject partObj = Instantiate(previewPartPrefab, previewPartsContainer);
                    
                    Image image = partObj.GetComponent<Image>();
                    if (image != null)
                    {
                        image.sprite = part.partSprite;
                    }
                    
                    // 应用偏移和旋转
                    RectTransform rectTransform = partObj.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        rectTransform.anchoredPosition = part.offset * 100f; // 转换为UI坐标
                        rectTransform.localRotation = Quaternion.Euler(0, 0, part.rotation);
                        rectTransform.localScale = part.scale;
                    }
                    
                    previewParts[kvp.Key] = partObj;
                }
            }
        }
        
        /// <summary>
        /// 获取类别名称
        /// </summary>
        private string GetCategoryName(OutfitCategory category)
        {
            switch (category)
            {
                case OutfitCategory.Bow: return "蝴蝶结";
                case OutfitCategory.Hat: return "帽子";
                case OutfitCategory.Glasses: return "眼镜";
                case OutfitCategory.Scarf: return "围巾";
                case OutfitCategory.Backpack: return "背包";
                case OutfitCategory.Shoes: return "鞋子";
                case OutfitCategory.Special: return "特殊";
                default: return category.ToString();
            }
        }
    }
}
