using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

namespace GeometryWarrior.Editor
{
    /// <summary>
    /// 主角装扮UI一键创建工具
    /// </summary>
    public class OutfitPanelSetupWizard : EditorWindow
    {
        private const string PANEL_NAME = "OutfitPanel";
        private const string PREFAB_PATH = "Assets/Prefabs/UI/" + PANEL_NAME + ".prefab";
        
        [MenuItem("绒毛几何物语/创建主角装扮面板")]
        public static void ShowWindow()
        {
            GetWindow<OutfitPanelSetupWizard>("创建装扮面板");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("主角装扮UI一键创建", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            EditorGUILayout.HelpBox(
                "此工具将创建完整的装扮面板，包括：\n" +
                "• 面板容器\n" +
                "• 分类标签（蝴蝶结、帽子、眼镜等）\n" +
                "• 部件列表（Grid布局）\n" +
                "• 预览区域\n" +
                "• 装备/卸下按钮\n" +
                "• 自动绑定OutfitPanel脚本", 
                MessageType.Info);
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("创建装扮面板", GUILayout.Height(40)))
            {
                CreateOutfitPanel();
            }
            
            GUILayout.Space(10);
            
            // 检查现有面板
            var existingPanel = GameObject.Find(PANEL_NAME);
            if (existingPanel != null)
            {
                EditorGUILayout.HelpBox("场景中已经存在 OutfitPanel", MessageType.Warning);
                if (GUILayout.Button("选中现有面板"))
                {
                    Selection.activeGameObject = existingPanel;
                }
            }
        }
        
        private void CreateOutfitPanel()
        {
            // 检查Canvas
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                EditorUtility.DisplayDialog("错误", "场景中没有Canvas！请先创建Canvas。", "确定");
                return;
            }
            
            // 检查是否已存在
            Transform existing = canvas.transform.Find(PANEL_NAME);
            if (existing != null)
            {
                if (!EditorUtility.DisplayDialog("确认", "场景中已存在OutfitPanel，是否删除并重新创建？", "是", "否"))
                {
                    Selection.activeGameObject = existing.gameObject;
                    return;
                }
                DestroyImmediate(existing.gameObject);
            }
            
            // 创建面板
            GameObject panel = CreatePanel(canvas.transform);
            
            // 创建标题
            CreateTitle(panel.transform);
            
            // 创建关闭按钮
            CreateCloseButton(panel.transform);
            
            // 创建分类标签区域
            Transform categoryContainer = CreateCategoryTabs(panel.transform);
            
            // 创建内容区域（左右布局）
            Transform contentArea = CreateContentArea(panel.transform);
            
            // 创建部件列表（左侧）
            Transform partListContainer = CreatePartList(contentArea);
            
            // 创建预览区域（右侧）
            Transform previewArea = CreatePreviewArea(contentArea);
            
            // 创建信息区域
            Transform infoArea = CreateInfoArea(previewArea);
            
            // 创建按钮区域
            Transform buttonArea = CreateButtonArea(previewArea);
            
            // 添加OutfitPanel脚本并绑定引用
            BindOutfitPanelScript(panel, categoryContainer, partListContainer, previewArea, buttonArea, infoArea);
            
            // 保存为预制体
            SaveAsPrefab(panel);
            
            // 选中
            Selection.activeGameObject = panel;
            EditorGUIUtility.PingObject(panel);
            
            Debug.Log("[OutfitPanelSetupWizard] 装扮面板创建完成！");
        }
        
