using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using FluffyGeometry.UI;
using GeometryWarrior;

namespace GeometryWarrior.Editor
{
    /// <summary>
    /// BackpackPanel 完整重建工具
    /// </summary>
    public class BackpackPanelRebuilder : EditorWindow
    {
        [MenuItem("绒毛几何物语/重建/BackpackPanel")]
        public static void ShowWindow()
        {
            GetWindow<BackpackPanelRebuilder>("重建 BackpackPanel");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("✦ BackpackPanel 完整重建 ✦", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            EditorGUILayout.HelpBox(
                "如果 BackpackPanel 布局混乱或功能异常，\n" +
                "使用此工具重新创建完整的背包面板。", 
                MessageType.Warning);
            
            GUILayout.Space(10);
            
            EditorGUILayout.LabelField("重建将包括：", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("• 主面板容器");
            EditorGUILayout.LabelField("• Tab 标签（主角装扮/家园装扮）");
            EditorGUILayout.LabelField("• 主角装扮列表（带滚动）");
            EditorGUILayout.LabelField("• 家园装扮列表（带滚动）");
            EditorGUILayout.LabelField("• 关闭按钮");
            EditorGUILayout.LabelField("• 自动绑定所有脚本引用");
            
            GUILayout.Space(20);
            
            GUI.backgroundColor = new Color(1f, 0.4f, 0.4f);
            if (GUILayout.Button("删除现有并重新创建", GUILayout.Height(50)))
            {
                if (EditorUtility.DisplayDialog("确认", 
                    "这将删除场景中现有的 BackpackPanel 并重新创建。\n" +
                    "确定继续吗？", 
                    "确定", "取消"))
                {
                    RebuildBackpackPanel();
                }
            }
            GUI.backgroundColor = Color.white;
        }
        
        private void RebuildBackpackPanel()
        {
            // 查找 Canvas
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                EditorUtility.DisplayDialog("错误", "场景中没有 Canvas！请先创建 Canvas。", "确定");
                return;
            }
            
            // 删除现有的 BackpackPanel
            var existingPanels = FindObjectsOfType<BackpackPanel>();
            foreach (var existingPanel in existingPanels)
            {
                if (existingPanel != null && existingPanel.gameObject != null)
                {
                    DestroyImmediate(existingPanel.gameObject);
                    Debug.Log("[BackpackPanelRebuilder] 删除现有 BackpackPanel");
                }
            }
            
            // 创建新的 BackpackPanel
            GameObject panelObj = CreateBackpackPanel(canvas.transform);
            
            // 添加 BackpackPanel 脚本并绑定
            BackpackPanel panel = panelObj.GetComponent<BackpackPanel>();
            if (panel == null)
            {
                panel = panelObj.AddComponent<BackpackPanel>();
            }
            
            // 绑定引用
            BindBackpackPanelReferences(panel, panelObj);
            
            // 选中并高亮
            Selection.activeGameObject = panelObj;
            EditorGUIUtility.PingObject(panelObj);
            
            Debug.Log("[BackpackPanelRebuilder] BackpackPanel 重建完成！");
            EditorUtility.DisplayDialog("完成", 
                "BackpackPanel 已重新创建！\n\n" +
                "包含：\n" +
                "- 主角装扮页签（带滚动列表）\n" +
                "- 家园装扮页签（带滚动列表）\n" +
                "- 关闭按钮\n\n" +
                "请运行游戏测试。", 
                "确定");
        }
        
        private GameObject CreateBackpackPanel(Transform parent)
        {
            // 主面板
            GameObject panel = new GameObject("BackpackPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            panel.transform.SetParent(parent, false);
            panel.SetActive(false); // 默认隐藏
            
            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            
            panel.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.12f, 0.95f);
            
            // 标题
            CreateTitle(panel.transform);
            
            // 关闭按钮
            CreateCloseButton(panel.transform);
            
            // Tab 容器
            Transform tabContainer = CreateTabContainer(panel.transform);
            
            // 内容区域
            GameObject contentArea = new GameObject("ContentArea", typeof(RectTransform));
            contentArea.transform.SetParent(panel.transform, false);
            
            RectTransform contentRect = contentArea.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 0);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.offsetMin = new Vector2(20, 20);
            contentRect.offsetMax = new Vector2(-20, -80);
            
            // 主角装扮内容
            GameObject characterContent = CreateCharacterContent(contentArea.transform);
            
            // 家园装扮内容
            GameObject furnitureContent = CreateFurnitureContent(contentArea.transform);
            
            // 默认显示主角装扮
            furnitureContent.SetActive(false);
            
            return panel;
        }
        
