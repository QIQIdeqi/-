using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;

namespace GeometryWarrior.Editor
{
    /// <summary>
    /// OutfitPanelNew 预制体重建工具
    /// 删除旧的并重新生成使用普通 Text 组件的版本
    /// </summary>
    public class OutfitPanelNewRebuilder : EditorWindow
    {
        private string savePath = "Assets/Prefabs/UI";
        
        [MenuItem("绒毛几何物语/工具/重建OutfitPanelNew预制体")]
        public static void ShowWindow()
        {
            GetWindow<OutfitPanelNewRebuilder>("重建OutfitPanelNew");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("🔨 重建 OutfitPanelNew 预制体", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox(
                "此工具会：\n" +
                "1. 删除旧的 OutfitPanelNew 预制体\n" +
                "2. 重新生成使用普通 Text 组件的新版本\n" +
                "3. 自动配置所有引用", 
                MessageType.Info);
            
            EditorGUILayout.Space();
            
            savePath = EditorGUILayout.TextField("保存路径:", savePath);
            
            EditorGUILayout.Space();
            
            GUI.backgroundColor = new Color(1f, 0.7f, 0.7f);
            if (GUILayout.Button("开始重建", GUILayout.Height(50)))
            {
                if (EditorUtility.DisplayDialog("确认重建", 
                    "这将删除旧的 OutfitPanelNew 预制体并创建新的。\n\n是否继续？", 
                    "重建", "取消"))
                {
                    RebuildOutfitPanelNew();
                }
            }
            GUI.backgroundColor = Color.white;
        }
        
        private void RebuildOutfitPanelNew()
        {
            try
            {
                // 1. 删除旧预制体
                DeleteOldPrefab();
                
                // 2. 创建新的
                CreateNewOutfitPanelNew();
                
                EditorUtility.DisplayDialog("完成", "OutfitPanelNew 预制体重建完成！", "确定");
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("错误", $"重建失败：{e.Message}", "确定");
                Debug.LogError($"[OutfitPanelNewRebuilder] 错误：{e}");
            }
        }
        
        private void DeleteOldPrefab()
        {
            string prefabPath = $"{savePath}/OutfitPanelNew.prefab";
            
            if (AssetDatabase.DeleteAsset(prefabPath))
            {
                Debug.Log($"[OutfitPanelNewRebuilder] 已删除旧预制体：{prefabPath}");
            }
            
            AssetDatabase.Refresh();
        }
        
        private void CreateNewOutfitPanelNew()
        {
            // 确保目录存在
            if (!AssetDatabase.IsValidFolder(savePath))
            {
                Directory.CreateDirectory(savePath);
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
            
            // 保存预制体
            string prefabPath = $"{savePath}/OutfitPanelNew.prefab";
            #if UNITY_2018_3_OR_NEWER
            PrefabUtility.SaveAsPrefabAsset(panelObj, prefabPath);
            #else
            PrefabUtility.CreatePrefab(prefabPath, panelObj);
            #endif
            
            DestroyImmediate(panelObj);
            
            AssetDatabase.Refresh();
            
            // 选中预制体
            Object prefab = AssetDatabase.LoadAssetAtPath<Object>(prefabPath);
            Selection.activeObject = prefab;
            
            Debug.Log($"[OutfitPanelNewRebuilder] 已创建新预制体：{prefabPath}");
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
            GameObject closeBtn = CreateButton(topBar.transform, "CloseButton", "X", 60, 60);
            SetAnchors(closeBtn, Vector2.zero, Vector2.zero);
            SetAnchoredPosition(closeBtn, new Vector2(50, 40));
            
            // 标题 - 使用普通 Text
            GameObject titleObj = new GameObject("Title", typeof(RectTransform), typeof(Text));
            titleObj.transform.SetParent(topBar.transform, false);
            Text titleText = titleObj.GetComponent<Text>();
            titleText.text = "换装间";
            titleText.fontSize = 36;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.color = new Color(0.365f, 0.251f, 0.216f);
            titleText.font = GetSimHeiFont();
            
            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.sizeDelta = new Vector2(200, 60);
            titleRect.anchoredPosition = Vector2.zero;
            
            // 金币 - 使用普通 Text
            GameObject coinObj = new GameObject("Coin", typeof(RectTransform), typeof(Text));
            coinObj.transform.SetParent(topBar.transform, false);
            Text coinText = coinObj.GetComponent<Text>();
            coinText.text = "金币: 999";
            coinText.fontSize = 28;
            coinText.alignment = TextAnchor.MiddleRight;
            coinText.color = new Color(0.365f, 0.251f, 0.216f);
            coinText.font = GetSimHeiFont();
            
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
            GameObject rotateLeft = CreateButton(preview.transform, "RotateLeftBtn", "<", 60, 60);
            SetAnchors(rotateLeft, Vector2.zero, Vector2.zero);
            SetAnchoredPosition(rotateLeft, new Vector2(50, 200));
            
            GameObject rotateRight = CreateButton(preview.transform, "RotateRightBtn", ">", 60, 60);
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
            
            string[] tabNames = new string[] { "头饰", "帽子", "眼镜", "围巾", "背饰" };
            
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
            
            // 设置按钮文字 - 使用普通 Text
            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(Text));
            textObj.transform.SetParent(tab.transform, false);
            
            Text txt = textObj.GetComponent<Text>();
            txt.text = text;
            txt.fontSize = 24;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = new Color(0.365f, 0.251f, 0.216f);
            txt.font = GetSimHeiFont();
            
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
            GameObject unequipBtn = CreateActionButton(bottomBar.transform, "UnequipAllBtn", "卸下全部", new Color(1f, 0.827f, 0.647f));
            
            // 一键换装
            GameObject quickBtn = CreateActionButton(bottomBar.transform, "QuickEquipBtn", "随机换装", new Color(0.827f, 0.886f, 0.812f));
            
            // 保存
            GameObject saveBtn = CreateActionButton(bottomBar.transform, "SaveBtn", "保存装扮", new Color(0.659f, 0.902f, 0.812f));
            
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
            
            // 文字 - 使用普通 Text
            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(Text));
            textObj.transform.SetParent(btn.transform, false);
            
            Text txt = textObj.GetComponent<Text>();
            txt.text = text;
            txt.fontSize = 24;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = new Color(0.365f, 0.251f, 0.216f);
            txt.font = GetSimHeiFont();
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
            
            // 文字 - 使用普通 Text
            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(Text));
            textObj.transform.SetParent(btn.transform, false);
            
            Text txt = textObj.GetComponent<Text>();
            txt.text = text;
            txt.fontSize = 24;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = Color.white;
            txt.font = GetSimHeiFont();
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            return btn;
        }
        
        private void ConfigureOutfitPanelScript(OutfitPanelNew script, GameObject overlay, GameObject content)
        {
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
        
        private Font GetSimHeiFont()
        {
            // 尝试获取 SIMHEI 字体，如果不存在则使用 Arial
            Font simhei = Resources.GetBuiltinResource<Font>("SIMHEI.ttf");
            if (simhei == null)
            {
                simhei = Font.CreateDynamicFontFromOSFont("SimHei", 24);
            }
            return simhei ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
        
        #endregion
    }
}
