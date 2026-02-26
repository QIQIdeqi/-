using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace FluffyGeometry.UI
{
    /// <summary>
    /// 背包按钮 - 家园场景左上角的背包入口
    /// </summary>
    public class BackpackButton : MonoBehaviour
    {
        [Header("【UI引用】")]
        [Tooltip("背包按钮")]
        public Button backpackBtn;
        
        [Tooltip("背包图标")]
        public Image backpackIcon;
        
        [Tooltip("新物品提示红点")]
        public GameObject newItemBadge;
        
        [Header("【配置】")]
        [Tooltip("背包面板预制体")]
        public BackpackPanel backpackPanelPrefab;
        
        [Header("【状态】")]
        [Tooltip("是否有新物品")]
        public bool hasNewItem = false;
        
        // 当前打开的背包面板
        private BackpackPanel currentPanel;
        
        private void Start()
        {
            if (backpackBtn != null)
            {
                backpackBtn.onClick.AddListener(OnBackpackClick);
            }
            
            UpdateBadge();
        }
        
        private void OnDestroy()
        {
            if (backpackBtn != null)
            {
                backpackBtn.onClick.RemoveListener(OnBackpackClick);
            }
        }
        
        /// <summary>
        /// 点击背包按钮
        /// </summary>
        private void OnBackpackClick()
        {
            if (currentPanel == null)
            {
                OpenBackpack();
            }
            else
            {
                CloseBackpack();
            }
        }
        
        /// <summary>
        /// 打开背包
        /// </summary>
        public void OpenBackpack(int defaultTab = 0)
        {
            if (backpackPanelPrefab == null) return;
            
            // 创建背包面板
            currentPanel = Instantiate(backpackPanelPrefab, transform.root);
            currentPanel.Initialize(this);
            currentPanel.Show(defaultTab);
            
            // 隐藏按钮（可选，根据设计）
            // backpackBtn.gameObject.SetActive(false);
        }
        
        /// <summary>
        /// 关闭背包
        /// </summary>
        public void CloseBackpack()
        {
            if (currentPanel != null)
            {
                currentPanel.Hide();
                Destroy(currentPanel.gameObject);
                currentPanel = null;
            }
            
            // 显示按钮
            // backpackBtn.gameObject.SetActive(true);
        }
        
        /// <summary>
        /// 背包面板关闭回调
        /// </summary>
        public void OnPanelClosed()
        {
            currentPanel = null;
        }
        
        /// <summary>
        /// 设置新物品提示
        /// </summary>
        public void SetNewItemBadge(bool show)
        {
            hasNewItem = show;
            UpdateBadge();
        }
        
        /// <summary>
        /// 更新红点显示
        /// </summary>
        private void UpdateBadge()
        {
            if (newItemBadge != null)
            {
                newItemBadge.SetActive(hasNewItem);
            }
        }
    }
}
