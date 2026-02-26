using UnityEngine;
using UnityEngine.UI;

namespace FluffyGeometry.UI
{
    /// <summary>
    /// 背包按钮预制体自动配置工具
    /// </summary>
    public class BackpackButtonAutoSetup : MonoBehaviour
    {
        [Header("【自动配置设置】")]
        [Tooltip("点击此按钮自动生成背包按钮预制体")]
        public bool autoSetup = false;
        
        [Tooltip("背包图标（可选）")]
        public Sprite backpackIcon;
        
        private void OnValidate()
        {
            if (autoSetup)
            {
                autoSetup = false;
                SetupBackpackButton();
            }
        }
        
        /// <summary>
        /// 自动配置背包按钮
        /// </summary>
        private void SetupBackpackButton()
        {
            GameObject btnObj = new GameObject("BackpackButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            btnObj.transform.SetParent(transform);
            
            var rectTransform = btnObj.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0, 1);
            rectTransform.anchoredPosition = new Vector2(30, -30);
            rectTransform.sizeDelta = new Vector2(80, 80);
            
            var image = btnObj.GetComponent<Image>();
            if (backpackIcon != null)
            {
                image.sprite = backpackIcon;
                image.color = Color.white;
            }
            else
            {
                image.color = new Color(0.8f, 0.6f, 0.4f);
            }
            
            // 添加BackpackButton脚本
            var backpackBtn = btnObj.AddComponent<BackpackButton>();
            backpackBtn.backpackBtn = btnObj.GetComponent<Button>();
            backpackBtn.backpackIcon = image;
            
            // 创建新物品红点
            GameObject badgeObj = new GameObject("NewItemBadge", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            badgeObj.transform.SetParent(btnObj.transform);
            
            var badgeRect = badgeObj.GetComponent<RectTransform>();
            badgeRect.anchorMin = new Vector2(1, 1);
            badgeRect.anchorMax = new Vector2(1, 1);
            badgeRect.pivot = new Vector2(0.5f, 0.5f);
            badgeRect.anchoredPosition = new Vector2(-10, -10);
            badgeRect.sizeDelta = new Vector2(20, 20);
            
            var badgeImage = badgeObj.GetComponent<Image>();
            badgeImage.color = Color.red;
            
            backpackBtn.newItemBadge = badgeObj;
            badgeObj.SetActive(false);
            
            // 选中创建的物体
            UnityEditor.Selection.activeGameObject = btnObj;
            
            Debug.Log("[BackpackButtonAutoSetup] 背包按钮预制体已创建！");
            Debug.Log("  请手动配置 backpackPanelPrefab 引用");
        }
    }
}
