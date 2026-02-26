using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using FluffyGeometry.Home;

namespace FluffyGeometry.Editor
{
    /// <summary>
    /// 背包系统一键配置工具
    /// 在菜单栏：绒毛几何物语 -> 背包系统一键配置
    /// </summary>
    public class BackpackSystemSetupWizard : EditorWindow
    {
        private string prefabSavePath = "Assets/Prefabs/UI/";
        private string furnitureDataPath = "Assets/Resources/Furniture/";
        
        private bool createBackpackButton = true;
        private bool createBackpackPanel = true;
        private bool createFurnitureItem = true;
        private bool createFurnitureEditController = true;
        private bool createSampleFurniture = true;
        
        private Texture2D headerTexture;
        private Vector2 scrollPosition;
        
        [MenuItem("绒毛几何物语/背包系统一键配置")]
        public static void ShowWindow()
        {
            var window = GetWindow<BackpackSystemSetupWizard>("背包系统配置");
            window.minSize = new Vector2(400, 500);
            window.Show();
        }
        
        private void OnGUI()
        {
            // 标题
            GUILayout.Space(10);
            GUIStyle titleStyle = new GUIStyle(EditorStyles.largeLabel);
            titleStyle.fontSize = 20;
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label("🏠 家园背包系统配置", titleStyle);
            
            GUILayout.Space(5);
            GUIStyle descStyle = new GUIStyle(EditorStyles.label);
            descStyle.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label("一键创建所有预制体和配置", descStyle);
            
            GUILayout.Space(20);
            
            // 滚动区域
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            
            // 路径设置
            GUILayout.Label("📁 保存路径", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            prefabSavePath = EditorGUILayout.TextField("预制体保存路径", prefabSavePath);
            furnitureDataPath = EditorGUILayout.TextField("家具数据保存路径", furnitureDataPath);
            EditorGUILayout.EndVertical();
            
            GUILayout.Space(15);
            
            // 创建选项
            GUILayout.Label("🔧 创建选项", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            createBackpackButton = EditorGUILayout.ToggleLeft("  背包按钮 (BackpackButton)", createBackpackButton);
            createBackpackPanel = EditorGUILayout.ToggleLeft("  背包面板 (BackpackPanel)", createBackpackPanel);
            createFurnitureItem = EditorGUILayout.ToggleLeft("  家具列表项 (FurnitureItem)", createFurnitureItem);
            createFurnitureEditController = EditorGUILayout.ToggleLeft("  家具编辑控制器 (FurnitureEditController)", createFurnitureEditController);
            createSampleFurniture = EditorGUILayout.ToggleLeft("  示例家具数据 (3个)", createSampleFurniture);
            EditorGUILayout.EndVertical();
            
            GUILayout.Space(15);
            
            // 说明
            GUILayout.Label("📋 配置说明", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("1. 点击'一键配置'按钮创建所有资源", EditorStyles.wordWrappedLabel);
            GUILayout.Label("2. 将生成的预制体拖入场景中的HomeManager", EditorStyles.wordWrappedLabel);
            GUILayout.Label("3. 运行测试背包功能", EditorStyles.wordWrappedLabel);
            EditorGUILayout.EndVertical();
            
            GUILayout.EndScrollView();
            
            GUILayout.Space(10);
            
            // 一键配置按钮
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 16;
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.fixedHeight = 50;
            
            if (GUILayout.Button("🚀 一键配置", buttonStyle))
            {
                SetupBackpackSystem();
            }
            
            GUILayout.Space(10);
        }
        
        private void SetupBackpackSystem()
        {
            int createdCount = 0;
            
            // 确保目录存在
            EnsureDirectoryExists(prefabSavePath);
            EnsureDirectoryExists(furnitureDataPath);
            
            EditorUtility.DisplayProgressBar("背包系统配置", "正在创建资源...", 0f);
            
            try
            {
                // 1. 创建背包按钮
                if (createBackpackButton)
                {
                    EditorUtility.DisplayProgressBar("背包系统配置", "创建背包按钮...", 0.2f);
                    CreateBackpackButton();
                    createdCount++;
                }
                
                // 2. 创建背包面板
                if (createBackpackPanel)
                {
                    EditorUtility.DisplayProgressBar("背包系统配置", "创建背包面板...", 0.4f);
                    CreateBackpackPanel();
                    createdCount++;
                }
                
                // 3. 创建家具列表项
                if (createFurnitureItem)
                {
                    EditorUtility.DisplayProgressBar("背包系统配置", "创建家具列表项...", 0.6f);
                    CreateFurnitureItem();
                    createdCount++;
                }
                
                // 4. 创建家具编辑控制器
                if (createFurnitureEditController)
                {
                    EditorUtility.DisplayProgressBar("背包系统配置", "创建家具编辑控制器...", 0.8f);
                    CreateFurnitureEditController();
                    createdCount++;
                }
                
                // 5. 创建示例家具
                if (createSampleFurniture)
                {
                    EditorUtility.DisplayProgressBar("背包系统配置", "创建示例家具数据...", 0.9f);
                    CreateSampleFurniture();
                    createdCount++;
                }
                
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                
                EditorUtility.ClearProgressBar();
                
                EditorUtility.DisplayDialog("配置完成", 
                    $"成功创建 {createdCount} 个资源！\n\n" +
                    $"预制体保存在：{prefabSavePath}\n" +
                    $"家具数据保存在：{furnitureDataPath}\n\n" +
                    "下一步：将预制体拖入HomeManager配置。", 
                    "确定");
                
                // 高亮保存目录
                PingDirectory(prefabSavePath);
            }
            catch (System.Exception e)
            {
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("配置失败", $"错误：{e.Message}", "确定");
                Debug.LogError($"[BackpackSystemSetupWizard] 配置失败: {e}");
            }
        }
        
        private void CreateBackpackButton()
        {
            // 创建画布
            GameObject canvasObj = new GameObject("BackpackButtonCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObj.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            
            // 创建按钮
            GameObject btnObj = new GameObject("BackpackButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            btnObj.transform.SetParent(canvasObj.transform);
            
            var rectTransform = btnObj.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0, 1);
            rectTransform.anchoredPosition = new Vector2(30, -30);
            rectTransform.sizeDelta = new Vector2(80, 80);
            
            var image = btnObj.GetComponent<Image>();
            image.color = new Color(0.8f, 0.6f, 0.4f);
            
            // 添加脚本
            var backpackBtn = btnObj.AddComponent<FluffyGeometry.UI.BackpackButton>();
            backpackBtn.backpackBtn = btnObj.GetComponent<Button>();
            backpackBtn.backpackIcon = image;
            
            // 红点
            GameObject badgeObj = new GameObject("NewItemBadge", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            badgeObj.transform.SetParent(btnObj.transform);
            var badgeRect = badgeObj.GetComponent<RectTransform>();
            badgeRect.anchorMin = new Vector2(1, 1);
            badgeRect.anchorMax = new Vector2(1, 1);
            badgeRect.pivot = new Vector2(0.5f, 0.5f);
            badgeRect.anchoredPosition = new Vector2(-10, -10);
            badgeRect.sizeDelta = new Vector2(20, 20);
            badgeObj.GetComponent<Image>().color = Color.red;
            badgeObj.SetActive(false);
            backpackBtn.newItemBadge = badgeObj;
            
            // 保存预制体
            string prefabPath = prefabSavePath + "BackpackButton.prefab";
            PrefabUtility.SaveAsPrefabAsset(btnObj, prefabPath);
            DestroyImmediate(canvasObj);
            
            Debug.Log($"[BackpackSystemSetupWizard] 创建: {prefabPath}");
        }
        
        private void CreateBackpackPanel()
        {
            // 创建画布
            GameObject canvasObj = new GameObject("BackpackPanelCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObj.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            
            // 创建面板根
            GameObject panelObj = new GameObject("BackpackPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            panelObj.transform.SetParent(canvasObj.transform);
            
            var rect = panelObj.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            var bgImage = panelObj.GetComponent<Image>();
            bgImage.color = new Color(1f, 1f, 1f, 0.95f);
            
            // 添加脚本
            var backpackPanel = panelObj.AddComponent<FluffyGeometry.UI.BackpackPanel>();
            
            // 标题
            CreateText(panelObj.transform, "Title", "背包", 36, TextAnchor.MiddleCenter, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), new Vector2(0, -60), new Vector2(0, 60));
            
            // 关闭按钮
            GameObject closeBtnObj = new GameObject("CloseBtn", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            closeBtnObj.transform.SetParent(panelObj.transform);
            var closeRect = closeBtnObj.GetComponent<RectTransform>();
            closeRect.anchorMin = new Vector2(1, 1);
            closeRect.anchorMax = new Vector2(1, 1);
            closeRect.pivot = new Vector2(1, 1);
            closeRect.anchoredPosition = new Vector2(-20, -20);
            closeRect.sizeDelta = new Vector2(60, 60);
            closeBtnObj.GetComponent<Image>().color = new Color(0.9f, 0.3f, 0.3f);
            backpackPanel.closeBtn = closeBtnObj.GetComponent<Button>();
            
            // Tab容器
            GameObject tabContainer = new GameObject("TabContainer", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            tabContainer.transform.SetParent(panelObj.transform);
            var tabRect = tabContainer.GetComponent<RectTransform>();
            tabRect.anchorMin = new Vector2(0.5f, 1);
            tabRect.anchorMax = new Vector2(0.5f, 1);
            tabRect.pivot = new Vector2(0.5f, 1);
            tabRect.anchoredPosition = new Vector2(0, -100);
            tabRect.sizeDelta = new Vector2(400, 60);
            var tabLayout = tabContainer.GetComponent<HorizontalLayoutGroup>();
            tabLayout.spacing = 20;
            tabLayout.childAlignment = TextAnchor.MiddleCenter;
            backpackPanel.tabContainer = tabContainer.transform;
            
            // Tab按钮
            backpackPanel.characterTabBtn = CreateTabButton(tabContainer.transform, "主角装扮", true);
            backpackPanel.furnitureTabBtn = CreateTabButton(tabContainer.transform, "家园装扮", false);
            
            // 内容区域
            backpackPanel.characterContent = CreateScrollView(panelObj.transform, "CharacterContent", true);
            backpackPanel.furnitureContent = CreateScrollView(panelObj.transform, "FurnitureContent", false);
            
            // 获取容器引用
            var charViewport = backpackPanel.characterContent.transform.Find("Viewport");
            if (charViewport != null)
            {
                var charContent = charViewport.Find("Content");
                if (charContent != null) backpackPanel.outfitListContainer = charContent;
            }
            
            var furnViewport = backpackPanel.furnitureContent.transform.Find("Viewport");
            if (furnViewport != null)
            {
                var furnContent = furnViewport.Find("Content");
                if (furnContent != null) backpackPanel.furnitureListContainer = furnContent;
            }
            
            // 保存
            string prefabPath = prefabSavePath + "BackpackPanel.prefab";
            PrefabUtility.SaveAsPrefabAsset(panelObj, prefabPath);
            DestroyImmediate(canvasObj);
            
            Debug.Log($"[BackpackSystemSetupWizard] 创建: {prefabPath}");
        }
        
        private void CreateFurnitureItem()
        {
            GameObject itemObj = new GameObject("FurnitureItem", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            
            var rect = itemObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(500, 100);
            itemObj.GetComponent<Image>().color = new Color(1f, 1f, 1f);
            
            var itemUI = itemObj.AddComponent<FluffyGeometry.UI.FurnitureItemUI>();
            
            // 选中边框
            GameObject borderObj = new GameObject("SelectedBorder", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            borderObj.transform.SetParent(itemObj.transform);
            var borderRect = borderObj.GetComponent<RectTransform>();
            borderRect.anchorMin = Vector2.zero;
            borderRect.anchorMax = Vector2.one;
            borderRect.offsetMin = Vector2.zero;
            borderRect.offsetMax = Vector2.zero;
            borderObj.GetComponent<Image>().color = new Color(1f, 0.8f, 0.4f, 0.3f);
            borderObj.SetActive(false);
            itemUI.selectedBorder = borderObj;
            
            // 图标
            GameObject iconObj = new GameObject("Icon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            iconObj.transform.SetParent(itemObj.transform);
            var iconRect = iconObj.GetComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0, 0.5f);
            iconRect.anchorMax = new Vector2(0, 0.5f);
            iconRect.pivot = new Vector2(0, 0.5f);
            iconRect.anchoredPosition = new Vector2(20, 0);
            iconRect.sizeDelta = new Vector2(80, 80);
            iconObj.GetComponent<Image>().color = new Color(0.9f, 0.9f, 0.9f);
            itemUI.furnitureIcon = iconObj.GetComponent<Image>();
            
            // 名称
            itemUI.furnitureNameText = CreateText(itemObj.transform, "Name", "家具名称", 24, TextAnchor.MiddleLeft, 
                new Vector2(0, 0.5f), new Vector2(1, 0.5f), new Vector2(0, 0.5f), new Vector2(120, 0), new Vector2(-240, 40));
            
            // 装饰按钮
            GameObject btnObj = new GameObject("DecorateBtn", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            btnObj.transform.SetParent(itemObj.transform);
            var btnRect = btnObj.GetComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(1, 0.5f);
            btnRect.anchorMax = new Vector2(1, 0.5f);
            btnRect.pivot = new Vector2(1, 0.5f);
            btnRect.anchoredPosition = new Vector2(-20, 0);
            btnRect.sizeDelta = new Vector2(100, 50);
            btnObj.GetComponent<Image>().color = new Color(0.4f, 0.8f, 0.4f);
            
            itemUI.decorateBtn = btnObj.GetComponent<Button>();
            itemUI.decorateBtnText = CreateText(btnObj.transform, "Text", "装饰", 20, TextAnchor.MiddleCenter, 
                Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), new Vector2(5, 5), new Vector2(-10, -10));
            
            btnObj.SetActive(false);
            
            // 保存
            string prefabPath = prefabSavePath + "FurnitureItem.prefab";
            PrefabUtility.SaveAsPrefabAsset(itemObj, prefabPath);
            DestroyImmediate(itemObj);
            
            Debug.Log($"[BackpackSystemSetupWizard] 创建: {prefabPath}");
        }
        
        private void CreateFurnitureEditController()
        {
            GameObject controllerObj = new GameObject("FurnitureEditController");
            var controller = controllerObj.AddComponent<FluffyGeometry.Home.FurnitureEditController>();
            
            // 创建画布
            GameObject canvasObj = new GameObject("ToolbarCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObj.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.transform.SetParent(controllerObj.transform);
            
            // 工具栏面板
            GameObject panelObj = new GameObject("ToolbarPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            panelObj.transform.SetParent(canvasObj.transform);
            
            var rect = panelObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(400, 80);
            panelObj.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
            
            controller.toolbarPanel = panelObj;
            
            // 布局
            var layout = panelObj.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(20, 20, 10, 10);
            layout.spacing = 20;
            layout.childAlignment = TextAnchor.MiddleCenter;
            
            // 翻转按钮
            controller.flipBtn = CreateToolbarButton(panelObj.transform, "翻转", new Color(0.4f, 0.6f, 1f));
            
            // 缩放滑条
            CreateScaleSlider(panelObj.transform, controller);
            
            // 确认按钮
            controller.confirmBtn = CreateToolbarButton(panelObj.transform, "确认", new Color(0.4f, 0.8f, 0.4f));
            
            panelObj.SetActive(false);
            
            // 保存
            string prefabPath = prefabSavePath + "FurnitureEditController.prefab";
            PrefabUtility.SaveAsPrefabAsset(controllerObj, prefabPath);
            DestroyImmediate(controllerObj);
            
            Debug.Log($"[BackpackSystemSetupWizard] 创建: {prefabPath}");
        }
        
        private void CreateSampleFurniture()
        {
            // 1. 云朵沙发
            var sofa = CreateFurnitureData("sofa_cloud_001", "云朵沙发", "软绵绵的云朵沙发，坐上去就像飘在天上", FurnitureCategory.座椅);
            SaveFurnitureAsset(sofa, "Furniture_Sofa_Cloud.asset");
            
            // 2. 几何茶几
            var table = CreateFurnitureData("table_geo_001", "几何茶几", "简洁的几何造型茶几", FurnitureCategory.桌子);
            table.canFlip = false;
            table.minScale = 0.8f;
            table.maxScale = 1.3f;
            SaveFurnitureAsset(table, "Furniture_Table_Geo.asset");
            
            // 3. 毛线球吊灯
            var light = CreateFurnitureData("light_yarn_001", "毛线球吊灯", "温暖的毛线球造型吊灯", FurnitureCategory.灯具);
            light.canFlip = false;
            light.minScale = 0.5f;
            light.maxScale = 1.2f;
            SaveFurnitureAsset(light, "Furniture_Light_Yarn.asset");
            
            Debug.Log($"[BackpackSystemSetupWizard] 创建3个示例家具数据");
        }
        
        private FluffyGeometry.Home.FurnitureData CreateFurnitureData(string id, string name, string desc, FurnitureCategory category)
        {
            var data = ScriptableObject.CreateInstance<FluffyGeometry.Home.FurnitureData>();
            data.furnitureId = id;
            data.furnitureName = name;
            data.description = desc;
            data.category = category;
            data.defaultScale = 1f;
            data.minScale = 0.5f;
            data.maxScale = 2f;
            data.canFlip = true;
            data.isUnlocked = true;
            return data;
        }
        
        private void SaveFurnitureAsset(FluffyGeometry.Home.FurnitureData data, string fileName)
        {
            string path = furnitureDataPath + fileName;
            AssetDatabase.CreateAsset(data, path);
        }
        
        // 辅助方法
        private void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                AssetDatabase.Refresh();
            }
        }
        
        private void PingDirectory(string path)
        {
            Object folder = AssetDatabase.LoadAssetAtPath<Object>(path);
            if (folder != null)
            {
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = folder;
                EditorGUIUtility.PingObject(folder);
            }
        }
        
        private Text CreateText(Transform parent, string name, string text, int fontSize, TextAnchor alignment, 
            Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform), typeof(Text));
            obj.transform.SetParent(parent);
            
            var rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = sizeDelta;
            
            var txt = obj.GetComponent<Text>();
            txt.text = text;
            txt.fontSize = fontSize;
            txt.alignment = alignment;
            txt.color = new Color(0.2f, 0.2f, 0.2f);
            
            return txt;
        }
        
        private Button CreateTabButton(Transform parent, string text, bool isActive)
        {
            GameObject btnObj = new GameObject($"{text}Tab", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            btnObj.transform.SetParent(parent);
            
            var rect = btnObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(180, 50);
            
            var image = btnObj.GetComponent<Image>();
            image.color = isActive ? new Color(1f, 0.8f, 0.6f) : new Color(0.8f, 0.8f, 0.8f);
            
            CreateText(btnObj.transform, "Text", text, 24, TextAnchor.MiddleCenter, 
                Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            
            return btnObj.GetComponent<Button>();
        }
        
        private Button CreateToolbarButton(Transform parent, string text, Color color)
        {
            GameObject btnObj = new GameObject($"{text}Btn", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            btnObj.transform.SetParent(parent);
            
            var rect = btnObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(80, 50);
            
            btnObj.GetComponent<Image>().color = color;
            
            CreateText(btnObj.transform, "Text", text, 20, TextAnchor.MiddleCenter, 
                Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), new Vector2(5, 5), new Vector2(-10, -10));
            
            return btnObj.GetComponent<Button>();
        }
        
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
            
            scrollObj.GetComponent<Image>().color = new Color(0.95f, 0.95f, 0.95f);
            
            var scrollRect = scrollObj.GetComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            
            // Viewport
            GameObject viewportObj = new GameObject("Viewport", typeof(RectTransform), typeof(CanvasRenderer), typeof(Mask), typeof(Image));
            viewportObj.transform.SetParent(scrollObj.transform);
            var viewportRect = viewportObj.GetComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = Vector2.zero;
            viewportRect.offsetMax = Vector2.zero;
            viewportObj.GetComponent<Mask>().showMaskGraphic = false;
            scrollRect.viewport = viewportRect;
            
            // Content
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
            layout.childForceExpandWidth = true;
            
            contentObj.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            scrollRect.content = contentRect;
            
            scrollObj.SetActive(active);
            
            return scrollObj;
        }
        
        private void CreateScaleSlider(Transform parent, FluffyGeometry.Home.FurnitureEditController controller)
        {
            GameObject container = new GameObject("ScaleSliderContainer", typeof(RectTransform));
            container.transform.SetParent(parent);
            container.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 60);
            
            GameObject sliderObj = new GameObject("ScaleSlider", typeof(RectTransform), typeof(Slider));
            sliderObj.transform.SetParent(container.transform);
            
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
            
            // Background
            GameObject bgObj = new GameObject("Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            bgObj.transform.SetParent(sliderObj.transform);
            var bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            bgObj.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f);
            
            // Fill Area
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
            
            // Handle Area
            GameObject handleAreaObj = new GameObject("Handle Slide Area", typeof(RectTransform));
            handleAreaObj.transform.SetParent(sliderObj.transform);
            var handleAreaRect = handleAreaObj.GetComponent<RectTransform>();
            handleAreaRect.anchorMin = Vector2.zero;
            handleAreaRect.anchorMax = Vector2.one;
            handleAreaRect.offsetMin = new Vector2(10, 0);
            handleAreaRect.offsetMax = new Vector2(-10, 0);
            
            GameObject handleObj = new GameObject("Handle", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            handleObj.transform.SetParent(handleAreaObj.transform);
            handleObj.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 30);
            handleObj.GetComponent<Image>().color = Color.white;
            
            slider.fillRect = fillRect;
            slider.handleRect = handleObj.GetComponent<RectTransform>();
            slider.targetGraphic = handleObj.GetComponent<Image>();
            
            controller.scaleSlider = slider;
            
            // Value text
            controller.scaleValueText = CreateText(container.transform, "ScaleValue", "1.0x", 16, TextAnchor.MiddleCenter, 
                new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), new Vector2(0, 0), new Vector2(0, 20));
            controller.scaleValueText.color = Color.white;
        }
    }
}
