using UnityEngine;
using UnityEngine.UI;

namespace FluffyGeometry.UI
{
    /// <summary>
    /// 家具项预制体自动配置工具
    /// </summary>
    public class FurnitureItemAutoSetup : MonoBehaviour
    {
        [Header("【自动配置设置】")]
        [Tooltip("点击此按钮自动生成家具项预制体")]
        public bool autoSetup = false;
        
        [Tooltip("家具图标（可选）")]
        public Sprite defaultIcon;
        
        private void OnValidate()
        {
            if (autoSetup)
            {
                autoSetup = false;
                SetupFurnitureItem();
            }
        }
        
        /// <summary>
        /// 自动配置家具项
        /// </summary>
        private void SetupFurnitureItem()
        {
            GameObject itemObj = new GameObject("FurnitureItem", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            itemObj.transform.SetParent(transform);
            
            var rectTransform = itemObj.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(500, 100);
            
            var bgImage = itemObj.GetComponent<Image>();
            bgImage.color = new Color(1f, 1f, 1f);
            
            // 添加FurnitureItemUI脚本
            var itemUI = itemObj.AddComponent<FurnitureItemUI>();
            
            // 1. 创建选中态边框
            itemUI.selectedBorder = CreateSelectedBorder(itemObj.transform);
            
            // 2. 创建图标
            itemUI.furnitureIcon = CreateIcon(itemObj.transform);
            
            // 3. 创建名称文本
            itemUI.furnitureNameText = CreateNameText(itemObj.transform);
            
            // 4. 创建装饰按钮
            itemUI.decorateBtn = CreateDecorateButton(itemObj.transform, itemUI);
            
            // 5. 创建已放置标签
            itemUI.placedLabel = CreatePlacedLabel(itemObj.transform);
            
            // 选中创建的物体
            UnityEditor.Selection.activeGameObject = itemObj;
            
            Debug.Log("[FurnitureItemAutoSetup] 家具项预制体已创建！");
        }
        
        /// <summary>
        /// 创建选中态边框
        /// </summary>
        private GameObject CreateSelectedBorder(Transform parent)
        {
            GameObject borderObj = new GameObject("SelectedBorder", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            borderObj.transform.SetParent(parent);
            
            var rect = borderObj.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            var image = borderObj.GetComponent<Image>();
            image.color = new Color(1f, 0.8f, 0.4f, 0.3f);
            
            borderObj.SetActive(false);
            
            return borderObj;
        }
        
        /// <summary>
        /// 创建图标
        /// </summary>
        private Image CreateIcon(Transform parent)
        {
            GameObject iconObj = new GameObject("Icon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            iconObj.transform.SetParent(parent);
            
            var rect = iconObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0.5f);
            rect.anchorMax = new Vector2(0, 0.5f);
            rect.pivot = new Vector2(0, 0.5f);
            rect.anchoredPosition = new Vector2(20, 0);
            rect.sizeDelta = new Vector2(80, 80);
            
            var image = iconObj.GetComponent<Image>();
            if (defaultIcon != null)
            {
                image.sprite = defaultIcon;
            }
            image.color = new Color(0.9f, 0.9f, 0.9f);
            
            return image;
        }
        
        /// <summary>
        /// 创建名称文本
        /// </summary>
        private Text CreateNameText(Transform parent)
        {
            GameObject textObj = new GameObject("Name", typeof(RectTransform), typeof(Text));
            textObj.transform.SetParent(parent);
            
            var rect = textObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0.5f);
            rect.anchorMax = new Vector2(1, 0.5f);
            rect.pivot = new Vector2(0, 0.5f);
            rect.anchoredPosition = new Vector2(120, 0);
            rect.sizeDelta = new Vector2(-240, 40);
            
            var text = textObj.GetComponent<Text>();
            text.text = "家具名称";
            text.fontSize = 24;
            text.alignment = TextAnchor.MiddleLeft;
            text.color = new Color(0.2f, 0.2f, 0.2f);
            
            return text;
        }
        
        /// <summary>
        /// 创建装饰按钮
        /// </summary>
        private Button CreateDecorateButton(Transform parent, FurnitureItemUI itemUI)
        {
            GameObject btnObj = new GameObject("DecorateBtn", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            btnObj.transform.SetParent(parent);
            
            var rect = btnObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1, 0.5f);
            rect.anchorMax = new Vector2(1, 0.5f);
            rect.pivot = new Vector2(1, 0.5f);
            rect.anchoredPosition = new Vector2(-20, 0);
            rect.sizeDelta = new Vector2(100, 50);
            
            var image = btnObj.GetComponent<Image>();
            image.color = new Color(0.4f, 0.8f, 0.4f);
            
            // 创建按钮文本
            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(Text));
            textObj.transform.SetParent(btnObj.transform);
            var textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(5, 5);
            textRect.offsetMax = new Vector2(-5, -5);
            
            var text = textObj.GetComponent<Text>();
            text.text = "装饰";
            text.fontSize = 20;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            
            itemUI.decorateBtnText = text;
            
            btnObj.SetActive(false);
            
            return btnObj.GetComponent<Button>();
        }
        
        /// <summary>
        /// 创建已放置标签
        /// </summary>
        private GameObject CreatePlacedLabel(Transform parent)
        {
            GameObject labelObj = new GameObject("PlacedLabel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            labelObj.transform.SetParent(parent);
            
            var rect = labelObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1, 0.5f);
            rect.anchorMax = new Vector2(1, 0.5f);
            rect.pivot = new Vector2(1, 0.5f);
            rect.anchoredPosition = new Vector2(-130, 0);
            rect.sizeDelta = new Vector2(80, 30);
            
            var image = labelObj.GetComponent<Image>();
            image.color = new Color(0.4f, 0.6f, 1f, 0.8f);
            
            // 创建文本
            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(Text));
            textObj.transform.SetParent(labelObj.transform);
            var textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(2, 2);
            textRect.offsetMax = new Vector2(-2, -2);
            
            var text = textObj.GetComponent<Text>();
            text.text = "已放置";
            text.fontSize = 14;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            
            labelObj.SetActive(false);
            
            return labelObj;
        }
    }
}
