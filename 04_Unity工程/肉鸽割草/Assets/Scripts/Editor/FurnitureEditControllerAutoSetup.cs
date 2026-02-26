using UnityEngine;
using UnityEngine.UI;

namespace FluffyGeometry.Home
{
    /// <summary>
    /// 家具编辑控制器预制体自动配置工具
    /// </summary>
    public class FurnitureEditControllerAutoSetup : MonoBehaviour
    {
        [Header("【自动配置设置】")]
        [Tooltip("点击此按钮自动生成家具编辑控制器预制体")]
        public bool autoSetup = false;
        
        private void OnValidate()
        {
            if (autoSetup)
            {
                autoSetup = false;
                SetupEditController();
            }
        }
        
        /// <summary>
        /// 自动配置编辑控制器
        /// </summary>
        private void SetupEditController()
        {
            GameObject controllerObj = new GameObject("FurnitureEditController");
            controllerObj.transform.SetParent(transform);
            
            // 添加FurnitureEditController脚本
            var controller = controllerObj.AddComponent<FurnitureEditController>();
            
            // 创建工具栏面板
            controller.toolbarPanel = CreateToolbarPanel(controllerObj.transform, controller);
            
            // 选中创建的物体
            UnityEditor.Selection.activeGameObject = controllerObj;
            
            Debug.Log("[FurnitureEditControllerAutoSetup] 家具编辑控制器预制体已创建！");
        }
        
        /// <summary>
        /// 创建工具栏面板
        /// </summary>
        private GameObject CreateToolbarPanel(Transform parent, FurnitureEditController controller)
        {
            GameObject panelObj = new GameObject("ToolbarPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            panelObj.transform.SetParent(parent);
            
            var rect = panelObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(400, 80);
            
            var image = panelObj.GetComponent<Image>();
            image.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
            
            // 创建水平布局
            var layout = panelObj.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(20, 20, 10, 10);
            layout.spacing = 20;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
            
            // 1. 创建翻转按钮
            controller.flipBtn = CreateToolbarButton(panelObj.transform, "翻转", new Color(0.4f, 0.6f, 1f));
            
            // 2. 创建缩放滑条
            CreateScaleSlider(panelObj.transform, controller);
            
            // 3. 创建确认按钮
            controller.confirmBtn = CreateToolbarButton(panelObj.transform, "确认", new Color(0.4f, 0.8f, 0.4f));
            
            panelObj.SetActive(false);
            
            return panelObj;
        }
        
        /// <summary>
        /// 创建工具栏按钮
        /// </summary>
        private Button CreateToolbarButton(Transform parent, string text, Color color)
        {
            GameObject btnObj = new GameObject($"{text}Btn", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            btnObj.transform.SetParent(parent);
            
            var rect = btnObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(80, 50);
            
            var image = btnObj.GetComponent<Image>();
            image.color = color;
            
            // 创建文本
            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(Text));
            textObj.transform.SetParent(btnObj.transform);
            var textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(5, 5);
            textRect.offsetMax = new Vector2(-5, -5);
            
            var txt = textObj.GetComponent<Text>();
            txt.text = text;
            txt.fontSize = 20;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = Color.white;
            
            return btnObj.GetComponent<Button>();
        }
        
        /// <summary>
        /// 创建缩放滑条
        /// </summary>
        private void CreateScaleSlider(Transform parent, FurnitureEditController controller)
        {
            // 创建滑条容器
            GameObject sliderContainer = new GameObject("ScaleSliderContainer", typeof(RectTransform));
            sliderContainer.transform.SetParent(parent);
            
            var containerRect = sliderContainer.GetComponent<RectTransform>();
            containerRect.sizeDelta = new Vector2(150, 60);
            
            // 创建滑条
            GameObject sliderObj = new GameObject("ScaleSlider", typeof(RectTransform), typeof(Slider));
            sliderObj.transform.SetParent(sliderContainer.transform);
            
            var sliderRect = sliderObj.GetComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0, 0.5f);
            sliderRect.anchorMax = new Vector2(1, 0.5f);
            sliderRect.pivot = new Vector2(0.5f, 0.5f);
            sliderRect.anchoredPosition = new Vector2(0, -10);
            sliderRect.sizeDelta = new Vector2(-20, 20);
            
            var slider = sliderObj.GetComponent<Slider>();
            slider.minValue = 0.5f;
            slider.maxValue = 2f;
            slider.value = 1f;
            
            // 创建背景
            GameObject bgObj = new GameObject("Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            bgObj.transform.SetParent(sliderObj.transform);
            var bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            bgObj.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f);
            
            // 创建Fill Area
            GameObject fillAreaObj = new GameObject("Fill Area", typeof(RectTransform));
            fillAreaObj.transform.SetParent(sliderObj.transform);
            var fillAreaRect = fillAreaObj.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.offsetMin = new Vector2(5, 0);
            fillAreaRect.offsetMax = new Vector2(-5, 0);
            
            GameObject fillObj = new GameObject("Fill", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            fillObj.transform.SetParent(fillAreaObj.transform);
            var fillRect = fillObj.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            fillObj.GetComponent<Image>().color = new Color(0.4f, 0.8f, 0.4f);
            
            // 创建Handle Area
            GameObject handleAreaObj = new GameObject("Handle Slide Area", typeof(RectTransform));
            handleAreaObj.transform.SetParent(sliderObj.transform);
            var handleAreaRect = handleAreaObj.GetComponent<RectTransform>();
            handleAreaRect.anchorMin = Vector2.zero;
            handleAreaRect.anchorMax = Vector2.one;
            handleAreaRect.offsetMin = new Vector2(10, 0);
            handleAreaRect.offsetMax = new Vector2(-10, 0);
            
            GameObject handleObj = new GameObject("Handle", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            handleObj.transform.SetParent(handleAreaObj.transform);
            var handleRect = handleObj.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(20, 30);
            handleObj.GetComponent<Image>().color = Color.white;
            
            slider.fillRect = fillObj.GetComponent<RectTransform>();
            slider.handleRect = handleObj.GetComponent<RectTransform>();
            slider.targetGraphic = handleObj.GetComponent<Image>();
            
            controller.scaleSlider = slider;
            
            // 创建数值文本
            GameObject valueObj = new GameObject("ScaleValue", typeof(RectTransform), typeof(Text));
            valueObj.transform.SetParent(sliderContainer.transform);
            var valueRect = valueObj.GetComponent<RectTransform>();
            valueRect.anchorMin = new Vector2(0, 1);
            valueRect.anchorMax = new Vector2(1, 1);
            valueRect.pivot = new Vector2(0.5f, 1);
            valueRect.anchoredPosition = new Vector2(0, 0);
            valueRect.sizeDelta = new Vector2(0, 20);
            
            var valueText = valueObj.GetComponent<Text>();
            valueText.text = "1.0x";
            valueText.fontSize = 16;
            valueText.alignment = TextAnchor.MiddleCenter;
            valueText.color = Color.white;
            
            controller.scaleValueText = valueText;
        }
    }
}
