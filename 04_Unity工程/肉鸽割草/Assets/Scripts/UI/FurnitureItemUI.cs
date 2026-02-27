using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using FluffyGeometry.Home;

namespace FluffyGeometry.UI
{
    /// <summary>
    /// 家具项UI - 背包中的家具列表项
    /// </summary>
    public class FurnitureItemUI : MonoBehaviour, IPointerClickHandler
    {
        [Header("【UI引用】")]
        [Tooltip("家具图标")]
        public Image furnitureIcon;
        
        [Tooltip("家具名称")]
        public Text furnitureNameText;
        
        [Tooltip("数量文本")]
        public Text countText;
        
        [Tooltip("选中态边框")]
        public GameObject selectedBorder;
        
        [Tooltip("装饰按钮")]
        public Button decorateBtn;
        
        [Tooltip("装饰按钮文本")]
        public Text decorateBtnText;
        
        [Tooltip("已放置标签")]
        public GameObject placedLabel;
        
        [Header("【配置】")]
        [Tooltip("装饰按钮显示延迟")]
        public float decorateBtnDelay = 0.1f;
        
        [Header("【状态】")]
        [Tooltip("关联的家具数据")]
        public FurnitureData furnitureData;
        
        [Tooltip("是否被选中")]
        public bool isSelected = false;
        
        // 回调
        private System.Action<FurnitureData> onClickCallback;
        private System.Action<FurnitureData> onDecorateCallback;
        
        /// <summary>
        /// 设置家具项
        /// </summary>
        public void Setup(FurnitureData data, 
            System.Action<FurnitureData> clickCallback,
            System.Action<FurnitureData> decorateCallback)
        {
            furnitureData = data;
            onClickCallback = clickCallback;
            onDecorateCallback = decorateCallback;
            
            // 设置图标
            if (furnitureIcon != null && data.furnitureSprite != null)
            {
                furnitureIcon.sprite = data.furnitureSprite;
            }
            
            // 设置名称
            if (furnitureNameText != null)
            {
                furnitureNameText.text = data.furnitureName;
            }
            
            // 更新数量和按钮状态
            UpdateCountDisplay();
            
            // 隐藏装饰按钮和选中态
            SetSelected(false);
            if (decorateBtn != null)
            {
                decorateBtn.gameObject.SetActive(false);
            }
            
            // 绑定装饰按钮
            if (decorateBtn != null)
            {
                decorateBtn.onClick.RemoveAllListeners();
                decorateBtn.onClick.AddListener(OnDecorateClick);
            }
        }
        
        /// <summary>
        /// 更新数量显示和按钮状态
        /// </summary>
        public void UpdateCountDisplay()
        {
            if (furnitureData == null) return;
            
            // 确保 FurnitureInventory 实例存在
            if (FurnitureInventory.Instance == null)
            {
                Debug.LogWarning($"[FurnitureItemUI] FurnitureInventory.Instance 为 null，尝试查找...");
                FindObjectOfType<FurnitureInventory>();
            }
            
            int totalCount = 0;
            int placedCount = 0;
            int availableCount = 0;
            
            if (FurnitureInventory.Instance != null)
            {
                totalCount = FurnitureInventory.Instance.GetTotalCount(furnitureData.furnitureId);
                placedCount = FurnitureInventory.Instance.GetPlacedCount(furnitureData.furnitureId);
                availableCount = FurnitureInventory.Instance.GetAvailableCount(furnitureData.furnitureId);
            }
            
            Debug.Log($"[FurnitureItemUI] {furnitureData.furnitureId}: 总计={totalCount}, 已放置={placedCount}, 可用={availableCount}");
            
            // 更新数量文本
            if (countText != null)
            {
                countText.text = $"x{availableCount}";
                countText.gameObject.SetActive(availableCount > 0);
            }
            
            // 更新按钮状态
            bool canPlace = availableCount > 0;
            if (decorateBtn != null)
            {
                decorateBtn.interactable = canPlace;
                
                // 更新按钮颜色和文本
                var colors = decorateBtn.colors;
                if (canPlace)
                {
                    colors.normalColor = new Color(0.4f, 0.8f, 0.4f); // 绿色
                    colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                }
                else
                {
                    colors.normalColor = new Color(0.5f, 0.5f, 0.5f); // 灰色
                }
                decorateBtn.colors = colors;
            }
            
            if (decorateBtnText != null)
            {
                decorateBtnText.text = canPlace ? "装饰" : "数量不足";
            }
        }
        
        /// <summary>
        /// 点击整项
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            // 通知父级选中此项
            onClickCallback?.Invoke(furnitureData);
            SetSelected(true);
        }
        
        /// <summary>
        /// 设置选中状态
        /// </summary>
        public void SetSelected(bool selected)
        {
            isSelected = selected;
            
            if (selectedBorder != null)
            {
                selectedBorder.SetActive(selected);
            }
            
            // 显示/隐藏装饰按钮，并更新数量状态
            if (decorateBtn != null)
            {
                decorateBtn.gameObject.SetActive(selected);
                if (selected)
                {
                    UpdateCountDisplay();
                }
            }
        }
        
        /// <summary>
        /// 隐藏装饰按钮（当其他项被选中时）
        /// </summary>
        public void HideDecorateButton()
        {
            if (decorateBtn != null)
            {
                decorateBtn.gameObject.SetActive(false);
            }
            if (selectedBorder != null)
            {
                selectedBorder.SetActive(false);
            }
            isSelected = false;
        }
        
        /// <summary>
        /// 装饰按钮点击
        /// </summary>
        private void OnDecorateClick()
        {
            onDecorateCallback?.Invoke(furnitureData);
        }
        
        /// <summary>
        /// 设置已放置状态
        /// </summary>
        public void SetPlaced(bool placed)
        {
            if (placedLabel != null)
            {
                placedLabel.SetActive(placed);
            }
            
            // 如果已放置，更改装饰按钮文本
            if (decorateBtnText != null && placed)
            {
                decorateBtnText.text = "已放置";
            }
        }
    }
}