        private GameObject CreatePanel(Transform parent)
        {
            GameObject panel = new GameObject(PANEL_NAME, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            panel.transform.SetParent(parent, false);
            
            RectTransform rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            Image bg = panel.GetComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
            
            panel.SetActive(false);
            
            return panel;
        }
        
        private void CreateTitle(Transform parent)
        {
            GameObject titleObj = new GameObject("Title", typeof(RectTransform), typeof(TextMeshProUGUI));
            titleObj.transform.SetParent(parent, false);
            
            RectTransform rect = titleObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.sizeDelta = new Vector2(0, 60);
            rect.anchoredPosition = new Vector2(0, -10);
            
            TextMeshProUGUI text = titleObj.GetComponent<TextMeshProUGUI>();
            text.text = "✦ 装扮小屋 ✦";
            text.fontSize = 36;
            text.alignment = TextAlignmentOptions.Center;
            text.color = new Color(1f, 0.85f, 0.6f);
        }
        
        private void CreateCloseButton(Transform parent)
        {
            GameObject btnObj = new GameObject("CloseButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            btnObj.transform.SetParent(parent, false);
            
            RectTransform rect = btnObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(1, 1);
            rect.sizeDelta = new Vector2(50, 50);
            rect.anchoredPosition = new Vector2(-20, -20);
            
            Image img = btnObj.GetComponent<Image>();
            img.color = new Color(0.8f, 0.3f, 0.3f);
            
            // 创建X文本
            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textObj.transform.SetParent(btnObj.transform, false);
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            TextMeshProUGUI text = textObj.GetComponent<TextMeshProUGUI>();
            text.text = "✕";
            text.fontSize = 30;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;
        }
        
        private Transform CreateCategoryTabs(Transform parent)
        {
            GameObject container = new GameObject("CategoryTabs", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            container.transform.SetParent(parent, false);
            
            RectTransform rect = container.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.sizeDelta = new Vector2(-60, 50);
            rect.anchoredPosition = new Vector2(0, -80);
            
            HorizontalLayoutGroup layout = container.GetComponent<HorizontalLayoutGroup>();
            layout.spacing = 10;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
            
            // 创建分类标签预制体
            CreateCategoryTabPrefab();
            
            return container.transform;
        }
        
        private void CreateCategoryTabPrefab()
        {
            string prefabPath = "Assets/Prefabs/UI/CategoryTab.prefab";
            
            // 检查是否已存在
            GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (existing != null) return;
            
            // 创建预制体
            GameObject tabObj = new GameObject("CategoryTab", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            
            RectTransform rect = tabObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(80, 40);
            
            Image img = tabObj.GetComponent<Image>();
            img.color = new Color(0.3f, 0.3f, 0.35f);
            
            // 创建文本
            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textObj.transform.SetParent(tabObj.transform, false);
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(5, 5);
            textRect.offsetMax = new Vector2(-5, -5);
            
            TextMeshProUGUI text = textObj.GetComponent<TextMeshProUGUI>();
            text.text = "分类";
            text.fontSize = 18;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;
            
            // 保存预制体
            EnsureDirectoryExists("Assets/Prefabs/UI");
            PrefabUtility.SaveAsPrefabAsset(tabObj, prefabPath);
            DestroyImmediate(tabObj);
            
            Debug.Log("[OutfitPanelSetupWizard] 创建分类标签预制体: " + prefabPath);
        }
        
        private Transform CreateContentArea(Transform parent)
        {
            GameObject content = new GameObject("ContentArea", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            content.transform.SetParent(parent, false);
            
            RectTransform rect = content.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.offsetMin = new Vector2(20, 150);
            rect.offsetMax = new Vector2(-20, -100);
            
            HorizontalLayoutGroup layout = content.GetComponent<HorizontalLayoutGroup>();
            layout.spacing = 20;
            layout.childAlignment = TextAnchor.UpperCenter;
            
            return content.transform;
        }
        
        private Transform CreatePartList(Transform parent)
        {
            // 容器
            GameObject container = new GameObject("PartListContainer", typeof(RectTransform), typeof(Image));
            container.transform.SetParent(parent, false);
            
            RectTransform rect = container.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(400, 0);
            
            Image bg = container.GetComponent<Image>();
            bg.color = new Color(0.15f, 0.15f, 0.18f, 0.5f);
            
            // 滚动视图
            GameObject scrollObj = new GameObject("ScrollView", typeof(RectTransform), typeof(ScrollRect));
            scrollObj.transform.SetParent(container.transform, false);
            
            RectTransform scrollRect = scrollObj.GetComponent<RectTransform>();
            scrollRect.anchorMin = Vector2.zero;
            scrollRect.anchorMax = Vector2.one;
            scrollRect.offsetMin = new Vector2(10, 10);
            scrollRect.offsetMax = new Vector2(-10, -10);
            
            ScrollRect scroll = scrollObj.GetComponent<ScrollRect>();
            scroll.horizontal = false;
            scroll.vertical = true;
            
            // Viewport
            GameObject viewport = new GameObject("Viewport", typeof(RectTransform), typeof(Mask), typeof(Image));
            viewport.transform.SetParent(scrollObj.transform, false);
            
            RectTransform vpRect = viewport.GetComponent<RectTransform>();
            vpRect.anchorMin = Vector2.zero;
            vpRect.anchorMax = Vector2.one;
            vpRect.offsetMin = Vector2.zero;
            vpRect.offsetMax = Vector2.zero;
            
            viewport.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            
            // Content (Grid Layout)
            GameObject content = new GameObject("Content", typeof(RectTransform), typeof(GridLayoutGroup), typeof(ContentSizeFitter));
            content.transform.SetParent(viewport.transform, false);
            
            RectTransform contentRect = content.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.anchoredPosition = Vector2.zero;
            
            GridLayoutGroup grid = content.GetComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(100, 100);
            grid.spacing = new Vector2(15, 15);
            grid.padding = new RectOffset(15, 15, 15, 15);
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.childAlignment = TextAnchor.UpperCenter;
            
            ContentSizeFitter fitter = content.GetComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            
            scroll.content = contentRect;
            scroll.viewport = vpRect;
            
            // 创建部件项预制体
            CreatePartItemPrefab();
            
            return content.transform;
        }
        
        private void CreatePartItemPrefab()
        {
            string prefabPath = "Assets/Prefabs/UI/OutfitItem.prefab";
            
            // 检查是否已存在
            GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (existing != null) return;
            
            // 创建预制体
            GameObject itemObj = new GameObject("OutfitItem", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            
            RectTransform rect = itemObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(100, 100);
            
            Image bg = itemObj.GetComponent<Image>();
            bg.color = new Color(0.25f, 0.25f, 0.3f);
            
            // 图标
            GameObject iconObj = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            iconObj.transform.SetParent(itemObj.transform, false);
            
            RectTransform iconRect = iconObj.GetComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.1f, 0.1f);
            iconRect.anchorMax = new Vector2(0.9f, 0.9f);
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;
            
            // 锁定图标（默认隐藏）
            GameObject lockObj = new GameObject("LockIcon", typeof(RectTransform), typeof(Image));
            lockObj.transform.SetParent(itemObj.transform, false);
            lockObj.SetActive(false);
            
            RectTransform lockRect = lockObj.GetComponent<RectTransform>();
            lockRect.anchorMin = new Vector2(0.3f, 0.3f);
            lockRect.anchorMax = new Vector2(0.7f, 0.7f);
            lockRect.offsetMin = Vector2.zero;
            lockRect.offsetMax = Vector2.zero;
            
            // 已装备标识（默认隐藏）
            GameObject equippedObj = new GameObject("EquippedIndicator", typeof(RectTransform), typeof(Image));
            equippedObj.transform.SetParent(itemObj.transform, false);
            equippedObj.SetActive(false);
            
            RectTransform equippedRect = equippedObj.GetComponent<RectTransform>();
            equippedRect.anchorMin = new Vector2(0.7f, 0.7f);
            equippedRect.anchorMax = new Vector2(1, 1);
            equippedRect.offsetMin = Vector2.zero;
            equippedRect.offsetMax = Vector2.zero;
            
            // 添加OutfitItemUI脚本
            OutfitItemUI itemUI = itemObj.AddComponent<OutfitItemUI>();
            // 通过反射设置私有字段
            var iconField = typeof(OutfitItemUI).GetField("iconImage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var lockField = typeof(OutfitItemUI).GetField("lockImage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var equippedField = typeof(OutfitItemUI).GetField("equippedIndicator", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var buttonField = typeof(OutfitItemUI).GetField("button", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (iconField != null) iconField.SetValue(itemUI, iconObj.GetComponent<Image>());
            if (lockField != null) lockField.SetValue(itemUI, lockObj.GetComponent<Image>());
            if (equippedField != null) equippedField.SetValue(itemUI, equippedObj);
            if (buttonField != null) buttonField.SetValue(itemUI, itemObj.GetComponent<Button>());
            
            // 保存预制体
            EnsureDirectoryExists("Assets/Prefabs/UI");
            PrefabUtility.SaveAsPrefabAsset(itemObj, prefabPath);
            DestroyImmediate(itemObj);
            
            Debug.Log("[OutfitPanelSetupWizard] 创建部件项预制体: " + prefabPath);
        }
        
        private Transform CreatePreviewArea(Transform parent)
        {
            GameObject area = new GameObject("PreviewArea", typeof(RectTransform), typeof(Image), typeof(VerticalLayoutGroup));
            area.transform.SetParent(parent, false);
            
            RectTransform rect = area.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, 0);
            
            Image bg = area.GetComponent<Image>();
            bg.color = new Color(0.12f, 0.12f, 0.15f, 0.8f);
            
            VerticalLayoutGroup layout = area.GetComponent<VerticalLayoutGroup>();
            layout.spacing = 15;
            layout.padding = new RectOffset(20, 20, 20, 20);
            layout.childAlignment = TextAnchor.UpperCenter;
            
            // 预览标题
            GameObject titleObj = new GameObject("PreviewTitle", typeof(RectTransform), typeof(TextMeshProUGUI));
            titleObj.transform.SetParent(area.transform, false);
            
            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.sizeDelta = new Vector2(0, 30);
            
            TextMeshProUGUI titleText = titleObj.GetComponent<TextMeshProUGUI>();
            titleText.text = "预览";
            titleText.fontSize = 24;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = new Color(0.8f, 0.8f, 0.8f);
            
            // 预览图容器
            GameObject previewContainer = new GameObject("PreviewContainer", typeof(RectTransform), typeof(Image));
            previewContainer.transform.SetParent(area.transform, false);
            
            RectTransform previewRect = previewContainer.GetComponent<RectTransform>();
            previewRect.sizeDelta = new Vector2(250, 250);
            
            Image previewBg = previewContainer.GetComponent<Image>();
            previewBg.color = new Color(0.2f, 0.2f, 0.25f);
            
            return previewContainer.transform;
        }
        
        private Transform CreateInfoArea(Transform parent)
        {
            GameObject area = new GameObject("InfoArea", typeof(RectTransform), typeof(VerticalLayoutGroup));
            area.transform.SetParent(parent, false);
            
            RectTransform rect = area.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, 120);
            
            VerticalLayoutGroup layout = area.GetComponent<VerticalLayoutGroup>();
            layout.spacing = 10;
            layout.childAlignment = TextAnchor.UpperCenter;
            
            // 部件名称
            GameObject nameObj = new GameObject("PartName", typeof(RectTransform), typeof(TextMeshProUGUI));
            nameObj.transform.SetParent(area.transform, false);
            
            RectTransform nameRect = nameObj.GetComponent<RectTransform>();
            nameRect.sizeDelta = new Vector2(0, 35);
            
            TextMeshProUGUI nameText = nameObj.GetComponent<TextMeshProUGUI>();
            nameText.text = "选择一个部件";
            nameText.fontSize = 22;
            nameText.alignment = TextAlignmentOptions.Center;
            nameText.color = new Color(1f, 0.9f, 0.6f);
            nameText.fontStyle = FontStyles.Bold;
            
            // 部件描述
            GameObject descObj = new GameObject("PartDesc", typeof(RectTransform), typeof(TextMeshProUGUI));
            descObj.transform.SetParent(area.transform, false);
            
            RectTransform descRect = descObj.GetComponent<RectTransform>();
            descRect.sizeDelta = new Vector2(0, 75);
            
            TextMeshProUGUI descText = descObj.GetComponent<TextMeshProUGUI>();
            descText.text = "点击左侧部件查看详情";
            descText.fontSize = 16;
            descText.alignment = TextAlignmentOptions.Top;
            descText.color = new Color(0.8f, 0.8f, 0.8f);
            
            return area.transform;
        }
        
        private Transform CreateButtonArea(Transform parent)
        {
            GameObject area = new GameObject("ButtonArea", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            area.transform.SetParent(parent, false);
            
            RectTransform rect = area.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, 50);
            
            HorizontalLayoutGroup layout = area.GetComponent<HorizontalLayoutGroup>();
            layout.spacing = 20;
            layout.childAlignment = TextAnchor.MiddleCenter;
            
            // 装备按钮
            GameObject equipBtn = CreateButton(area.transform, "装备", new Color(0.3f, 0.7f, 0.4f));
            equipBtn.name = "EquipButton";
            equipBtn.SetActive(false);
            
            // 卸下按钮
            GameObject unequipBtn = CreateButton(area.transform, "卸下", new Color(0.7f, 0.3f, 0.3f));
            unequipBtn.name = "UnequipButton";
            unequipBtn.SetActive(false);
            
            return area.transform;
        }
        
        private GameObject CreateButton(Transform parent, string text, Color color)
        {
            GameObject btnObj = new GameObject(text + "Button", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            btnObj.transform.SetParent(parent, false);
            
            RectTransform rect = btnObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(120, 45);
            
            Image img = btnObj.GetComponent<Image>();
            img.color = color;
            
            // 文本
            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textObj.transform.SetParent(btnObj.transform, false);
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(10, 10);
            textRect.offsetMax = new Vector2(-10, -10);
            
            TextMeshProUGUI textComp = textObj.GetComponent<TextMeshProUGUI>();
            textComp.text = text;
            textComp.fontSize = 20;
            textComp.alignment = TextAlignmentOptions.Center;
            textComp.color = Color.white;
            
            return btnObj;
        }
        
        private void BindOutfitPanelScript(GameObject panel, Transform categoryContainer, Transform partListContainer, 
            Transform previewArea, Transform buttonArea, Transform infoArea)
        {
            OutfitPanel outfitPanel = panel.GetComponent<OutfitPanel>();
            if (outfitPanel == null)
            {
                outfitPanel = panel.AddComponent<OutfitPanel>();
            }
            
            // 通过SerializedObject设置字段
            SerializedObject serializedObj = new SerializedObject(outfitPanel);
            
            // 设置字段
            serializedObj.FindProperty("panel").objectReferenceValue = panel;
            serializedObj.FindProperty("closeButton").objectReferenceValue = 
                panel.transform.Find("CloseButton")?.GetComponent<Button>();
            serializedObj.FindProperty("categoryTabContainer").objectReferenceValue = categoryContainer;
            serializedObj.FindProperty("categoryTabPrefab").objectReferenceValue = 
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/CategoryTab.prefab");
            serializedObj.FindProperty("partListContainer").objectReferenceValue = partListContainer;
            serializedObj.FindProperty("partItemPrefab").objectReferenceValue = 
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/OutfitItem.prefab");
            serializedObj.FindProperty("playerPreviewImage").objectReferenceValue = null; // 可选
            serializedObj.FindProperty("previewPartsContainer").objectReferenceValue = previewArea;
            serializedObj.FindProperty("previewPartPrefab").objectReferenceValue = null; // 可选
            serializedObj.FindProperty("selectedPartNameText").objectReferenceValue = 
                infoArea.Find("PartName")?.GetComponent<TextMeshProUGUI>();
            serializedObj.FindProperty("selectedPartDescText").objectReferenceValue = 
                infoArea.Find("PartDesc")?.GetComponent<TextMeshProUGUI>();
            serializedObj.FindProperty("equipButton").objectReferenceValue = 
                buttonArea.Find("EquipButton")?.GetComponent<Button>();
            serializedObj.FindProperty("unequipButton").objectReferenceValue = 
                buttonArea.Find("UnequipButton")?.GetComponent<Button>();
            
            serializedObj.ApplyModifiedProperties();
            
            Debug.Log("[OutfitPanelSetupWizard] OutfitPanel脚本绑定完成");
        }
        
        private void SaveAsPrefab(GameObject panel)
        {
            EnsureDirectoryExists("Assets/Prefabs/UI");
            
            // 保存为预制体
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(panel, PREFAB_PATH);
            if (prefab != null)
            {
                Debug.Log("[OutfitPanelSetupWizard] 预制体已保存: " + PREFAB_PATH);
            }
        }
        
        private void EnsureDirectoryExists(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                string parent = System.IO.Path.GetDirectoryName(path).Replace('\\', '/');
                string folderName = System.IO.Path.GetFileName(path);
                AssetDatabase.CreateFolder(parent, folderName);
            }
        }
    }
}
