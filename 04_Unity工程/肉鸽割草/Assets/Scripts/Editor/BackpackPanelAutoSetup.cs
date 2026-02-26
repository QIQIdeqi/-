using UnityEngine;
using UnityEngine.UI;

namespace FluffyGeometry.UI
{
    /// <summary>
    /// 背包面板预制体自动配置工具
    /// 将此脚本挂载在空物体上，点击"自动配置"即可生成完整背包面板
    /// </summary>
    public class BackpackPanelAutoSetup : MonoBehaviour
    {
        [Header("【自动配置设置】")]
        [Tooltip("点击此按钮自动生成背包面板预制体")]
        public bool autoSetup = false;
        
        private void OnValidate()
        {
            if (autoSetup)
            {
                autoSetup = false;
                SetupBackpackPanel();
            }
        }
        
        /// <summary>
        /// 自动配置背包面板
        /// </summary>
        private void SetupBackpackPanel()
        {
            // 创建根节点
            GameObject panelObj = new GameObject("BackpackPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            panelObj.transform.SetParent(transform);
            
            var rectTransform = panelObj.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            
            var bgImage = panelObj.GetComponent<Image>();
            bgImage.color = new Color(1f, 1f, 1f, 0.95f);
            
            // 添加BackpackPanel脚本
            var backpackPanel = panelObj.AddComponent<BackpackPanel>();
            
            // 1. 创建标题栏
            CreateTitleBar(panelObj.transform);
            
            // 2. 创建关闭按钮
            backpackPanel.closeBtn = CreateCloseButton(panelObj.transform);
            
            // 3. 创建Tab容器
            backpackPanel.tabContainer = CreateTabContainer(panelObj.transform, backpackPanel);
            
            // 4. 创建内容区域
            CreateContentAreas(panelObj.transform, backpackPanel);
            
            // 5. 创建预制体引用（需要在运行时赋值）
            Debug.Log("[BackpackPanelAutoSetup] 背包面板结构已创建！请手动配置以下引用：");
            Debug.Log("  - outfitItemPrefab");
            Debug.Log("  - furnitureItemPrefab");
            
            // 选中创建的物体
            UnityEditor.Selection.activeGameObject = panelObj;
        }
        
        /// <summary>
        /// 创建标题栏
        /// </summary>
        private void CreateTitleBar(Transform parent)
        {
            GameObject titleObj = new GameObject("Title", typeof(RectTransform), typeof(Text));
            titleObj.transform.SetParent(parent);
            
            var rect = titleObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.anchoredPosition = new Vector2(0, -20);
            rect.sizeDelta = new Vector2(0, 60);
            
            var text = titleObj.GetComponent<Text>();
            text.text = "背包";
            text.fontSize = 36;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = new Color(0.3f, 0.3f, 0.3f);
            
            // 设置字体（如果有默认字体）
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
        
        /// <summary>
        /// 创建关闭按钮
        /// </summary>
        private Button CreateCloseButton(Transform parent)
        {
            GameObject btnObj = new GameObject("CloseBtn", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            btnObj.transform.SetParent(parent);
            
            var rect = btnObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(1, 1);
            rect.anchoredPosition = new Vector2(-20, -20);
            rect.sizeDelta = new Vector2(60, 60);
            
            var image = btnObj.GetComponent<Image>();
            image.color = new Color(0.9f, 0.3f, 0.3f);
            
            // 添加X文本
            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(Text));
            textObj.transform.SetParent(btnObj.transform);
            var textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            var text = textObj.GetComponent<Text>();
            text.text = "×";
            text.fontSize = 40;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            
            return btnObj.GetComponent<Button>();
        }
        
        /// <summary>
        /// 创建Tab容器
        /// </summary>
        private Transform CreateTabContainer(Transform parent, BackpackPanel panel)
        {
            GameObject containerObj = new GameObject("TabContainer", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            containerObj.transform.SetParent(parent);
            
            var rect = containerObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1);
            rect.anchorMax = new Vector2(0.5f, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.anchoredPosition = new Vector2(0, -100);
            rect.sizeDelta = new Vector2(400, 60);
            
            var layout = containerObj.GetComponent<HorizontalLayoutGroup>();
            layout.spacing = 20;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            
            // 创建主角装扮Tab
            panel.characterTabBtn = CreateTabButton(containerObj.transform, "主角装扮", true);
            
            // 创建家园装扮Tab
            panel.furnitureTabBtn = CreateTabButton(containerObj.transform, "家园装扮", false);
            
            return containerObj.transform;
        }
        
        /// <summary>
        /// 创建Tab按钮
        /// </summary>
        private Button CreateTabButton(Transform parent, string text, bool isActive)
        {
            GameObject btnObj = new GameObject($"{text}Tab", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            btnObj.transform.SetParent(parent);
            
            var rect = btnObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(180, 50);
            
            var image = btnObj.GetComponent<Image>();
            image.color = isActive ? new Color(1f, 0.8f, 0.6f) : new Color(0.8f, 0.8f, 0.8f);
            
            // 添加文本
            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(Text));
            textObj.transform.SetParent(btnObj.transform);
            var textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(10, 5);
            textRect.offsetMax = new Vector2(-10, -5);
            
            var txt = textObj.GetComponent<Text>();
            txt.text = text;
            txt.fontSize = 24;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = new Color(0.2f, 0.2f, 0.2f);
            
            return btnObj.GetComponent<Button>();
        }
        
        /// <summary>
        /// 创建内容区域
        /// </summary>
        private void CreateContentAreas(Transform parent, BackpackPanel panel)
        {
            // 主角装扮内容区
            panel.characterContent = CreateScrollView(parent, "CharacterContent", true);
            
            // 家园装扮内容区
            panel.furnitureContent = CreateScrollView(parent, "FurnitureContent", false);
            
            // 获取容器引用
            var characterViewport = panel.characterContent.transform.Find("Viewport");
            if (characterViewport != null)
            {
                var characterContent = characterViewport.Find("Content");
                if (characterContent != null)
                {
                    panel.outfitListContainer = characterContent;
                }
            }
            
            var furnitureViewport = panel.furnitureContent.transform.Find("Viewport");
            if (furnitureViewport != null)
            {
                var furnitureContent = furnitureViewport.Find("Content");
                if (furnitureContent != null)
                {
                    panel.furnitureListContainer = furnitureContent;
                }
            }
        }
        
        /// <summary>
        /// 创建滚动视图
        /// </summary>
        private GameObject CreateScrollView(Transform parent, string name, bool active)
        {
            GameObject scrollObj = new GameObject(name, typeof(RectTransform), typeof(ScrollRect), typeof(Image));
            scrollObj.transform.SetParent(parent);
            
            var rect = scrollObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMin = new Vector2(40, 100);
            rect.offsetMax = new Vector2(-40, -180);
            
            var image = scrollObj.GetComponent<Image>();
            image.color = new Color(0.95f, 0.95f, 0.95f);
            
            var scrollRect = scrollObj.GetComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            
            // 创建Viewport
            GameObject viewportObj = new GameObject("Viewport", typeof(RectTransform), typeof(CanvasRenderer), typeof(Mask), typeof(Image));
            viewportObj.transform.SetParent(scrollObj.transform);
            var viewportRect = viewportObj.GetComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = Vector2.zero;
            viewportRect.offsetMax = Vector2.zero;
            
            var mask = viewportObj.GetComponent<Mask>();
            mask.showMaskGraphic = false;
            
            scrollRect.viewport = viewportRect;
            
            // 创建Content
            GameObject contentObj = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            contentObj.transform.SetParent(viewportObj.transform);
            var contentRect = contentObj.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = Vector2.zero;
            
            var layout = contentObj.GetComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(20, 20, 20, 20);
            layout.spacing = 15;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            
            var fitter = contentObj.GetComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            
            scrollRect.content = contentRect;
            
            scrollObj.SetActive(active);
            
            return scrollObj;
        }
    }
}