        private void CreateTitle(Transform parent)
        {
            GameObject title = new GameObject("Title", typeof(RectTransform), typeof(Text));
            title.transform.SetParent(parent, false);
            
            RectTransform rect = title.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.sizeDelta = new Vector2(0, 50);
            rect.anchoredPosition = new Vector2(0, -10);
            
            Text text = title.GetComponent<Text>();
            text.text = "✦ 背包 ✦";
            text.fontSize = 32;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = new Color(1f, 0.85f, 0.6f);
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
        
        private void CreateCloseButton(Transform parent)
        {
            GameObject btn = new GameObject("CloseBtn", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            btn.transform.SetParent(parent, false);
            
            RectTransform rect = btn.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(1, 1);
            rect.sizeDelta = new Vector2(50, 50);
            rect.anchoredPosition = new Vector2(-20, -20);
            
            btn.GetComponent<Image>().color = new Color(0.8f, 0.3f, 0.3f);
            
            // X 文字
            GameObject txt = new GameObject("Text", typeof(RectTransform), typeof(Text));
            txt.transform.SetParent(btn.transform, false);
            txt.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            txt.GetComponent<RectTransform>().anchorMax = Vector2.one;
            txt.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            txt.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            
            Text text = txt.GetComponent<Text>();
            text.text = "✕";
            text.fontSize = 28;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
        
        private Transform CreateTabContainer(Transform parent)
        {
            GameObject container = new GameObject("TabContainer", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            container.transform.SetParent(parent, false);
            
            RectTransform rect = container.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.sizeDelta = new Vector2(-40, 50);
            rect.anchoredPosition = new Vector2(0, -70);
            
            HorizontalLayoutGroup layout = container.GetComponent<HorizontalLayoutGroup>();
            layout.spacing = 10;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            
            // 主角装扮 Tab
            CreateTabButton(container.transform, "主角装扮", "CharacterTab");
            
            // 家园装扮 Tab
            CreateTabButton(container.transform, "家园装扮", "FurnitureTab");
            
            return container.transform;
        }
        
        private void CreateTabButton(Transform parent, string text, string name)
        {
            GameObject btn = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            btn.transform.SetParent(parent, false);
            btn.GetComponent<RectTransform>().sizeDelta = new Vector2(120, 40);
            btn.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.35f);
            
            GameObject txt = new GameObject("Text", typeof(RectTransform), typeof(Text));
            txt.transform.SetParent(btn.transform, false);
            txt.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            txt.GetComponent<RectTransform>().anchorMax = Vector2.one;
            txt.GetComponent<RectTransform>().offsetMin = new Vector2(5, 5);
            txt.GetComponent<RectTransform>().offsetMax = new Vector2(-5, -5);
            
            Text t = txt.GetComponent<Text>();
            t.text = text;
            t.fontSize = 18;
            t.alignment = TextAnchor.MiddleCenter;
            t.color = Color.white;
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
        
        private GameObject CreateCharacterContent(Transform parent)
        {
            GameObject content = new GameObject("CharacterContent", typeof(RectTransform));
            content.transform.SetParent(parent, false);
            
            RectTransform rect = content.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            // 创建滚动视图
            CreateScrollView(content.transform, "OutfitScroll", out Transform contentTransform);
            
            // 添加布局组件
            SetupContentLayout(contentTransform);
            
            return content;
        }
        
        private GameObject CreateFurnitureContent(Transform parent)
        {
            GameObject content = new GameObject("FurnitureContent", typeof(RectTransform));
            content.transform.SetParent(parent, false);
            
            RectTransform rect = content.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            // 创建滚动视图
            CreateScrollView(content.transform, "FurnitureScroll", out Transform contentTransform);
            
            // 添加布局组件
            SetupContentLayout(contentTransform);
            
            return content;
        }
        
        private void CreateScrollView(Transform parent, string name, out Transform contentTransform)
        {
            // ScrollView
            GameObject scroll = new GameObject(name, typeof(RectTransform), typeof(ScrollRect), typeof(Image), typeof(Mask));
            scroll.transform.SetParent(parent, false);
            
            RectTransform scrollRect = scroll.GetComponent<RectTransform>();
            scrollRect.anchorMin = Vector2.zero;
            scrollRect.anchorMax = Vector2.one;
            scrollRect.offsetMin = new Vector2(10, 10);
            scrollRect.offsetMax = new Vector2(-10, -10);
            
            scroll.GetComponent<Image>().color = new Color(0.15f, 0.15f, 0.18f, 0.5f);
            
            ScrollRect scrollRectComp = scroll.GetComponent<ScrollRect>();
            scrollRectComp.horizontal = false;
            scrollRectComp.vertical = true;
            
            // Viewport
            GameObject viewport = new GameObject("Viewport", typeof(RectTransform), typeof(Image));
            viewport.transform.SetParent(scroll.transform, false);
            
            RectTransform vpRect = viewport.GetComponent<RectTransform>();
            vpRect.anchorMin = Vector2.zero;
            vpRect.anchorMax = Vector2.one;
            vpRect.offsetMin = Vector2.zero;
            vpRect.offsetMax = Vector2.zero;
            
            viewport.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            
            // Content
            GameObject content = new GameObject("Content", typeof(RectTransform));
            content.transform.SetParent(viewport.transform, false);
            
            RectTransform contentRect = content.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = new Vector2(0, 0);
            
            scrollRectComp.content = contentRect;
            scrollRectComp.viewport = vpRect;
            
            contentTransform = content.transform;
        }
        
        private void SetupContentLayout(Transform content)
        {
            // Grid Layout
            GridLayoutGroup grid = content.gameObject.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(100, 100);
            grid.spacing = new Vector2(15, 15);
            grid.padding = new RectOffset(15, 15, 15, 15);
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.childAlignment = TextAnchor.UpperCenter;
            
            // Content Size Fitter
            ContentSizeFitter fitter = content.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        }
        
        private void BindBackpackPanelReferences(BackpackPanel panel, GameObject panelObj)
        {
            SerializedObject serializedObj = new SerializedObject(panel);
            
            // 基础引用
            serializedObj.FindProperty("closeBtn").objectReferenceValue = panelObj.transform.Find("CloseBtn")?.GetComponent<Button>();
            serializedObj.FindProperty("tabContainer").objectReferenceValue = panelObj.transform.Find("TabContainer");
            serializedObj.FindProperty("characterTabBtn").objectReferenceValue = panelObj.transform.Find("TabContainer/CharacterTab")?.GetComponent<Button>();
            serializedObj.FindProperty("furnitureTabBtn").objectReferenceValue = panelObj.transform.Find("TabContainer/FurnitureTab")?.GetComponent<Button>();
            serializedObj.FindProperty("characterContent").objectReferenceValue = panelObj.transform.Find("ContentArea/CharacterContent")?.gameObject;
            serializedObj.FindProperty("furnitureContent").objectReferenceValue = panelObj.transform.Find("ContentArea/FurnitureContent")?.gameObject;
            
            // 列表容器
            Transform characterContent = panelObj.transform.Find("ContentArea/CharacterContent");
            if (characterContent != null)
            {
                Transform outfitContent = characterContent.Find("OutfitScroll/Viewport/Content");
                serializedObj.FindProperty("outfitListContainer").objectReferenceValue = outfitContent;
            }
            
            Transform furnitureContent = panelObj.transform.Find("ContentArea/FurnitureContent");
            if (furnitureContent != null)
            {
                Transform furnitureListContent = furnitureContent.Find("FurnitureScroll/Viewport/Content");
                serializedObj.FindProperty("furnitureListContainer").objectReferenceValue = furnitureListContent;
            }
            
            // 预制体 - 创建或加载
            GameObject outfitItemPrefab = CreateOutfitItemPrefab();
            GameObject furnitureItemPrefab = CreateFurnitureItemPrefab();
            
            OutfitItemUI outfitUI = outfitItemPrefab?.GetComponent<OutfitItemUI>();
            FurnitureItemUI furnitureUI = furnitureItemPrefab?.GetComponent<FurnitureItemUI>();
            
            serializedObj.FindProperty("outfitItemPrefab").objectReferenceValue = outfitUI;
            serializedObj.FindProperty("furnitureItemPrefab").objectReferenceValue = furnitureUI;
            
            serializedObj.ApplyModifiedProperties();
            
            Debug.Log("[BackpackPanelRebuilder] 脚本引用绑定完成");
        }
        
        private GameObject CreateOutfitItemPrefab()
        {
            string path = "Assets/Prefabs/UI/OutfitItem.prefab";
            
            // 检查是否已存在
            GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existing != null) return existing;
            
            // 创建新的
            GameObject go = new GameObject("OutfitItem", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
            go.GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.3f);
            
            // Icon
            GameObject icon = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            icon.transform.SetParent(go.transform, false);
            icon.GetComponent<RectTransform>().anchorMin = new Vector2(0.1f, 0.1f);
            icon.GetComponent<RectTransform>().anchorMax = new Vector2(0.9f, 0.9f);
            icon.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            icon.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            
            // 添加脚本
            go.AddComponent<OutfitItemUI>();
            
            // 保存
            EnsureDirectoryExists("Assets/Prefabs/UI");
            PrefabUtility.SaveAsPrefabAsset(go, path);
            DestroyImmediate(go);
            
            return AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }
        
        private GameObject CreateFurnitureItemPrefab()
        {
            string path = "Assets/Prefabs/UI/FurnitureItem.prefab";
            
            GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existing != null) return existing;
            
            GameObject go = new GameObject("FurnitureItem", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(120, 140);
            go.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.35f);
            
            // Icon
            GameObject icon = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            icon.transform.SetParent(go.transform, false);
            icon.GetComponent<RectTransform>().anchorMin = new Vector2(0.1f, 0.25f);
            icon.GetComponent<RectTransform>().anchorMax = new Vector2(0.9f, 0.9f);
            icon.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            icon.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            
            // Name
            GameObject nameObj = new GameObject("Name", typeof(RectTransform), typeof(Text));
            nameObj.transform.SetParent(go.transform, false);
            nameObj.GetComponent<RectTransform>().anchorMin = new Vector2(0.05f, 0.05f);
            nameObj.GetComponent<RectTransform>().anchorMax = new Vector2(0.95f, 0.2f);
            nameObj.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            nameObj.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            nameObj.GetComponent<Text>().text = "家具";
            nameObj.GetComponent<Text>().fontSize = 14;
            nameObj.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            nameObj.GetComponent<Text>().color = Color.white;
            nameObj.GetComponent<Text>().font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            
            // 添加脚本
            go.AddComponent<FurnitureItemUI>();
            
            EnsureDirectoryExists("Assets/Prefabs/UI");
            PrefabUtility.SaveAsPrefabAsset(go, path);
            DestroyImmediate(go);
            
            return AssetDatabase.LoadAssetAtPath<GameObject>(path);
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
