using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

namespace GeometryWarrior.Editor
{
    /// <summary>
    /// UI生成工具 - 一键创建家园UI预制体
    /// </summary>
    public class UIBuilderTool : EditorWindow
    {
        private string savePath = "Assets/Prefabs/UI";
        private bool createAsPrefab = true;
        
        [MenuItem("绒毛几何物语/UI工具/生成OutfitPanel")]
        public static void ShowWindow()
        {
            GetWindow<UIBuilderTool>("UI生成工具");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("🎨 UI生成工具", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("保存路径:", EditorStyles.boldLabel);
            savePath = EditorGUILayout.TextField(savePath);
            
            EditorGUILayout.Space();
            
            createAsPrefab = EditorGUILayout.Toggle("保存为预制体", createAsPrefab);
            
            EditorGUILayout.Space();
            
            GUI.backgroundColor = new Color(0.66f, 0.9f, 0.81f);
            if (GUILayout.Button("生成 OutfitPanelNew", GUILayout.Height(50)))
            {
                GenerateOutfitPanel();
            }
            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("生成 HomeHUD", GUILayout.Height(40)))
            {
                GenerateHomeHUD();
            }
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("生成 PartItem 预制体", GUILayout.Height(40)))
            {
                GeneratePartItemPrefab();
            }
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("生成 BackpackPanel", GUILayout.Height(40)))
            {
                GenerateBackpackPanel();
            }
        }
        
        #region OutfitPanelNew 生成
        
        private void GenerateOutfitPanel()
        {
            // 确保保存目录存在
            if (!AssetDatabase.IsValidFolder(savePath))
            {
                System.IO.Directory.CreateDirectory(savePath);
                AssetDatabase.Refresh();
            }
            
            // 创建 Canvas（如果没有）
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                canvas = canvasGO.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 100;
                
                CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(750, 1334);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;
            }
            
            // 创建 OutfitPanelNew
            GameObject panelObj = new GameObject("OutfitPanelNew", typeof(RectTransform));
            panelObj.transform.SetParent(canvas.transform, false);
            
            RectTransform panelRect = panelObj.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            
            // 创建子元素
            GameObject overlay = CreateOverlay(panelObj.transform);
            GameObject content = CreateContent(panelObj.transform);
            
            // 添加脚本并配置
            OutfitPanelNew panelScript = panelObj.AddComponent<OutfitPanelNew>();
            ConfigureOutfitPanelScript(panelScript, overlay, content);
            
            // 默认隐藏
            panelObj.SetActive(false);
            
            // 保存为预制体
            if (createAsPrefab)
            {
                string prefabPath = $"{savePath}/OutfitPanelNew.prefab";
                #if UNITY_2018_3_OR_NEWER
                PrefabUtility.SaveAsPrefabAsset(panelObj, prefabPath);
                #else
                PrefabUtility.CreatePrefab(prefabPath, panelObj);
                #endif
                
                DestroyImmediate(panelObj);
                
                EditorUtility.DisplayDialog("完成", $"OutfitPanelNew 预制体已创建！\n路径: {prefabPath}", "确定");
                
                // 选中预制体
                AssetDatabase.Refresh();
                Object prefab = AssetDatabase.LoadAssetAtPath<Object>(prefabPath);
                Selection.activeObject = prefab;
            }
            else
            {
                Selection.activeGameObject = panelObj;
                EditorUtility.DisplayDialog("完成", "OutfitPanelNew 已在场景中创建！", "确定");
            }
        }
        
        private GameObject CreateOverlay(Transform parent)
        {
            GameObject overlay = new GameObject("Overlay", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
            overlay.transform.SetParent(parent, false);
            
            RectTransform rect = overlay.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            Image image = overlay.GetComponent<Image>();
            image.color = new Color(0, 0, 0, 0);
            
            CanvasGroup canvasGroup = overlay.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            
            return overlay;
        }
        
        private GameObject CreateContent(Transform parent)
        {
            GameObject content = new GameObject("Content", typeof(RectTransform), typeof(Image));
            content.transform.SetParent(parent, false);
            
            RectTransform rect = content.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 0);
            rect.pivot = new Vector2(0.5f, 0);
            rect.sizeDelta = new Vector2(0, 1100);
            rect.anchoredPosition = Vector2.zero;
            
            Image image = content.GetComponent<Image>();
            image.color = new Color(1f, 0.96f, 0.97f); // #FFF5F7
            
            // 创建子元素
            GameObject topBar = CreateTopBar(content.transform);
            GameObject charPreview = CreateCharacterPreview(content.transform);
            GameObject categoryTabs = CreateCategoryTabs(content.transform);
            GameObject partsList = CreatePartsList(content.transform);
            GameObject bottomBar = CreateBottomBar(content.transform);
            
            // 添加 Vertical Layout Group
            VerticalLayoutGroup layout = content.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(20, 20, 20, 20);
            layout.spacing = 15;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            
            // 设置子元素高度
            SetPreferredHeight(topBar, 80);
            SetPreferredHeight(charPreview, 400);
            SetPreferredHeight(categoryTabs, 100);
            SetPreferredHeight(partsList, 350);
            SetPreferredHeight(bottomBar, 100);
            
            content.name = "Content";
            return content;
        }
        
        private GameObject CreateTopBar(Transform parent)
        {
            GameObject topBar = new GameObject("TopBar", typeof(RectTransform), typeof(Image));
            topBar.transform.SetParent(parent, false);
            
            Image image = topBar.GetComponent<Image>();
            image.color = new Color(1f, 0.72f, 0.77f); // #FFB7C5
            
            // 关闭按钮
            GameObject closeBtn = CreateButton(topBar.transform, "CloseButton", "✕", 60, 60);
            SetAnchors(closeBtn, Vector2.zero, Vector2.zero);
            SetAnchoredPosition(closeBtn, new Vector2(50, 40));
            
            // 标题
            GameObject titleObj = new GameObject("Title", typeof(RectTransform), typeof(TextMeshProUGUI));
            titleObj.transform.SetParent(topBar.transform, false);
            TextMeshProUGUI titleText = titleObj.GetComponent<TextMeshProUGUI>();
            titleText.text = "🎀 换装间";
            titleText.fontSize = 36;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = new Color(0.365f, 0.251f, 0.216f); // #5D4037
            
            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.sizeDelta = new Vector2(300, 60);
            titleRect.anchoredPosition = Vector2.zero;
            
            // 金币
            GameObject coinObj = new GameObject("Coin", typeof(RectTransform), typeof(TextMeshProUGUI));
            coinObj.transform.SetParent(topBar.transform, false);
            TextMeshProUGUI coinText = coinObj.GetComponent<TextMeshProUGUI>();
            coinText.text = "💰 999";
            coinText.fontSize = 28;
            coinText.alignment = TextAlignmentOptions.Right;
            coinText.color = new Color(0.365f, 0.251f, 0.216f);
            
            RectTransform coinRect = coinObj.GetComponent<RectTransform>();
            coinRect.anchorMin = Vector2.right;
            coinRect.anchorMax = Vector2.right;
            coinRect.pivot = Vector2.right;
            coinRect.sizeDelta = new Vector2(150, 60);
            coinRect.anchoredPosition = new Vector2(-20, 0);
            
            return topBar;
        }
        
        private GameObject CreateCharacterPreview(Transform parent)
        {
            GameObject preview = new GameObject("CharacterPreview", typeof(RectTransform), typeof(Image));
            preview.transform.SetParent(parent, false);
            
            Image image = preview.GetComponent<Image>();
            image.color = new Color(1f, 0.894f, 0.925f); // #FFE4EC
            
            // 角色图片
            GameObject charImage = new GameObject("CharacterImage", typeof(RectTransform), typeof(Image));
            charImage.transform.SetParent(preview.transform, false);
            
            RectTransform charRect = charImage.GetComponent<RectTransform>();
            charRect.anchorMin = new Vector2(0.5f, 0.5f);
            charRect.anchorMax = new Vector2(0.5f, 0.5f);
            charRect.sizeDelta = new Vector2(300, 300);
            charRect.anchoredPosition = Vector2.zero;
            
            // 旋转按钮
            GameObject rotateLeft = CreateButton(preview.transform, "RotateLeft", "↺", 60, 60);
            SetAnchors(rotateLeft, Vector2.zero, Vector2.zero);
            SetAnchoredPosition(rotateLeft, new Vector2(50, 200));
            
            GameObject rotateRight = CreateButton(preview.transform, "RotateRight", "↻", 60, 60);
            SetAnchors(rotateRight, Vector2.right, Vector2.right);
            SetAnchoredPosition(rotateRight, new Vector2(-50, 200));
            
            return preview;
        }
        
        private GameObject CreateCategoryTabs(Transform parent)
        {
            GameObject tabsObj = new GameObject("CategoryTabs", typeof(RectTransform));
            tabsObj.transform.SetParent(parent, false);
            
            HorizontalLayoutGroup layout = tabsObj.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 10;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
            
            string[] tabNames = new string[] { "🎀 头饰", "👒 帽子", "👓 眼镜", "📿 围巾", "🎒 背饰" };
            
            for (int i = 0; i < tabNames.Length; i++)
            {
                GameObject tab = CreateTabButton(tabsObj.transform, $"Tab_{i}", tabNames[i]);
            }
            
            return tabsObj;
        }
        
        private GameObject CreateTabButton(Transform parent, string name, string text)
        {
            GameObject tab = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            tab.transform.SetParent(parent, false);
            
            RectTransform rect = tab.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(120, 80);
            
            Image image = tab.GetComponent<Image>();
            image.color = new Color(1f, 0.96f, 0.97f); // #FFF5F7
            
            // 文字
            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textObj.transform.SetParent(tab.transform, false);
            
            TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 24;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = new Color(0.365f, 0.251f, 0.216f);
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            return tab;
        }
        
        private GameObject CreatePartsList(Transform parent)
        {
            GameObject scrollObj = new GameObject("PartsList", typeof(RectTransform), typeof(ScrollRect), typeof(Image));
            scrollObj.transform.SetParent(parent, false);
            
            Image bgImage = scrollObj.GetComponent<Image>();
            bgImage.color = new Color(1f, 1f, 1f, 0.3f);
            
            // Viewport
            GameObject viewport = new GameObject("Viewport", typeof(RectTransform), typeof(Mask), typeof(Image));
            viewport.transform.SetParent(scrollObj.transform, false);
            
            RectTransform vpRect = viewport.GetComponent<RectTransform>();
            vpRect.anchorMin = Vector2.zero;
            vpRect.anchorMax = Vector2.one;
            vpRect.offsetMin = new Vector2(10, 10);
            vpRect.offsetMax = new Vector2(-10, -10);
            
            Image vpImage = viewport.GetComponent<Image>();
            vpImage.color = new Color(1f, 1f, 1f, 0.1f);
            
            // Content
            GameObject content = new GameObject("Content", typeof(RectTransform), typeof(GridLayoutGroup), typeof(ContentSizeFitter));
            content.transform.SetParent(viewport.transform, false);
            
            GridLayoutGroup grid = content.GetComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(120, 160);
            grid.spacing = new Vector2(15, 15);
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.childAlignment = TextAnchor.UpperCenter;
            grid.constraint = GridLayoutGroup.Constraint.Flexible;
            
            ContentSizeFitter fitter = content.GetComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            
            RectTransform contentRect = content.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.sizeDelta = Vector2.zero;
            
            // 配置 ScrollRect
            ScrollRect scroll = scrollObj.GetComponent<ScrollRect>();
            scroll.content = contentRect;
            scroll.viewport = vpRect;
            scroll.horizontal = true;
            scroll.vertical = false;
            scroll.movementType = ScrollRect.MovementType.Elastic;
            
            return scrollObj;
        }
        
        private GameObject CreateBottomBar(Transform parent)
        {
            GameObject bottomBar = new GameObject("BottomBar", typeof(RectTransform));
            bottomBar.transform.SetParent(parent, false);
            
            HorizontalLayoutGroup layout = bottomBar.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 20;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
            
            // 一键卸下
            GameObject unequipBtn = CreateActionButton(bottomBar.transform, "UnequipAllBtn", "🗑️ 卸下全部", new Color(1f, 0.827f, 0.647f));
            
            // 一键换装
            GameObject quickBtn = CreateActionButton(bottomBar.transform, "QuickEquipBtn", "🎲 随机换装", new Color(0.827f, 0.886f, 0.812f));
            
            // 保存
            GameObject saveBtn = CreateActionButton(bottomBar.transform, "SaveBtn", "✨ 保存装扮", new Color(0.659f, 0.902f, 0.812f));
            
            return bottomBar;
        }
        
        private GameObject CreateActionButton(Transform parent, string name, string text, Color color)
        {
            GameObject btn = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            btn.transform.SetParent(parent, false);
            
            RectTransform rect = btn.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(200, 80);
            
            Image image = btn.GetComponent<Image>();
            image.color = color;
            
            // 文字 - 使用普通 UI Text 以支持中文
            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(Text));
            textObj.transform.SetParent(btn.transform, false);
            
            Text txt = textObj.GetComponent<Text>();
            txt.text = text;
            txt.fontSize = 24;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = new Color(0.365f, 0.251f, 0.216f);
            // 使用 Arial 作为默认字体
            txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            txt.fontStyle = FontStyle.Bold;
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            return btn;
        }
        
        private GameObject CreateButton(Transform parent, string name, string text, float width, float height)
        {
            GameObject btn = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            btn.transform.SetParent(parent, false);
            
            RectTransform rect = btn.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(width, height);
            
            Image image = btn.GetComponent<Image>();
            image.color = new Color(0.659f, 0.902f, 0.812f); // #A8E6CF
            
            // 文字
            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textObj.transform.SetParent(btn.transform, false);
            
            TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 24;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            return btn;
        }
        
        private void ConfigureOutfitPanelScript(OutfitPanelNew script, GameObject overlay, GameObject content)
        {
            // 通过反射或查找来设置字段
            // 这里简化处理，手动在Inspector中配置
            
            SerializedObject so = new SerializedObject(script);
            
            // 设置基础引用
            so.FindProperty("contentPanel").objectReferenceValue = content.GetComponent<RectTransform>();
            so.FindProperty("overlay").objectReferenceValue = overlay.GetComponent<CanvasGroup>();
            
            // 查找子元素并设置
            Transform contentTrans = content.transform;
            
            // Close Button
            Transform closeBtn = contentTrans.Find("TopBar/CloseButton");
            if (closeBtn != null)
                so.FindProperty("closeButton").objectReferenceValue = closeBtn.GetComponent<Button>();
            
            // Character Preview
            Transform charImage = contentTrans.Find("CharacterPreview/CharacterImage");
            if (charImage != null)
                so.FindProperty("characterPreview").objectReferenceValue = charImage.GetComponent<Image>();
            
            // Rotate Buttons
            Transform rotateLeft = contentTrans.Find("CharacterPreview/RotateLeftBtn");
            if (rotateLeft != null)
                so.FindProperty("rotateLeftBtn").objectReferenceValue = rotateLeft.GetComponent<Button>();
            
            Transform rotateRight = contentTrans.Find("CharacterPreview/RotateRightBtn");
            if (rotateRight != null)
                so.FindProperty("rotateRightBtn").objectReferenceValue = rotateRight.GetComponent<Button>();
            
            // Category Container
            Transform categoryTabs = contentTrans.Find("CategoryTabs");
            if (categoryTabs != null)
                so.FindProperty("categoryContainer").objectReferenceValue = categoryTabs;
            
            // Scroll Rect
            Transform partsList = contentTrans.Find("PartsList");
            if (partsList != null)
            {
                so.FindProperty("partsScrollRect").objectReferenceValue = partsList.GetComponent<ScrollRect>();
                so.FindProperty("partsContainer").objectReferenceValue = partsList.Find("Viewport/Content");
            }
            
            // Bottom Buttons
            Transform bottomBar = contentTrans.Find("BottomBar");
            if (bottomBar != null)
            {
                Transform saveBtn = bottomBar.Find("SaveBtn");
                if (saveBtn != null)
                    so.FindProperty("saveButton").objectReferenceValue = saveBtn.GetComponent<Button>();
                
                Transform unequipBtn = bottomBar.Find("UnequipAllBtn");
                if (unequipBtn != null)
                    so.FindProperty("unequipAllButton").objectReferenceValue = unequipBtn.GetComponent<Button>();
                
                Transform quickBtn = bottomBar.Find("QuickEquipBtn");
                if (quickBtn != null)
                    so.FindProperty("quickEquipButton").objectReferenceValue = quickBtn.GetComponent<Button>();
            }
            
            so.ApplyModifiedProperties();
        }
        
        #endregion
        
        #region 辅助方法
        
        private void SetAnchors(GameObject obj, Vector2 min, Vector2 max)
        {
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.pivot = new Vector2(0.5f, 0.5f);
        }
        
        private void SetAnchoredPosition(GameObject obj, Vector2 pos)
        {
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.anchoredPosition = pos;
        }
        
        private void SetPreferredHeight(GameObject obj, float height)
        {
            LayoutElement layout = obj.GetComponent<LayoutElement>();
            if (layout == null)
                layout = obj.AddComponent<LayoutElement>();
            layout.preferredHeight = height;
        }
        
        #endregion
        
        #region HomeHUD 生成
        
        private void GenerateHomeHUD()
        {
            // 确保保存目录存在
            if (!AssetDatabase.IsValidFolder(savePath))
            {
                System.IO.Directory.CreateDirectory(savePath);
                AssetDatabase.Refresh();
            }
            
            // 获取或创建 Canvas
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                canvas = canvasGO.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 50;
                
                CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(750, 1334);
            }
            
            // 创建 HomeHUD
            GameObject hudObj = new GameObject("HomeHUD", typeof(RectTransform));
            hudObj.transform.SetParent(canvas.transform, false);
            
            RectTransform hudRect = hudObj.GetComponent<RectTransform>();
            hudRect.anchorMin = Vector2.zero;
            hudRect.anchorMax = Vector2.one;
            hudRect.offsetMin = Vector2.zero;
            hudRect.offsetMax = Vector2.zero;
            
            // 创建子元素
            CreateJoystickPlaceholder(hudObj.transform);
            GameObject topLeft = CreateTopLeftButtons(hudObj.transform);
            GameObject topRight = CreateTopRightButtons(hudObj.transform);
            GameObject backpackBtn = CreateBackpackButton(hudObj.transform);
            GameObject npcHint = CreateNPCHint(hudObj.transform);
            GameObject exitHint = CreateExitHint(hudObj.transform);
            GameObject toastContainer = CreateToastContainer(hudObj.transform);
            
            // 添加脚本
            HomeHUD hudScript = hudObj.AddComponent<HomeHUD>();
            
            // 配置引用
            SerializedObject so = new SerializedObject(hudScript);
            
            // Joystick - 需要在场景中找
            Joystick joystick = FindObjectOfType<Joystick>();
            so.FindProperty("joystick").objectReferenceValue = joystick;
            
            // 按钮（返回按钮已移除 - 通过门离开家园）
            so.FindProperty("settingsButton").objectReferenceValue = topRight.transform.Find("SettingsButton")?.GetComponent<Button>();
            so.FindProperty("backpackButton").objectReferenceValue = backpackBtn.GetComponent<Button>();
            
            // 提示
            so.FindProperty("npcHintPanel").objectReferenceValue = npcHint;
            so.FindProperty("npcHintText").objectReferenceValue = npcHint.transform.Find("Text")?.GetComponent<TextMeshProUGUI>();
            so.FindProperty("exitHintPanel").objectReferenceValue = exitHint;
            so.FindProperty("exitHintText").objectReferenceValue = exitHint.transform.Find("Text")?.GetComponent<TextMeshProUGUI>();
            
            // Toast
            so.FindProperty("toastContainer").objectReferenceValue = toastContainer.transform;
            
            so.ApplyModifiedProperties();
            
            // 保存预制体
            if (createAsPrefab)
            {
                string prefabPath = $"{savePath}/HomeHUD.prefab";
                #if UNITY_2018_3_OR_NEWER
                PrefabUtility.SaveAsPrefabAsset(hudObj, prefabPath);
                #else
                PrefabUtility.CreatePrefab(prefabPath, hudObj);
                #endif
                
                DestroyImmediate(hudObj);
                
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("完成", $"HomeHUD 预制体已创建！\n路径: {prefabPath}", "确定");
                
                Object prefab = AssetDatabase.LoadAssetAtPath<Object>(prefabPath);
                Selection.activeObject = prefab;
            }
            else
            {
                Selection.activeGameObject = hudObj;
                EditorUtility.DisplayDialog("完成", "HomeHUD 已在场景中创建！", "确定");
            }
        }
        
        private void CreateJoystickPlaceholder(Transform parent)
        {
            // 创建摇杆位置标记（需要在场景中配置实际的Joystick）
            GameObject joystickMarker = new GameObject("[Joystick Placeholder]", typeof(RectTransform));
            joystickMarker.transform.SetParent(parent, false);
            
            RectTransform rect = joystickMarker.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.zero;
            rect.pivot = new Vector2(0, 0);
            rect.anchoredPosition = new Vector2(100, 100);
            rect.sizeDelta = new Vector2(200, 200);
        }
        
        private GameObject CreateTopLeftButtons(Transform parent)
        {
            // 返回按钮已移除 - 通过门离开家园
            // 保留空容器以备将来使用
            GameObject container = new GameObject("TopLeft", typeof(RectTransform));
            container.transform.SetParent(parent, false);
            
            RectTransform rect = container.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.up;
            rect.anchorMax = Vector2.up;
            rect.pivot = Vector2.up;
            rect.anchoredPosition = new Vector2(20, -20);
            rect.sizeDelta = new Vector2(200, 80);
            
            // 不再创建返回按钮 - 通过门离开家园
            // 如需添加其他左上角按钮，可在此创建
            
            return container;
        }
        
        private GameObject CreateTopRightButtons(Transform parent)
        {
            GameObject container = new GameObject("TopRight", typeof(RectTransform));
            container.transform.SetParent(parent, false);
            
            RectTransform rect = container.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.one;
            rect.anchorMax = Vector2.one;
            rect.pivot = Vector2.one;
            rect.anchoredPosition = new Vector2(-20, -20);
            rect.sizeDelta = new Vector2(200, 80);
            
            // 设置按钮
            GameObject settingsBtn = CreateCircleButton(container.transform, "SettingsButton", "⚙", 60);
            SetAnchors(settingsBtn, Vector2.right, Vector2.right);
            SetAnchoredPosition(settingsBtn, new Vector2(-30, -30));
            
            return container;
        }
        
        private GameObject CreateBackpackButton(Transform parent)
        {
            GameObject btn = CreateCircleButton(parent, "BackpackButton", "🎒", 80);
            
            RectTransform rect = btn.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.right;
            rect.anchorMax = Vector2.right;
            rect.pivot = Vector2.right;
            rect.anchoredPosition = new Vector2(-30, 30);
            
            // 设置更大的尺寸和颜色
            Image image = btn.GetComponent<Image>();
            image.color = new Color(1f, 0.827f, 0.647f); // #FFD3A5
            
            return btn;
        }
        
        private GameObject CreateNPCHint(Transform parent)
        {
            GameObject hint = new GameObject("NPCHint", typeof(RectTransform), typeof(Image));
            hint.transform.SetParent(parent, false);
            hint.SetActive(false);
            
            RectTransform rect = hint.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0);
            rect.sizeDelta = new Vector2(250, 60);
            rect.anchoredPosition = new Vector2(0, 100);
            
            Image image = hint.GetComponent<Image>();
            image.color = new Color(0.706f, 0.906f, 0.941f); // #B4E7F0
            
            // 文字
            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textObj.transform.SetParent(hint.transform, false);
            
            TextMeshProUGUI text = textObj.GetComponent<TextMeshProUGUI>();
            text.text = "💬 点击对话";
            text.fontSize = 24;
            text.alignment = TextAlignmentOptions.Center;
            text.color = new Color(0.365f, 0.251f, 0.216f);
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(10, 5);
            textRect.offsetMax = new Vector2(-10, -5);
            
            return hint;
        }
        
        private GameObject CreateExitHint(Transform parent)
        {
            GameObject hint = new GameObject("ExitHint", typeof(RectTransform), typeof(Image));
            hint.transform.SetParent(parent, false);
            hint.SetActive(false);
            
            RectTransform rect = hint.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0);
            rect.sizeDelta = new Vector2(200, 50);
            rect.anchoredPosition = new Vector2(0, 80);
            
            Image image = hint.GetComponent<Image>();
            image.color = new Color(1f, 0.827f, 0.647f); // #FFD3A5
            
            // 箭头 + 文字
            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textObj.transform.SetParent(hint.transform, false);
            
            TextMeshProUGUI text = textObj.GetComponent<TextMeshProUGUI>();
            text.text = "▲ 点击离开";
            text.fontSize = 22;
            text.alignment = TextAlignmentOptions.Center;
            text.color = new Color(0.365f, 0.251f, 0.216f);
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            return hint;
        }
        
        private GameObject CreateToastContainer(Transform parent)
        {
            GameObject container = new GameObject("ToastContainer", typeof(RectTransform));
            container.transform.SetParent(parent, false);
            
            RectTransform rect = container.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.8f);
            rect.anchorMax = new Vector2(0.5f, 0.9f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(400, 100);
            rect.anchoredPosition = Vector2.zero;
            
            return container;
        }
        
        private GameObject CreateCircleButton(Transform parent, string name, string text, float size)
        {
            GameObject btn = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            btn.transform.SetParent(parent, false);
            
            RectTransform rect = btn.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(size, size);
            
            Image image = btn.GetComponent<Image>();
            image.color = new Color(0.659f, 0.902f, 0.812f); // #A8E6CF
            
            // 文字
            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textObj.transform.SetParent(btn.transform, false);
            
            TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = (int)(size * 0.4f);
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            return btn;
        }
        
        #endregion
        
        #region PartItem 预制体生成
        
        private void GeneratePartItemPrefab()
        {
            // 确保目录存在
            if (!AssetDatabase.IsValidFolder(savePath))
            {
                System.IO.Directory.CreateDirectory(savePath);
                AssetDatabase.Refresh();
            }
            
            // 创建 PartItem
            GameObject partItem = new GameObject("OutfitPartItem", typeof(RectTransform), typeof(Image), typeof(Button));
            
            RectTransform rect = partItem.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(120, 160);
            
            Image image = partItem.GetComponent<Image>();
            image.color = Color.white;
            
            // Bg
            GameObject bg = new GameObject("Bg", typeof(RectTransform), typeof(Image));
            bg.transform.SetParent(partItem.transform, false);
            RectTransform bgRect = bg.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            
            // Icon
            GameObject icon = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            icon.transform.SetParent(partItem.transform, false);
            RectTransform iconRect = icon.GetComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.5f, 0.7f);
            iconRect.anchorMax = new Vector2(0.5f, 0.7f);
            iconRect.sizeDelta = new Vector2(80, 80);
            
            // Name
            GameObject nameObj = new GameObject("Name", typeof(RectTransform), typeof(TextMeshProUGUI));
            nameObj.transform.SetParent(partItem.transform, false);
            TextMeshProUGUI nameText = nameObj.GetComponent<TextMeshProUGUI>();
            nameText.text = "部件名称";
            nameText.fontSize = 18;
            nameText.alignment = TextAlignmentOptions.Center;
            nameText.color = new Color(0.365f, 0.251f, 0.216f);
            
            RectTransform nameRect = nameObj.GetComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 0);
            nameRect.anchorMax = new Vector2(1, 0.3f);
            nameRect.offsetMin = new Vector2(5, 5);
            nameRect.offsetMax = new Vector2(-5, -5);
            
            // Status
            GameObject statusObj = new GameObject("Status", typeof(RectTransform), typeof(TextMeshProUGUI));
            statusObj.transform.SetParent(partItem.transform, false);
            statusObj.SetActive(false);
            
            TextMeshProUGUI statusText = statusObj.GetComponent<TextMeshProUGUI>();
            statusText.text = "✅ 已装备";
            statusText.fontSize = 14;
            statusText.alignment = TextAlignmentOptions.Center;
            statusText.color = new Color(0.4f, 0.7f, 0.5f);
            
            RectTransform statusRect = statusObj.GetComponent<RectTransform>();
            statusRect.anchorMin = new Vector2(0, 0.7f);
            statusRect.anchorMax = new Vector2(1, 0.9f);
            statusRect.offsetMin = Vector2.zero;
            statusRect.offsetMax = Vector2.zero;
            
            // 保存预制体
            string prefabPath = $"{savePath}/OutfitPartItem.prefab";
            #if UNITY_2018_3_OR_NEWER
            PrefabUtility.SaveAsPrefabAsset(partItem, prefabPath);
            #else
            PrefabUtility.CreatePrefab(prefabPath, partItem);
            #endif
            
            DestroyImmediate(partItem);
            
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("完成", $"OutfitPartItem 预制体已创建！\n路径: {prefabPath}", "确定");
            
            Object prefab = AssetDatabase.LoadAssetAtPath<Object>(prefabPath);
            Selection.activeObject = prefab;
        }
        
        #endregion
        
        #region BackpackPanel 生成
        
        private void GenerateBackpackPanel()
        {
            // 确保保存目录存在
            if (!AssetDatabase.IsValidFolder(savePath))
            {
                System.IO.Directory.CreateDirectory(savePath);
                AssetDatabase.Refresh();
            }
            
            // 获取或创建 Canvas
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                canvas = canvasGO.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 100;
                
                CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(750, 1334);
            }
            
            // 创建 BackpackPanel
            GameObject panelObj = new GameObject("BackpackPanel", typeof(RectTransform));
            panelObj.transform.SetParent(canvas.transform, false);
            
            RectTransform panelRect = panelObj.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            
            // 创建子元素
            GameObject overlay = CreateOverlay(panelObj.transform);
            GameObject content = CreateBackpackContent(panelObj.transform);
            
            // 添加脚本
            BackpackPanel panelScript = panelObj.AddComponent<BackpackPanel>();
            
            // 配置引用
            SerializedObject so = new SerializedObject(panelScript);
            so.FindProperty("contentPanel").objectReferenceValue = content.GetComponent<RectTransform>();
            so.FindProperty("overlay").objectReferenceValue = overlay.GetComponent<CanvasGroup>();
            
            // 查找并设置引用
            Transform contentTrans = content.transform;
            
            // Close Button
            Transform closeBtn = contentTrans.Find("TopBar/CloseButton");
            if (closeBtn != null)
                so.FindProperty("closeButton").objectReferenceValue = closeBtn.GetComponent<Button>();
            
            // Tabs
            Transform tabs = contentTrans.Find("Tabs");
            if (tabs != null)
            {
                so.FindProperty("playerEquipTab").objectReferenceValue = tabs.Find("PlayerEquipTab")?.GetComponent<Button>();
                so.FindProperty("homeOutfitTab").objectReferenceValue = tabs.Find("HomeOutfitTab")?.GetComponent<Button>();
            }
            
            // Pages
            so.FindProperty("playerEquipPage").objectReferenceValue = contentTrans.Find("PlayerEquipPage")?.gameObject;
            so.FindProperty("homeOutfitPage").objectReferenceValue = contentTrans.Find("HomeOutfitPage")?.gameObject;
            
            // OutfitPanel Prefab - 需要手动拖入
            // so.FindProperty("outfitPanelPrefab").objectReferenceValue = ...;
            
            so.ApplyModifiedProperties();
            
            // 默认隐藏
            panelObj.SetActive(false);
            
            // 保存预制体
            if (createAsPrefab)
            {
                string prefabPath = $"{savePath}/BackpackPanel.prefab";
                #if UNITY_2018_3_OR_NEWER
                PrefabUtility.SaveAsPrefabAsset(panelObj, prefabPath);
                #else
                PrefabUtility.CreatePrefab(prefabPath, panelObj);
                #endif
                
                DestroyImmediate(panelObj);
                
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("完成", $"BackpackPanel 预制体已创建！\n路径: {prefabPath}\n\n注意：需要在Inspector中拖入 OutfitPanelNew 预制体引用！", "确定");
                
                Object prefab = AssetDatabase.LoadAssetAtPath<Object>(prefabPath);
                Selection.activeObject = prefab;
            }
            else
            {
                Selection.activeGameObject = panelObj;
                EditorUtility.DisplayDialog("完成", "BackpackPanel 已在场景中创建！\n\n注意：需要在Inspector中拖入 OutfitPanelNew 预制体引用！", "确定");
            }
        }
        
        private GameObject CreateBackpackContent(Transform parent)
        {
            GameObject content = new GameObject("Content", typeof(RectTransform), typeof(Image));
            content.transform.SetParent(parent, false);
            
            RectTransform rect = content.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.offsetMin = new Vector2(20, 20);
            rect.offsetMax = new Vector2(-20, -20);
            
            Image image = content.GetComponent<Image>();
            image.color = new Color(1f, 0.96f, 0.97f); // #FFF5F7
            
            // 创建子元素
            GameObject topBar = CreateBackpackTopBar(content.transform);
            GameObject tabs = CreateBackpackTabs(content.transform);
            GameObject playerPage = CreatePlayerEquipPage(content.transform);
            GameObject outfitPage = CreateHomeOutfitPage(content.transform);
            
            return content;
        }
        
        private GameObject CreateBackpackTopBar(Transform parent)
        {
            GameObject topBar = new GameObject("TopBar", typeof(RectTransform), typeof(Image));
            topBar.transform.SetParent(parent, false);
            
            RectTransform rect = topBar.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.sizeDelta = new Vector2(0, 80);
            rect.anchoredPosition = Vector2.zero;
            
            Image image = topBar.GetComponent<Image>();
            image.color = new Color(1f, 0.72f, 0.77f); // #FFB7C5
            
            // 标题
            GameObject titleObj = new GameObject("Title", typeof(RectTransform), typeof(TextMeshProUGUI));
            titleObj.transform.SetParent(topBar.transform, false);
            
            TextMeshProUGUI titleText = titleObj.GetComponent<TextMeshProUGUI>();
            titleText.text = "🎒 背包";
            titleText.fontSize = 36;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = new Color(0.365f, 0.251f, 0.216f);
            titleText.fontStyle = FontStyles.Bold;
            
            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.sizeDelta = new Vector2(200, 60);
            titleRect.anchoredPosition = Vector2.zero;
            
            // 关闭按钮
            GameObject closeBtn = CreateButton(topBar.transform, "CloseButton", "✕", 60, 60);
            SetAnchors(closeBtn, Vector2.right, Vector2.right);
            SetAnchoredPosition(closeBtn, new Vector2(-20, -40));
            
            return topBar;
        }
        
        private GameObject CreateBackpackTabs(Transform parent)
        {
            GameObject tabs = new GameObject("Tabs", typeof(RectTransform));
            tabs.transform.SetParent(parent, false);
            
            RectTransform rect = tabs.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.sizeDelta = new Vector2(0, 100);
            rect.anchoredPosition = new Vector2(0, -90);
            
            HorizontalLayoutGroup layout = tabs.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 20;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
            
            // 主角装备页签
            GameObject playerTab = CreateTabButton(tabs.transform, "PlayerEquipTab", "⚔️ 主角装备");
            SetLayoutElement(playerTab, 180, 70);
            
            // 家园装扮页签
            GameObject outfitTab = CreateTabButton(tabs.transform, "HomeOutfitTab", "🎀 家园装扮");
            SetLayoutElement(outfitTab, 180, 70);
            
            return tabs;
        }
        
        private GameObject CreatePlayerEquipPage(Transform parent)
        {
            GameObject page = new GameObject("PlayerEquipPage", typeof(RectTransform), typeof(Image));
            page.transform.SetParent(parent, false);
            
            RectTransform rect = page.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(10, 10);
            rect.offsetMax = new Vector2(-10, -110);
            
            Image image = page.GetComponent<Image>();
            image.color = new Color(1f, 1f, 1f, 0.5f);
            
            // 占位文字
            GameObject hintObj = new GameObject("Hint", typeof(RectTransform), typeof(TextMeshProUGUI));
            hintObj.transform.SetParent(page.transform, false);
            
            TextMeshProUGUI hintText = hintObj.GetComponent<TextMeshProUGUI>();
            hintText.text = "主角装备页面\n（待配置）";
            hintText.fontSize = 28;
            hintText.alignment = TextAlignmentOptions.Center;
            hintText.color = new Color(0.5f, 0.5f, 0.5f);
            
            RectTransform hintRect = hintObj.GetComponent<RectTransform>();
            hintRect.anchorMin = new Vector2(0.5f, 0.5f);
            hintRect.anchorMax = new Vector2(0.5f, 0.5f);
            hintRect.sizeDelta = new Vector2(300, 100);
            hintRect.anchoredPosition = Vector2.zero;
            
            return page;
        }
        
        private GameObject CreateHomeOutfitPage(Transform parent)
        {
            GameObject page = new GameObject("HomeOutfitPage", typeof(RectTransform), typeof(Image));
            page.transform.SetParent(parent, false);
            
            RectTransform rect = page.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(10, 10);
            rect.offsetMax = new Vector2(-10, -110);
            
            Image image = page.GetComponent<Image>();
            image.color = new Color(1f, 1f, 1f, 0.5f);
            
            // 占位文字（OutfitPanel会在这里实例化）
            GameObject hintObj = new GameObject("Hint", typeof(RectTransform), typeof(TextMeshProUGUI));
            hintObj.transform.SetParent(page.transform, false);
            
            TextMeshProUGUI hintText = hintObj.GetComponent<TextMeshProUGUI>();
            hintText.text = "家园装扮页面\n（OutfitPanelNew 会在这里显示）";
            hintText.fontSize = 24;
            hintText.alignment = TextAlignmentOptions.Center;
            hintText.color = new Color(0.5f, 0.5f, 0.5f);
            
            RectTransform hintRect = hintObj.GetComponent<RectTransform>();
            hintRect.anchorMin = new Vector2(0.5f, 0.5f);
            hintRect.anchorMax = new Vector2(0.5f, 0.5f);
            hintRect.sizeDelta = new Vector2(400, 100);
            hintRect.anchoredPosition = Vector2.zero;
            
            return page;
        }
        
        private void SetLayoutElement(GameObject obj, float width, float height)
        {
            LayoutElement layout = obj.GetComponent<LayoutElement>();
            if (layout == null)
                layout = obj.AddComponent<LayoutElement>();
            layout.preferredWidth = width;
            layout.preferredHeight = height;
        }
        
        #endregion
    }
}
