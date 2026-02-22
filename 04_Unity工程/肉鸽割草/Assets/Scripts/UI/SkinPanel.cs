using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GeometryWarrior
{
    /// <summary>
    /// 皮肤界面 - 显示和选择皮肤
    /// 流程：显示列表 -> 点击皮肤 -> 显示预览 -> 装备/取消 -> 返回列表
    /// </summary>
    public class SkinPanel : MonoBehaviour
    {
        [Header("[UI组件]")]
        [SerializeField] private GameObject skinPanel;           // 皮肤界面面板
        [SerializeField] private Transform skinListContainer;    // 皮肤列表容器
        [SerializeField] private GameObject skinItemPrefab;      // 皮肤项预制体
        [SerializeField] private Button closeButton;             // 关闭按钮（关闭整个皮肤界面）
        [SerializeField] private Button openButton;              // 打开按钮（在主菜单上）
        
        [Header("[预览面板]")]
        [SerializeField] private GameObject previewPanel;        // 预览面板（单独控制显示/隐藏）
        [SerializeField] private Image previewImage;             // 预览图
        [SerializeField] private Text previewNameLegacy;         // 预览名称 (Legacy Text)
        [SerializeField] private TextMeshProUGUI previewNameTMP; // 预览名称 (TextMeshPro)
        [SerializeField] private Text previewDescriptionLegacy;  // 预览描述 (Legacy Text)
        [SerializeField] private TextMeshProUGUI previewDescriptionTMP; // 预览描述 (TextMeshPro)
        [SerializeField] private Button equipButton;             // 装备/替换按钮
        [SerializeField] private Button unlockButton;            // 解锁按钮
        [SerializeField] private Button cancelButton;            // 取消按钮（关闭预览面板）
        [SerializeField] private Text priceTextLegacy;           // 价格文本 (Legacy Text)
        [SerializeField] private TextMeshProUGUI priceTextTMP;   // 价格文本 (TextMeshPro)
        
        private List<SkinItemUI> skinItems = new List<SkinItemUI>();
        private SkinData selectedSkin;                           // 当前选中的皮肤
        
        private void Awake()
        {
            // 如果没有指定面板，使用当前物体
            if (skinPanel == null)
            {
                skinPanel = gameObject;
            }
        }
        
        private void Start()
        {
            // 绑定按钮事件
            if (openButton != null)
            {
                openButton.onClick.AddListener(OnOpenButtonClick);
            }
            else
            {
                Debug.LogError("[SkinPanel] openButton is null!");
            }
            
            if (closeButton != null)
            {
                // 确保关闭按钮和打开按钮不是同一个
                if (closeButton != openButton)
                {
                    closeButton.onClick.AddListener(Hide);
                }
            }
            
            if (equipButton != null)
                equipButton.onClick.AddListener(OnEquipClick);
            if (unlockButton != null)
                unlockButton.onClick.AddListener(OnUnlockClick);
            if (cancelButton != null)
                cancelButton.onClick.AddListener(OnCancelClick);
            
            // 初始隐藏整个面板
            Hide();
        }
        
        /// <summary>
        /// 打开按钮点击回调
        /// </summary>
        private void OnOpenButtonClick()
        {
            Show();
        }
        
        /// <summary>
        /// 显示皮肤界面（只显示列表，不显示预览）
        /// </summary>
        public void Show()
        {
            // 激活主面板
            GameObject panel = skinPanel != null ? skinPanel : gameObject;
            panel.SetActive(true);
            
            // 隐藏预览面板
            HidePreview();
            
            // 刷新皮肤列表
            RefreshSkinList();
        }
        
        /// <summary>
        /// 隐藏皮肤界面（整个关闭）
        /// </summary>
        public void Hide()
        {
            GameObject panel = skinPanel != null ? skinPanel : gameObject;
            panel.SetActive(false);
        }
        
        /// <summary>
        /// 显示预览面板
        /// </summary>
        private void ShowPreview()
        {
            Debug.Log($"[SkinPanel] ShowPreview() called, previewPanel={previewPanel != null}");
            if (previewPanel != null)
            {
                previewPanel.SetActive(true);
                Debug.Log("[SkinPanel] Preview panel activated");
            }
            else
            {
                Debug.LogError("[SkinPanel] previewPanel is null! Please assign it in Inspector.");
            }
        }
        
        /// <summary>
        /// 隐藏预览面板
        /// </summary>
        private void HidePreview()
        {
            if (previewPanel != null)
            {
                previewPanel.SetActive(false);
            }
        }
        
        /// <summary>
        /// 刷新皮肤列表
        /// </summary>
        private void RefreshSkinList()
        {
            if (SkinManager.Instance == null || skinListContainer == null) return;
            
            // 清除旧列表
            foreach (var item in skinItems)
            {
                if (item != null)
                    Destroy(item.gameObject);
            }
            skinItems.Clear();
            
            // 获取皮肤列表
            var skins = SkinManager.Instance.GetAllSkins();
            
            if (skins.Count == 0)
            {
                Debug.LogWarning("[SkinPanel] No skins found!");
                return;
            }
            
            if (skinItemPrefab == null)
            {
                Debug.LogError("[SkinPanel] skinItemPrefab is null!");
                return;
            }
            
            // 创建新列表
            foreach (var skin in skins)
            {
                if (skin == null) continue;
                
                GameObject itemObj = Instantiate(skinItemPrefab, skinListContainer);
                itemObj.SetActive(true);
                
                SkinItemUI itemUI = itemObj.GetComponent<SkinItemUI>();
                
                if (itemUI != null)
                {
                    bool isUnlocked = SkinManager.Instance.IsSkinUnlocked(skin);
                    bool isEquipped = SkinManager.Instance.GetCurrentSkin() == skin;
                    
                    itemUI.Setup(skin, isUnlocked, isEquipped, OnSkinItemClick);
                    skinItems.Add(itemUI);
                }
            }
            
            // 强制刷新布局
            if (skinListContainer != null)
            {
                LayoutGroup layout = skinListContainer.GetComponent<LayoutGroup>();
                if (layout != null)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(skinListContainer as RectTransform);
                }
            }
        }
        
        /// <summary>
        /// 点击皮肤项
        /// </summary>
        private void OnSkinItemClick(SkinData skin)
        {
            Debug.Log($"[SkinPanel] OnSkinItemClick: {skin?.skinName}");
            selectedSkin = skin;
            
            // 更新预览内容
            UpdatePreviewContent(skin);
            
            // 显示预览面板
            ShowPreview();
            
            // 高亮选中的项
            foreach (var item in skinItems)
            {
                item.SetSelected(item.SkinData == skin);
            }
        }
        
        /// <summary>
        /// 更新预览面板内容
        /// </summary>
        private void UpdatePreviewContent(SkinData skin)
        {
            if (skin == null) return;
            
            // 更新预览图
            if (previewImage != null && skin.icon != null)
                previewImage.sprite = skin.icon;
            
            // 更新名称
            SetNameText(skin.skinName);
            
            // 更新描述
            SetDescriptionText(skin.description);
            
            // 更新按钮状态
            bool isUnlocked = SkinManager.Instance.IsSkinUnlocked(skin);
            bool isEquipped = SkinManager.Instance.GetCurrentSkin() == skin;
            
            if (equipButton != null)
            {
                // 已装备时显示"已装备"，未装备但已解锁时显示"替换"
                string buttonLabel = isEquipped ? "已装备" : "替换";
                
                // 尝试 TMP
                TextMeshProUGUI tmpText = equipButton.GetComponentInChildren<TextMeshProUGUI>();
                if (tmpText != null)
                {
                    tmpText.text = buttonLabel;
                }
                else
                {
                    // 尝试 Legacy Text
                    Text legacyText = equipButton.GetComponentInChildren<Text>();
                    if (legacyText != null)
                    {
                        legacyText.text = buttonLabel;
                    }
                }
                
                equipButton.gameObject.SetActive(isUnlocked);
                equipButton.interactable = !isEquipped;
            }
            
            if (unlockButton != null)
            {
                unlockButton.gameObject.SetActive(!isUnlocked);
            }
            
            // 更新价格文本
            SetPriceText(isUnlocked ? "已拥有" : $"{skin.price} 金币");
        }
        
        /// <summary>
        /// 点击装备按钮
        /// </summary>
        private void OnEquipClick()
        {
            if (selectedSkin == null || SkinManager.Instance == null) return;
            
            SkinManager.Instance.EquipSkin(selectedSkin);
            
            // 刷新列表显示
            RefreshSkinList();
            
            // 更新预览面板（按钮状态）
            UpdatePreviewContent(selectedSkin);
            
            Debug.Log($"[SkinPanel] 装备皮肤: {selectedSkin.skinName}");
        }
        
        /// <summary>
        /// 点击解锁按钮
        /// </summary>
        private void OnUnlockClick()
        {
            if (selectedSkin == null || SkinManager.Instance == null) return;
            
            // TODO: 检查金币是否足够
            // TODO: 扣除金币
            
            SkinManager.Instance.UnlockSkin(selectedSkin);
            
            // 刷新显示
            RefreshSkinList();
            UpdatePreviewContent(selectedSkin);
            
            Debug.Log($"[SkinPanel] 解锁皮肤: {selectedSkin.skinName}");
        }
        
        /// <summary>
        /// 点击取消按钮 - 关闭预览面板，回到列表
        /// </summary>
        private void OnCancelClick()
        {
            // 隐藏预览面板
            HidePreview();
            
            // 清除选中状态
            selectedSkin = null;
            foreach (var item in skinItems)
            {
                item.SetSelected(false);
            }
            
            Debug.Log("[SkinPanel] 取消预览，返回列表");
        }
        
        /// <summary>
        /// 设置名称文本（自动适配 Legacy Text 或 TMP）
        /// </summary>
        private void SetNameText(string text)
        {
            if (previewNameTMP != null)
            {
                previewNameTMP.text = text;
            }
            else if (previewNameLegacy != null)
            {
                previewNameLegacy.text = text;
            }
        }
        
        /// <summary>
        /// 设置描述文本（自动适配 Legacy Text 或 TMP）
        /// </summary>
        private void SetDescriptionText(string text)
        {
            if (previewDescriptionTMP != null)
            {
                previewDescriptionTMP.text = text;
            }
            else if (previewDescriptionLegacy != null)
            {
                previewDescriptionLegacy.text = text;
            }
        }
        
        /// <summary>
        /// 设置价格文本（自动适配 Legacy Text 或 TMP）
        /// </summary>
        private void SetPriceText(string text)
        {
            if (priceTextTMP != null)
            {
                priceTextTMP.text = text;
            }
            else if (priceTextLegacy != null)
            {
                priceTextLegacy.text = text;
            }
        }
    }
}
